using Espectaculos.Domain.Enums;

namespace Espectaculos.Application.Commands.CreateUpdateBeneficio;

public class CreateUpdateBeneficioCommand
{
    public Guid? Id { get; init; }
    public BeneficioTipo Tipo { get; init; }
    public string Nombre { get; init; } = default!;
    public string? Descripcion { get; init; }
    public DateTime? VigenciaInicioUtc { get; init; }
    public DateTime? VigenciaFinUtc { get; init; }
    public int? CupoTotal { get; init; }
    public int? CupoPorUsuario { get; init; }
    public bool RequiereBiometria { get; init; }
    public string? CriterioElegibilidad { get; init; }
}
