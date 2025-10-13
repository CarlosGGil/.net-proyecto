using Espectaculos.Domain.Enums;
using Espectaculos.Domain.Entities;

public class Usuario
{
    public Guid id { get; set; }
    public string documento { get; set; }
    public string nombre { get; set; }
    public string apellido { get; set; }
    public string email { get; set; }
    public UsuarioEstado estado { get; set; }
    public Credencial credencial { get; set; }
    public ICollection<UsuarioRol> UsuarioRoles { get; set; } = new List<UsuarioRol>();

    public ICollection<Dispositivo>  Dispositivos { get; set; } = new List<Dispositivo>();
    public ICollection<BeneficioUsuario> Beneficios { get; set; } = new List<BeneficioUsuario>();
    public ICollection<Canje> Canjes { get; set; }= new List<Canje>();



}