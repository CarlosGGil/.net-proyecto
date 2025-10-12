namespace Espectaculos.Application.Commands.CrearOrden;

public class CrearOrdenResult
{
    public Guid Id { get; init; }
    public string EmailComprador { get; init; } = string.Empty;
    public DateTime Fecha { get; init; }
    public decimal Total { get; init; }
    public List<Item> Items { get; init; } = new();

    public class Item
    {
        public Guid EventoId { get; init; }
        public string EntradaTipo { get; init; } = string.Empty;
        public int Cantidad { get; init; }
        public decimal PrecioUnitario { get; init; }
        public decimal Subtotal { get; init; }
    }
}
