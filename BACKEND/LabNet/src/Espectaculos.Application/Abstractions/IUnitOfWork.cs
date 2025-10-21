using Espectaculos.Application.Abstractions.Repositories;

namespace Espectaculos.Application.Abstractions;

public interface IUnitOfWork
{
    IEventoRepository Eventos { get; }
    IEntradaRepository Entradas { get; }
    IOrdenRepository Ordenes { get; }
    IUsuarioRepository Usuarios { get; }
    IBeneficioRepository Beneficios { get; }
    ICanjeRepository Canjes { get; }
    IBeneficioUsuarioRepository BeneficioUsuarios { get; }
    IBeneficioEspacioRepository BeneficioEspacios { get; }

Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
