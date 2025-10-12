namespace Espectaculos.Domain.Entities;

public class BeneficioUsuario
{
    public Guid BeneficioId { get; set; }
    public Beneficio Beneficio { get; set; } = default!;
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = default!;

}