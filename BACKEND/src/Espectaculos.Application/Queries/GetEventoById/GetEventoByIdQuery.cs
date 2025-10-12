namespace Espectaculos.Application.Queries.GetEventoById;

public class GetEventoByIdQuery
{
    public Guid Id { get; init; }
    public GetEventoByIdQuery(Guid id) => Id = id;
}
