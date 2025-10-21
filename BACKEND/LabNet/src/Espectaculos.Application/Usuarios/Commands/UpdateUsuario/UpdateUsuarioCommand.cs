using MediatR;

namespace Espectaculos.Application.Usuarios.Commands.UpdateUsuario
{
    public record UpdateUsuarioCommand(
        Guid UsuarioId,       
        string Nombre,
        string Apellido,
        string Email,
        string Documento
    ) : IRequest;            
}