namespace Espectaculos.Domain.Entities;

public class OrdenItem
{
    public Guid Id { get; set; }
    public Guid OrdenId { get; set; }
    public Guid EventoId { get; set; }
    public string EntradaTipo { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }

    public Orden? Orden { get; set; }
    public Evento? Evento { get; set; }
}
