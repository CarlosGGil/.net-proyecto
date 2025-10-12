namespace Espectaculos.Application.Queries.ListEventos;

public class ListEventosQuery
{
    public string? Q { get; init; }
    public string? Sort { get; init; } // fecha|titulo|lugar
    public string? Dir { get; init; } // asc|desc
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public bool OnlyPublished { get; init; } = false;
    public bool OnlyDisponibles { get; init; } = false;
}
