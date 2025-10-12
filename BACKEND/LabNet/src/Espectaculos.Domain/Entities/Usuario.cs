using Espectaculos.Domain.Enums;

namespace Espectaculos.Domain.Entities;

public class Usuario
{
    public Guid UsuarioId { get; set; }
    public string Documento { get; set; } = default!;
    public string Nombre { get; set; } = default!;
    public string Apellido { get; set; } = default!;
    public string Email { get; set; } = default!;
    public UsuarioTipo? Tipo { get; set; }
    public UsuarioEstado Estado { get; set; }

    public Guid? CredencialId { get; set; }
    public Credencial? Credencial { get; set; }
    
    public ICollection<BeneficioUsuario> Beneficios { get; set; } = new List<BeneficioUsuario>();

    // M:N con Rol a través de UsuarioRol
    public ICollection<UsuarioRol> UsuarioRoles { get; set; } = new List<UsuarioRol>();

    // 1:* con Dispositivo
    public ICollection<Dispositivo> Dispositivos { get; set; } = new List<Dispositivo>();

    // 1:* con Canje
    public ICollection<Canje> Canjes { get; set; } = new List<Canje>();

}