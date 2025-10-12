using Microsoft.Extensions.DependencyInjection;

namespace Espectaculos.WebApi.Health;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddPostgresHealthChecks(this IServiceCollection services, string connectionString)
    {
        services.AddHealthChecks()
            .AddNpgSql(connectionString, name: "postgres", tags: new[] { "db", "sql", "postgres" });
        return services;
    }
}
