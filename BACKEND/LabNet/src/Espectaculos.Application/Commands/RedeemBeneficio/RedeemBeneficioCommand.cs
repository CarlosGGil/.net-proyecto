namespace Espectaculos.Application.Commands.RedeemBeneficio;

public class RedeemBeneficioCommand
{
    public Guid BeneficioId { get; init; }
    public Guid UsuarioId { get; init; }
    public DateTime FechaUtc { get; init; } = DateTime.UtcNow;
    public bool VerificacionBiometrica { get; init; }
}
