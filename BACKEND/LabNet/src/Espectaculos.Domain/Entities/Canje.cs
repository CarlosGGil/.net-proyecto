using Espectaculos.Domain.Enums;

namespace Espectaculos.Domain.Entities;

public class Canje
{
    public Guid CanjeId { get; set; }
    public Guid BeneficioId { get; set; }
    public Beneficio Beneficio { get; set; } = default!;
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = default!;
    public DateTime Fecha { get; set; }
    public EstadoCanje Estado { get; set; }
    public bool? VerificacionBiometrica { get; set; }
    public string? Firma { get; set; }

}