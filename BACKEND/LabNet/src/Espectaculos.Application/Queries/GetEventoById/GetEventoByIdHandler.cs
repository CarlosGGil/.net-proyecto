using Espectaculos.Application.Abstractions;
using Espectaculos.Application.DTOs;
using System.Linq;

namespace Espectaculos.Application.Queries.GetEventoById;

public class GetEventoByIdHandler
{
    private readonly IUnitOfWork _uow;

    public GetEventoByIdHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<EventoDto?> HandleAsync(GetEventoByIdQuery query, CancellationToken ct = default)
    {
        var e = await _uow.Eventos.GetByIdAsync(query.Id, ct);
        return e is null ? null : new EventoDto(
            e.Id,
            e.Titulo,
            e.Descripcion,
            e.Fecha,
            e.Lugar,
            (e.Fecha > DateTime.UtcNow) && (((e.Entradas?.Sum(x => (int?)x.StockDisponible) ?? 0)) > 0)
        );
    }
}
