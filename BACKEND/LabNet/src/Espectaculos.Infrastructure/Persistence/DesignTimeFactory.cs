using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Espectaculos.Infrastructure.Persistence;

public class DesignTimeFactory : IDesignTimeDbContextFactory<EspectaculosDbContext>
{
    public EspectaculosDbContext CreateDbContext(string[] args)
    {
        // Read env vars if present; otherwise use sensible defaults.
        var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        var db   = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "espectaculosdb";
        var usr  = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
        var pwd  = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "hola123";

        var cs = $"Host={host};Port={port};Database={db};Username={usr};Password={pwd}";

        var options = new DbContextOptionsBuilder<EspectaculosDbContext>()
            .UseNpgsql(cs) 
            .Options;

        return new EspectaculosDbContext(options);
    }
}