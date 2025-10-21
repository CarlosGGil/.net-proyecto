<<<<<<< Updated upstream
namespace Espectaculos.Application.Commands.RedeemBeneficio;

public class RedeemBeneficioCommand
{
    public Guid BeneficioId { get; init; }
    public Guid UsuarioId { get; init; }
    public DateTime FechaUtc { get; init; } = DateTime.UtcNow;
    public bool VerificacionBiometrica { get; init; }
}
=======
using MediatR;

namespace Espectaculos.Application.Commands.RedeemBeneficio;

public record RedeemBeneficioCommand(Guid BeneficioId, Guid UsuarioId, bool? VerificacionBiometrica, string? Firma) : IRequest<bool>;
>>>>>>> Stashed changes
