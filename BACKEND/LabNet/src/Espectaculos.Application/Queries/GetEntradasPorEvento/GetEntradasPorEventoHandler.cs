using Espectaculos.Application.Abstractions;
using Espectaculos.Application.DTOs;

namespace Espectaculos.Application.Queries.GetEntradasPorEvento;

public class GetEntradasPorEventoHandler
{
    private readonly IUnitOfWork _uow;

    public GetEntradasPorEventoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<List<EntradaDto>> HandleAsync(GetEntradasPorEventoQuery query, CancellationToken ct = default)
    {
        var entradas = await _uow.Entradas.GetByEventoIdAsync(query.EventoId, ct);
        return entradas
            .OrderBy(e => e.Precio)
            .Select(e => new EntradaDto(e.Id, e.EventoId, e.Tipo, e.Precio, e.StockTotal, e.StockDisponible))
            .ToList();
    }
}
