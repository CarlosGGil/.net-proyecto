using Espectaculos.Application.Abstractions.Repositories;
using Espectaculos.Application.Common;
using Espectaculos.Application.DTOs;

namespace Espectaculos.Application.Queries.ListEventos;

public sealed class ListEventosHandler
{
    private readonly IEventoRepository _repo;

    public ListEventosHandler(IEventoRepository repo)
    {
        _repo = repo;
    }

    public Task<PagedResult<EventoDto>> HandleAsync(ListEventosQuery query, CancellationToken ct = default)
        => _repo.SearchAsync(
            q: query.Q,
            sort: query.Sort,
            dir: query.Dir,
            page: query.Page,
            pageSize: query.PageSize,
            onlyPublished: query.OnlyPublished,
            onlyDisponibles: query.OnlyDisponibles,
            cancellationToken: ct);
}
