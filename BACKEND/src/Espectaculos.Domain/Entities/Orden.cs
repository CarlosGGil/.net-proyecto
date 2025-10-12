using Espectaculos.Domain.Common;

namespace Espectaculos.Domain.Entities;

public class Orden : AuditableEntity
{
    public Guid Id { get; set; }
    public string EmailComprador { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }

    // Estado de canje/validación
    public DateTime? RedeemedAtUtc { get; set; }
    public string? RedeemedBy { get; set; }

    public ICollection<OrdenItem> Items { get; set; } = new List<OrdenItem>();
}
