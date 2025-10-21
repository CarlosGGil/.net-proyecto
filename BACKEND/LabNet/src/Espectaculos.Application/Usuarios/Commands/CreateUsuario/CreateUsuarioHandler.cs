using Espectaculos.Application.Abstractions.Repositories;
using Espectaculos.Application.Usuarios.Commands.CreateUsuario;
using Espectaculos.Domain.Entities;
using Espectaculos.Domain.Enums;
using MediatR;

namespace Espectaculos.Application.Usuarios.Commands.CrearUsuario
{
    public class CrearUsuarioCommandHandler : IRequestHandler<CreateUsuarioCommand, Guid>
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public CrearUsuarioCommandHandler(
            IUsuarioRepository usuarioRepository
        )
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Guid> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = new Usuario
            {
                UsuarioId = Guid.NewGuid(),
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Documento = request.Documento,
                Email = request.Email,
                PasswordHash = request.Password,
                Estado = UsuarioEstado.Activo
            };

            await _usuarioRepository.AddAsync(usuario, cancellationToken);
            await _usuarioRepository.SaveChangesAsync(cancellationToken);

            return usuario.UsuarioId;
        }
    }
}