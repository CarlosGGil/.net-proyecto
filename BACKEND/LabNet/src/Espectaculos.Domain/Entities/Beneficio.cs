using Espectaculos.Domain.Enums;

namespace Espectaculos.Domain.Entities;

public partial class Beneficio
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

    public bool EstaVigente(DateTime now) =>
        (VigenciaInicio == null || VigenciaInicio.Value <= now) &&
        (VigenciaFin == null || VigenciaFin.Value >= now);

    public bool TieneCupoGlobal() => CupoTotal == null || CupoTotal.Value > 0;

    // yaCanjeadosPorUsuario: número de canjes que tiene el usuario para este beneficio
    public bool PuedeUsuarioRedimir(int yaCanjeadosPorUsuario) =>
        (CupoPorUsuario == null || yaCanjeadosPorUsuario < CupoPorUsuario.Value) && TieneCupoGlobal();

    public void ConsumirCupoGlobal()
    {
        if (CupoTotal == null) return;
        if (CupoTotal <= 0) throw new InvalidOperationException("Cupo agotado");
        CupoTotal--;
    }
}