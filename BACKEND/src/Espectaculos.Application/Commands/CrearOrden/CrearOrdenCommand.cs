namespace Espectaculos.Application.Commands.CrearOrden;

public class CrearOrdenCommand
{
    public string EmailComprador { get; init; } = string.Empty;
    public List<Item> Items { get; init; } = new();

    public class Item
    {
        public Guid EventoId { get; init; }
        public string EntradaTipo { get; init; } = string.Empty;
        public int Cantidad { get; init; }
    }
}
