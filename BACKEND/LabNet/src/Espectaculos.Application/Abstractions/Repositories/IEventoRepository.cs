using Espectaculos.Application.Common;
using Espectaculos.Application.DTOs;
using Espectaculos.Domain.Entities;

namespace Espectaculos.Application.Abstractions.Repositories;

public interface IEventoRepository
{
    Task<Evento?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Evento entity, CancellationToken ct = default);
    void Update(Evento entity);

    Task<PagedResult<EventoDto>> SearchAsync(
        string? q, string? sort, string? dir,
        int page, int pageSize, bool onlyPublished, bool onlyDisponibles,
        CancellationToken cancellationToken = default);
}
