namespace Espectaculos.Application.Queries.GetEntradasPorEvento;

public class GetEntradasPorEventoQuery
{
    public Guid EventoId { get; init; }
    public GetEntradasPorEventoQuery(Guid eventoId) => EventoId = eventoId;
}
