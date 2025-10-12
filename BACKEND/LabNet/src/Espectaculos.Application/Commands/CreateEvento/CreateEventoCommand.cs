namespace Espectaculos.Application.Commands.CreateEvento;

public class CreateEventoCommand
{
    public string Titulo { get; init; } = string.Empty;
    public string Descripcion { get; init; } = string.Empty;
    public DateTime Fecha { get; init; }
    public string Lugar { get; init; } = string.Empty;
    public int Stock { get; init; } = 0;
    public bool? Disponible { get; init; }
}
