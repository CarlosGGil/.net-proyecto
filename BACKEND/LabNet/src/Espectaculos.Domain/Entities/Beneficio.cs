using Espectaculos.Domain.Enums;

namespace Espectaculos.Domain.Entities;

public class Beneficio
{
    public Guid BeneficioId { get; set; }
    public BeneficioTipo Tipo { get; set; }
    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; }
    public DateTime? VigenciaInicio { get; set; }
    public DateTime? VigenciaFin { get; set; }
    public int? CupoTotal { get; set; }
    public int? CupoPorUsuario { get; set; }
    public bool RequiereBiometria { get; set; }
    public string? CriterioElegibilidad { get; set; }

    public ICollection<BeneficioEspacio> Espacios { get; set; } = new List<BeneficioEspacio>();
    public ICollection<BeneficioUsuario> Usuarios { get; set; } = new List<BeneficioUsuario>();

}