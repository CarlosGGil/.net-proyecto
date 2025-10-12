namespace Espectaculos.Application.Commands.PublicarEvento;

public class PublicarEventoCommand
{
    public Guid Id { get; init; }
    public PublicarEventoCommand(Guid id) => Id = id;
}
