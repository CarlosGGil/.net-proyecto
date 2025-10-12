using Espectaculos.Application.Abstractions.Repositories;

namespace Espectaculos.Application.Abstractions;

public interface IUnitOfWork
{
    IEventoRepository Eventos { get; }
    IEntradaRepository Entradas { get; }
    IOrdenRepository Ordenes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
