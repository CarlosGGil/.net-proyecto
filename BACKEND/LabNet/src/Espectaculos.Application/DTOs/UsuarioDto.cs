using Espectaculos.Domain.Enums;

namespace Espectaculos.Application.DTOs;

public class UsuarioDto
{
    public Guid UsuarioId { get; set; }
    public string Documento { get; set; } = default!;
    public string Nombre { get; set; } = default!;
    public string Apellido { get; set; } = default!;
    public string Email { get; set; } = default!;
    public UsuarioEstado Estado { get; set; }
}