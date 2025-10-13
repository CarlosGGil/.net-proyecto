using Espectaculos.Application;
using Espectaculos.Application.Abstractions;
using Espectaculos.Application.Abstractions.Repositories;
using Espectaculos.Application.Commands.CrearOrden;
using Espectaculos.Application.Commands.CreateEvento;
using Espectaculos.Application.Commands.PublicarEvento;
using Espectaculos.Infrastructure.Persistence;
using Espectaculos.Infrastructure.Persistence.Interceptors;
using Espectaculos.Infrastructure.Persistence.Seed;
using Espectaculos.Infrastructure.Repositories;
using Espectaculos.WebApi.Endpoints;
using Espectaculos.WebApi.Health;
using Espectaculos.WebApi.Options;
using Espectaculos.WebApi.SerilogConfig;
using Espectaculos.WebApi.Security;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.StaticFiles;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// ---- Logging (Serilog)
builder.AddSerilogLogging();

// ---- Configuración base
builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables("APP__")
    .AddEnvironmentVariables();

// ---- Diagnóstico en Development
if (builder.Environment.IsDevelopment())
{
    var secretPresent = string.IsNullOrWhiteSpace(builder.Configuration["ValidationTokens:Secret"]) ? "absent" : "present";
    Log.Information("Startup diagnostic: ValidationTokens:Secret {Status} in configuration (Development).", secretPresent);
}

// ---- Connection string resolution
var inContainer = string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.OrdinalIgnoreCase);

string? cs =
    builder.Configuration.GetConnectionString("EspectaculosDb")         
    ?? builder.Configuration.GetConnectionString("Default")             
    ?? builder.Configuration["ConnectionStrings__EspectaculosDb"]       
    ?? builder.Configuration["ConnectionStrings__Default"];

if (string.IsNullOrWhiteSpace(cs))
{
    var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? (inContainer ? "db" : "localhost");
    var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
    var db   = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "espectaculosdb";
    var usr  = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
    var pwd  = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres";
    cs = $"Host={host};Port={port};Database={db};Username={usr};Password={pwd}";
}

var connectionString = cs;

// ---- Servicios
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Espectáculos - Demo API",
        Version = "v1",
        Description = "API pública para la demo. Incluye endpoints de eventos y órdenes. Endpoints de administración permanecen ocultos por defecto."
    });
});

// ---- Validation tokens
var validationSection = builder.Configuration.GetSection("ValidationTokens");
builder.Services
    .AddOptions<ValidationTokenOptions>()
    .Bind(validationSection)
    .Validate(o => !string.IsNullOrWhiteSpace(o.Secret), "ValidationTokens:Secret es obligatorio.")
    .Validate(o => o.DefaultExpiryMinutes > 0 && o.DefaultExpiryMinutes <= 10080, "ValidationTokens:DefaultExpiryMinutes debe ser 1..10080.")
    .ValidateOnStart();

builder.Services.AddSingleton<IValidationTokenService, HmacValidationTokenService>();

// ---- EF Core + Npgsql
builder.Services.AddSingleton<AuditableEntitySaveChangesInterceptor>();
builder.Services.AddDbContext<EspectaculosDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// ---- Health checks
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "postgres");

// ---- CORS (dev only)
var isDev = builder.Environment.IsDevelopment();
var devOrigins = (builder.Configuration["Cors:AllowedOrigins"]
                  ?? Environment.GetEnvironmentVariable("CORS_ORIGINS")
                  ?? "http://localhost:5262,http://localhost:5173")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

if (isDev)
{
    builder.Services.AddCors(o =>
    {
        o.AddPolicy("DevCors", p =>
            p.WithOrigins(devOrigins)
             .AllowAnyHeader()
             .AllowAnyMethod()
        );
    });
}

// ---- Validators
builder.Services.AddScoped<IValidator<CreateEventoCommand>, CreateEventoValidator>();
builder.Services.AddScoped<IValidator<PublicarEventoCommand>, PublicarEventoValidator>();
builder.Services.AddScoped<IValidator<CrearOrdenCommand>, CrearOrdenValidator>();

// ---- Repos + UoW
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEventoRepository, EventoRepository>();
builder.Services.AddScoped<IEntradaRepository, EntradaRepository>();
builder.Services.AddScoped<IOrdenRepository, OrdenRepository>();

// ---- Seeder
builder.Services.AddScoped<DbSeeder>();
builder.Services.AddRouting();

var app = builder.Build();

// ---------- Static files ----------
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".dat"]  = "application/octet-stream";
provider.Mappings[".wasm"] = "application/wasm";
provider.Mappings[".br"]   = "application/octet-stream";

app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });

// ---------- Logging / Swagger ----------
app.UseSerilogRequestLogging();
if (isDev) app.UseCors("DevCors");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Espectaculos API v1");
    c.RoutePrefix = "swagger";
});

// ---------- API under /api ----------
var api = app.MapGroup("/api");
api.MapEventosEndpoints();
api.MapOrdenesEndpoints();

// ---------- Health checks ----------
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).AllowAnonymous();

app.MapGet("/healthz", () => Results.Ok(new { ok = true })).AllowAnonymous();

// ---------- Auto migrations + seed ----------
static bool GetFlag(IConfiguration cfg, string key, bool def = false)
    => (Environment.GetEnvironmentVariable(key)
        ?? cfg[key]
        ?? (def ? "true" : "false"))
       .Equals("true", StringComparison.OrdinalIgnoreCase);

async Task ApplyMigrationsAndSeedAsync()
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

    try
    {
        var db = scope.ServiceProvider.GetRequiredService<EspectaculosDbContext>();

        logger.LogInformation("Applying migrations...");
        await db.Database.MigrateAsync();
        logger.LogInformation("Migrations applied.");

        var doSeed = GetFlag(builder.Configuration, "AUTO_SEED", false);
        if (doSeed)
        {
            logger.LogInformation("Seeding database...");
            var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
            await seeder.SeedAsync(forceResetAndLoadAll: true);
            logger.LogInformation("Seed complete.");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error during migration/seed");
        throw;
    }
}

await ApplyMigrationsAndSeedAsync();

app.Run();
