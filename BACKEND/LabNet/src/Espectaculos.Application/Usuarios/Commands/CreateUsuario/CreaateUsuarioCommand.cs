using MediatR;

namespace Espectaculos.Application.Usuarios.Commands.CreateUsuario
{
    public record CreateUsuarioCommand(
        string Nombre,
        string Apellido,
        string Email,
        string Documento,
        string Password
    ) : IRequest<Guid>;
}