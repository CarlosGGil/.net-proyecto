using Espectaculos.Application.Abstractions;
using Espectaculos.Application.Abstractions.Repositories;
using Espectaculos.Infrastructure.Persistence;

namespace Espectaculos.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EspectaculosDbContext _db;

    public UnitOfWork(EspectaculosDbContext db,
                      IEventoRepository eventos,
                      IEntradaRepository entradas,
                      IOrdenRepository ordenes)
    {
        _db = db;
        Eventos = eventos;
        Entradas = entradas;
        Ordenes = ordenes;
    }

    public IEventoRepository Eventos { get; }
    public IEntradaRepository Entradas { get; }
    public IOrdenRepository Ordenes { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);
}
