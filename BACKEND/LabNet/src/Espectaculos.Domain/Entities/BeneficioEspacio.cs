namespace Espectaculos.Domain.Entities;

public class BeneficioEspacio
{
    public Guid BeneficioId { get; set; }
    public 
        Beneficio Beneficio { get; set; } = default!;
    public Guid EspacioId { get; set; }
    public Espacio Espacio { get; set; } = default!;

}