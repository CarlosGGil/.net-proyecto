namespace Espectaculos.Domain.Entities;

public class Notificacion
{
    public Guid NotificacionId { get; set; }
    public Guid DispositivoId { get; set; }
    public Dispositivo Dispositivo { get; set; } = default!;
    public string Tipo { get; set; } = default!;
    public string Titulo { get; set; } = default!;
    public string Cuerpo { get; set; } = default!;
    public DateTime? ProgramadaPara { get; set; }
    public string? Estado { get; set; }
    public List<string>? Canales { get; set; }
    public string? Metadatos { get; set; }
}