using FluentValidation;

namespace Espectaculos.Application.Commands.CreateEvento;

public class CreateEventoValidator : AbstractValidator<CreateEventoCommand>
{
    public CreateEventoValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Lugar).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Fecha).GreaterThan(DateTime.UtcNow.AddDays(-1));
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}
