using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Espectaculos.Infrastructure.Persistence;

namespace Espectaculos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<EspectaculosDbContext>(opt =>
            opt.UseNpgsql(cs));  

        return services;
    }
}