using Espectaculos.Application.Abstractions.Repositories;
using MediatR;

namespace Espectaculos.Application.Usuarios.Commands.UpdateUsuario
{
    public class UpdateUsuarioHandler : IRequestHandler<UpdateUsuarioCommand>
    {
        private readonly IUsuarioRepository _repo;

        public UpdateUsuarioHandler(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(UpdateUsuarioCommand request, CancellationToken ct)
        {
            var usuario = await _repo.GetByIdAsync(request.UsuarioId, ct)
                          ?? throw new KeyNotFoundException("Usuario no encontrado");

            usuario.Nombre = request.Nombre;
            usuario.Apellido = request.Apellido;
            usuario.Email = request.Email;
            usuario.Documento = request.Documento;

            await _repo.UpdateAsync(usuario, ct);
            await _repo.SaveChangesAsync(ct); // guardamos cambios desde el repositorio
        }
    }
}