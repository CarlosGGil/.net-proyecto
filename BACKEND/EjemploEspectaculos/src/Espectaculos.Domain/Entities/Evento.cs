using Espectaculos.Domain.Common;

namespace Espectaculos.Domain.Entities;

public class Evento : AuditableEntity
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Lugar { get; set; } = string.Empty;
    public int Stock { get; set; } = 0;
    public bool Disponible { get; set; } = false;
    public bool Publicado { get; set; } = false;

    public ICollection<Entrada> Entradas { get; set; } = new List<Entrada>();
}
