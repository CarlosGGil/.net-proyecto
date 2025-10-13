using Espectaculos.Application.Abstractions;
using Espectaculos.Application.Abstractions.Repositories;
using Espectaculos.Infrastructure.Persistence;
using Espectaculos.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Espectaculos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // DbContext (Scoped by default)
        services.AddDbContext<EspectaculosDbContext>(options =>
            options.UseNpgsql(connectionString)); // or SqlServer, etc.

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories (SCOPED is typical for EF)
        services.AddScoped<IDispositivoRepository, DispositivoRepository>();
        services.AddScoped<IBeneficioRepository, BeneficioRepository>();
        services.AddScoped<ICanjeRepository, CanjeRepository>();
        services.AddScoped<ICredencialRepository, CredencialRepository>();
        services.AddScoped<IEntradaRepository, EntradaRepository>();
        services.AddScoped<IEspacioRepository, EspacioRepository>();
        services.AddScoped<IEventoRepository, EventoRepository>();
        services.AddScoped<INotificacionRepository, NotificacionRepository>();
        services.AddScoped<IOrdenRepository, OrdenRepository>();
        services.AddScoped<IOrdenItemRepository, OrdenItemRepository>();
        services.AddScoped<IReglaDeAccesoRepository, ReglaDeAccesoRepository>();
        services.AddScoped<IRolRepository, RolRepository>();
        services.AddScoped<ISincronizacionRepository, SincronizacionRepository>();
        services.AddScoped<IBeneficioEspacioRepository, BeneficioEspacioRepository>();
        services.AddScoped<IBeneficioUsuarioRepository, BeneficioUsuarioRepository>();
        services.AddScoped<IEspacioReglaDeAccesoRepository, EspacioReglaDeAccesoRepository>();
        services.AddScoped<IEventoAccesoRepository, EventoAccesoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IUsuarioRolRepository, UsuarioRolRepository>();

        return services;
    }
}
