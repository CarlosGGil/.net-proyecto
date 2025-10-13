using Espectaculos.Domain.Enums;

namespace Espectaculos.Domain.Entities;

public class Usuario
{
    public Guid id { get; set; }
    public string documento { get; set; }
    public string nombre { get; set; }
    public string apellido { get; set; }
    public string email { get; set; }
    public UsuarioEstado estado { get; set; }
    public Credencial credencial { get; set; }
    public ICollection<UsuarioRol> roles { get; set; }

    private ICollection<Dispositivo> dispositivos = new List<Dispositivo>();
    private ICollection<Canje> canjes = new List<Canje>();
    
    
    
}