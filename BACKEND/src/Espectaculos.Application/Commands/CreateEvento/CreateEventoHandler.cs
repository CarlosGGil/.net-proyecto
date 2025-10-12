using Espectaculos.Application.Abstractions;
using Espectaculos.Domain.Entities;
using FluentValidation;

namespace Espectaculos.Application.Commands.CreateEvento;

public class CreateEventoHandler
{
    private readonly IUnitOfWork _uow;
    private readonly IValidator<CreateEventoCommand> _validator;

    public CreateEventoHandler(IUnitOfWork uow, IValidator<CreateEventoCommand> validator)
    {
        _uow = uow;
        _validator = validator;
    }

    public async Task<Guid> HandleAsync(CreateEventoCommand command, CancellationToken ct = default)
    {
        await _validator.ValidateAndThrowAsync(command, ct);

        var e = new Evento
        {
            Id = Guid.NewGuid(),
            Titulo = command.Titulo.Trim(),
            Descripcion = command.Descripcion.Trim(),
            Fecha = command.Fecha,
            Lugar = command.Lugar.Trim(),
            Stock = Math.Max(0, command.Stock),
            Disponible = command.Disponible ?? (command.Stock > 0)
        };

        await _uow.Eventos.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return e.Id;
    }
}
