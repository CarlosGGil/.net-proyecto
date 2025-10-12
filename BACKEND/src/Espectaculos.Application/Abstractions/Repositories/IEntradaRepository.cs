using Espectaculos.Domain.Entities;

namespace Espectaculos.Application.Abstractions.Repositories;

public interface IEntradaRepository
{
    Task<List<Entrada>> GetByEventoIdAsync(Guid eventoId, CancellationToken ct = default);
    Task<Entrada?> GetByEventoAndTipoAsync(Guid eventoId, string tipo, CancellationToken ct = default);
    void Update(Entrada entity);
}
