using FluentValidation;

namespace Espectaculos.Application.Commands.PublicarEvento;

public class PublicarEventoValidator : AbstractValidator<PublicarEventoCommand>
{
    public PublicarEventoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
