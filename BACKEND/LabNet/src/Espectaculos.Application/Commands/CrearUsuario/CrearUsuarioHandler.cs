using Espectaculos.Application.Abstractions;
using Espectaculos.Domain.Entities;
using FluentValidation;

namespace Espectaculos.Application.Commands.CrearUsuario;

public class CrearUsuarioHandler
{
    private readonly IUnitOfWork _uow;
    private readonly IValidator<CrearUsuarioCommand> _validator;

    public CrearUsuarioHandler(IUnitOfWork uow, IValidator<CrearUsuarioCommand> validator)
    {
        _uow = uow;
        _validator = validator;
    }
    
    public async Task<Guid> HandleAsync(CrearUsuarioCommand command, CancellationToken ct = default)
    {
        await _validator.ValidateAndThrowAsync(command, ct);

        var e = new Usuario
        {
            UsuarioId = Guid.NewGuid(),
            Documento = command.Documento.Trim(),
            Nombre = command.Nombre.Trim(),
            Apellido = command.Apellido.Trim(),
            Email = command.Email.Trim(),
        };

        await _uow.Usuarios.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return e.UsuarioId;
    }
    
    
}