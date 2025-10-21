using System;
using Espectaculos.Infrastructure.Persistence;
using Espectaculos.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Espectaculos.Infrastructure
{
    /// <summary>
    /// Design-time factory so `dotnet ef` can create a <see cref="EspectaculosDbContext"/>
    /// without requiring the full application DI graph.
    /// Reads a connection string from the environment variable DB_CONN or falls back to a sensible default.
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EspectaculosDbContext>
    {
        public EspectaculosDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<EspectaculosDbContext>();

            // Allow overriding via env var for CI/locals
            var conn = Environment.GetEnvironmentVariable("DB_CONN")
                       ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default")
                       ?? "Host=localhost;Port=5432;Database=espectaculosdb;Username=postgres;Password=postgres";

            builder.UseNpgsql(conn, npgsqlOptions => { /* keep defaults */ });

            // The interceptor has no external deps so we can instantiate it here.
            var interceptor = new AuditableEntitySaveChangesInterceptor();

            return new EspectaculosDbContext(builder.Options, interceptor);
        }
    }
}
