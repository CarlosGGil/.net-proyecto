using System.Reflection;
using Espectaculos.Application;
using Espectaculos.Application.Abstractions;
using Espectaculos.Application.Abstractions.Repositories;
using Espectaculos.Application.Commands.CrearOrden;
using Espectaculos.Application.Commands.CreateEvento;
using Espectaculos.Application.Commands.PublicarEvento;
using Espectaculos.Application.Commands.CrearUsuario;
<<<<<<< Updated upstream
using Espectaculos.Application.Usuarios.Commands.CreateUsuario;
using Espectaculos.Infrastructure;
using Espectaculos.Application.Commands.CreateUpdateBeneficio;
using Espectaculos.Application.Commands.RedeemBeneficio;
=======
using Espectaculos.Infrastructure;
>>>>>>> Stashed changes
using Espectaculos.Infrastructure.Persistence;
using Espectaculos.Infrastructure.Persistence.Interceptors;
using Espectaculos.Infrastructure.Persistence.Seed;
using Espectaculos.Infrastructure.Repositories;
using Espectaculos.WebApi.Endpoints;
using Espectaculos.WebApi.Health;
using Espectaculos.WebApi.Options;
using Espectaculos.WebApi.SerilogConfig;
using FluentValidation;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
                    using Serilog;
using Microsoft.AspNetCore.StaticFiles;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Serilog.AspNetCore;
using Espectaculos.WebApi.Security;

var builder = WebApplication.CreateBuilder(args);

// Registrar handlers directamente (usamos handler inyectados en endpoints)
builder.Services.AddScoped<Espectaculos.Application.Commands.RedeemBeneficio.RedeemBeneficioHandler>();
// ---- Configuración
var config = builder.Configuration;

// Asegurar orden de configuración: JSON base -> JSON por entorno -> Variables de entorno
// Nota: soportamos variables con y sin prefijo "APP__".
// - Si usás docker-compose con APP__VALIDATION_TOKENS__SECRET, el proveedor con prefijo lo recorta a VALIDATION_TOKENS__SECRET.
// - Si usás VALIDATION_TOKENS__SECRET directamente, el proveedor sin prefijo lo toma tal cual.
builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables("APP__") // quita el prefijo "APP__" si existe
    .AddEnvironmentVariables();       // sin prefijo (toma el resto)

// Configure Serilog (reads from configuration)
builder.AddSerilogLogging();

// Ensure the Serilog ILogger instance (Log.Logger) is available in DI so
// Serilog.Extensions.Hosting.DiagnosticContext can be constructed during host validation.
builder.Services.AddSingleton<Serilog.ILogger>(Serilog.Log.Logger);

// Serilog is configured via builder.AddSerilogLogging(); the Host integration
// registers the required services (ILogger, DiagnosticContext, etc.).

// Log de diagnóstico en Development (no muestra el secreto)
if (builder.Environment.IsDevelopment())
{
    var secretPresent = string.IsNullOrWhiteSpace(builder.Configuration["ValidationTokens:Secret"]) ? "absent" : "present";
    Serilog.Log.Information("Startup diagnostic: ValidationTokens:Secret {Status} in configuration (Development).", secretPresent);
}

string connectionString =
    config.GetConnectionString("Default")
    ?? config["ConnectionStrings__Default"]
    ?? "Host=localhost;Port=5432;Database=espectaculosdb;Username=postgres;Password=postgres";

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

// JSON global options: permitir enums como cadenas y nombres de propiedad case-insensitive
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opts =>
{
    // Convertir enums a/desde string (ej: "Comedor") — y aceptar nombres case-insensitive
    opts.SerializerOptions.Converters.Add(new Espectaculos.WebApi.JsonConverters.CaseInsensitiveEnumConverterFactory());
    // Aceptar nombres de propiedad indiferentes a mayúsculas/minúsculas (p.ej. 'tipo' -> 'Tipo')
    opts.SerializerOptions.PropertyNameCaseInsensitive = true;
});

// Options: ValidationTokens (fail-fast)
var validationSection = builder.Configuration.GetSection("ValidationTokens");
builder.Services
    .AddOptions<ValidationTokenOptions>()
    .Bind(validationSection)
    // Fallbacks robustos: primero IConfiguration con ":", luego variables de entorno crudas (con y sin prefijo APP__)
    .PostConfigure(o =>
    {
        if (string.IsNullOrWhiteSpace(o.Secret))
        {
            var envSecret =
                builder.Configuration["ValidationTokens:Secret"]
                ?? Environment.GetEnvironmentVariable("VALIDATION_TOKENS__SECRET")
                ?? Environment.GetEnvironmentVariable("APP__VALIDATION_TOKENS__SECRET");
            if (!string.IsNullOrWhiteSpace(envSecret)) o.Secret = envSecret;
        }
        if (o.DefaultExpiryMinutes <= 0)
        {
            var envExpStr =
                builder.Configuration["ValidationTokens:DefaultExpiryMinutes"]
                ?? Environment.GetEnvironmentVariable("VALIDATION_TOKENS__DEFAULT_EXPIRY_MINUTES")
                ?? Environment.GetEnvironmentVariable("APP__VALIDATION_TOKENS__DEFAULT_EXPIRY_MINUTES");
            if (int.TryParse(envExpStr, out var mins) && mins > 0) o.DefaultExpiryMinutes = mins;
        }
    })
    .Validate(o => !string.IsNullOrWhiteSpace(o.Secret), "ValidationTokens:Secret es obligatorio (use env VALIDATION_TOKENS__SECRET).")
    .Validate(o => o.DefaultExpiryMinutes > 0 && o.DefaultExpiryMinutes <= 10080, "ValidationTokens:DefaultExpiryMinutes debe ser 1..10080.")
    .ValidateOnStart();

builder.Services.AddSingleton<IValidationTokenService, HmacValidationTokenService>();

// EF Core + Npgsql (simple y robusto) + infraestructura
builder.Services.AddSingleton<AuditableEntitySaveChangesInterceptor>();
builder.Services.AddInfrastructure(connectionString);

// Health checks
builder.Services.AddHealthChecks();
builder.Services.AddPostgresHealthChecks(connectionString);

// CORS (solo dev) — orígenes permitidos por config/env, con defaults sensatos
var isDev = builder.Environment.IsDevelopment();
var devOrigins = (config["Cors:AllowedOrigins"]
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

<<<<<<< Updated upstream
// Validators (Application)
builder.Services.AddScoped<IValidator<CreateEventoCommand>, CreateEventoValidator>();
builder.Services.AddScoped<IValidator<PublicarEventoCommand>, PublicarEventoValidator>();
builder.Services.AddScoped<IValidator<CrearOrdenCommand>, CrearOrdenValidator>();
//builder.Services.AddScoped<IValidator<CrearUsuarioCommand>, CrearUsuarioValidator>();
builder.Services.AddScoped<CrearUsuarioHandler>();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(
        typeof(CreateUsuarioCommand).Assembly,  // Tu capa Application, donde están los handlers
        Assembly.GetExecutingAssembly()          // La capa WebApi
    )
);

// Validators para beneficios/canje
builder.Services.AddScoped<IValidator<CreateUpdateBeneficioCommand>, CreateUpdateBeneficioValidator>();
builder.Services.AddScoped<IValidator<RedeemBeneficioCommand>, RedeemBeneficioValidator>();


// Unit of Work + repositorios (registrados desde Infrastructure)
// Nota: AddInfrastructure registra IUnitOfWork y todos los repos necesarios.
=======
// Validators and handlers are registered inside Application project or discovered by DI; skip explicit registration here to avoid type mismatches.


// Repos, UoW and Infrastructure registrations
// Centralizar el wiring de infra para que EF Tools (design-time) pueda construir los servicios
builder.Services.AddInfrastructure(connectionString);

>>>>>>> Stashed changes
// Seeder
builder.Services.AddScoped<DbSeeder>();
builder.Services.AddRouting();

var app = builder.Build();

// ---------- 1) Archivos estáticos (sirven la SPA publicada) ----------
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".dat"]  = "application/octet-stream"; // ICU/native data
provider.Mappings[".wasm"] = "application/wasm";
provider.Mappings[".br"]   = "application/octet-stream";

app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });

// ---------- 2) Logging, CORS, Swagger ----------
// NOTE: Serilog request-logging middleware temporarily disabled to avoid
// design-time/service-validation issues with DiagnosticContext during startup.
// Re-enable (app.UseSerilogRequestLogging()) after wiring Serilog DI correctly.
// if you want request logs enable it here.
if (isDev) app.UseCors("DevCors");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Espectaculos API v1");
    c.RoutePrefix = "swagger";
});

// ---------- 3) API bajo /api ----------
var api = app.MapGroup("/api");

// Mapea tus endpoints SOBRE el grupo (usar rutas relativas en las extensiones)
api.MapEventosEndpoints();
api.MapUsuariosEndpoints();
api.MapBeneficiosEndpoints();


api.MapOrdenesEndpoints();


// Health root para readiness checks fuera de /api
app.MapHealthChecks("/health");
api.MapHealthChecks("/health");

// === ADMIN: quick seed para poblar datos desde curl ===
// Uso: POST /admin/quick-seed?count=70&publish=true
var enableAdmin = (Environment.GetEnvironmentVariable("DEMO_ENABLE_ADMIN") ?? config["DEMO_ENABLE_ADMIN"] ?? "false")
    .Equals("true", StringComparison.OrdinalIgnoreCase);
if (enableAdmin)
{
    app.MapPost("/admin/quick-seed", async (int count, bool publish, EspectaculosDbContext db) =>
    {
        if (count <= 0) return Results.BadRequest("count debe ser > 0");

        var ahora = DateTime.UtcNow;
        var rnd = new Random();
        var nuevos = new List<Espectaculos.Domain.Entities.Evento>(count);

        for (int i = 0; i < count; i++)
        {
            var ev = new Espectaculos.Domain.Entities.Evento
            {
                Id = Guid.NewGuid(),
                Titulo = $"Festival Demo #{i + 1}",
                Descripcion = "Evento generado rápido para pruebas de front.",
                Fecha = ahora.AddDays(rnd.Next(3, 180)),
                Lugar = rnd.Next(0, 2) == 0 ? "Antel Arena, Montevideo" : "Luna Park, Buenos Aires",
                Publicado = publish,
                Entradas = new List<Espectaculos.Domain.Entities.Entrada>
                {
                    new() { Id = Guid.NewGuid(), Tipo = "General", Precio = rnd.Next(40,120),  StockTotal = 5000, StockDisponible = 5000 },
                    new() { Id = Guid.NewGuid(), Tipo = "Platea",  Precio = rnd.Next(120,240), StockTotal = 2000, StockDisponible = 2000 },
                    new() { Id = Guid.NewGuid(), Tipo = "VIP",     Precio = rnd.Next(240,480), StockTotal =  500, StockDisponible =  500 },
                }
            };

            foreach (var en in ev.Entradas) en.EventoId = ev.Id; // asegurar FK
            nuevos.Add(ev);
        }

        await db.Eventos.AddRangeAsync(nuevos);
        await db.SaveChangesAsync();

        return Results.Ok(new { inserted = nuevos.Count });
    });
}

// === ADMIN: publicar todos los eventos existentes ===
// Uso: POST /admin/publish-all
if (enableAdmin)
{
    app.MapPost("/admin/publish-all", async (EspectaculosDbContext db) =>
    {
        var todos = await db.Eventos.ToListAsync();
        foreach (var e in todos) e.Publicado = true;
        await db.SaveChangesAsync();
        return Results.Ok(new { updated = todos.Count });
    });
}

// ---------- 5) Migrar SIEMPRE + (opcional) SEED ----------
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

        // 1) Migraciones SIEMPRE
        logger.LogInformation("Aplicando migraciones...");
        await db.Database.MigrateAsync();
        logger.LogInformation("Migraciones aplicadas.");

        // 2) Seed sólo si lo pediste explícitamente (AUTO_SEED=true)
        var doSeed = GetFlag(config, "AUTO_SEED", false);
        if (doSeed)
        {
            logger.LogInformation("SEED solicitado → Reset + carga completa.");
            var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
            await seeder.SeedAsync(forceResetAndLoadAll: true);
            logger.LogInformation("SEED finalizado.");
        }
        else
        {
            logger.LogInformation("AUTO_SEED=false → seed omitido.");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error durante migración/seed");
        throw;
    }
}

await ApplyMigrationsAndSeedAsync();

if (enableAdmin)
{
    app.MapGet("/admin/debug-event/{id:guid}", async (Guid id, IUnitOfWork uow) => {
        var ev = await uow.Eventos.GetByIdAsync(id);
        return Results.Ok(new { ev?.Id, ev?.Disponible, ev?.Fecha, Kind = ev?.Fecha.Kind.ToString() });
    });
}

app.Run();