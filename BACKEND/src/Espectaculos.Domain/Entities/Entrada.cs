using Espectaculos.Domain.Common;

namespace Espectaculos.Domain.Entities;

public class Entrada : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid EventoId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int StockTotal { get; set; }
    public int StockDisponible { get; set; }

    public Evento? Evento { get; set; }
}
