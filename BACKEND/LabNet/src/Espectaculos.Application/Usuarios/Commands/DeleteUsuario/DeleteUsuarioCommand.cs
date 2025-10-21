using MediatR;

namespace Espectaculos.Application.Usuarios.Commands.DeleteUsuario
{
    public record DeleteUsuarioCommand(
        Guid UsuarioId,       
        string Nombre,
        string Apellido,
        string Email,
        string Documento
    ) : IRequest;     
    
}


    
