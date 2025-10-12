using Espectaculos.Application.Abstractions;
using FluentValidation;

namespace Espectaculos.Application.Commands.PublicarEvento;

public class PublicarEventoHandler
{
    private readonly IUnitOfWork _uow;
    private readonly IValidator<PublicarEventoCommand> _validator;

    public PublicarEventoHandler(IUnitOfWork uow, IValidator<PublicarEventoCommand> validator)
    {
        _uow = uow;
        _validator = validator;
    }

    public async Task<bool> HandleAsync(PublicarEventoCommand command, CancellationToken ct = default)
    {
        await _validator.ValidateAndThrowAsync(command, ct);

        var e = await _uow.Eventos.GetByIdAsync(command.Id, ct);
        if (e is null) return false;

        e.Publicado = true;
        _uow.Eventos.Update(e);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
