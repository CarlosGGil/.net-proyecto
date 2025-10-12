using FluentValidation;

namespace Espectaculos.Application.Commands.CrearOrden;

public class CrearOrdenValidator : AbstractValidator<CrearOrdenCommand>
{
    public CrearOrdenValidator()
    {
        RuleFor(x => x.EmailComprador).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(i =>
        {
            i.RuleFor(x => x.EventoId).NotEmpty();
            i.RuleFor(x => x.EntradaTipo).NotEmpty().MaximumLength(100);
            i.RuleFor(x => x.Cantidad).GreaterThanOrEqualTo(0);
        });
        
        RuleFor(x => x.Items)
            .Must(items => items.Any(i => i.Cantidad >= 1))
            .WithMessage("Al menos una cantidad debe ser mayor o igual a 1.");
        
        RuleFor(x => x.Items)
            .Must(items => items.Select(i => (i.EventoId, i.EntradaTipo)).Distinct().Count() == items.Count)
            .WithMessage("No se permiten items duplicados (mismo EventoId y EntradaTipo).");
        
        
    }
}
