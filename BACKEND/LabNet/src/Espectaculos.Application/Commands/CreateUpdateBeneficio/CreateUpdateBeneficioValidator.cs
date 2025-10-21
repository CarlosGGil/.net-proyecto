using FluentValidation;

namespace Espectaculos.Application.Commands.CreateUpdateBeneficio;

public class CreateUpdateBeneficioValidator : AbstractValidator<CreateUpdateBeneficioCommand>
{
    public CreateUpdateBeneficioValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.VigenciaFinUtc).GreaterThanOrEqualTo(x => x.VigenciaInicioUtc).When(x => x.VigenciaFinUtc.HasValue && x.VigenciaInicioUtc.HasValue);
        RuleFor(x => x.CupoTotal).GreaterThan(0).When(x => x.CupoTotal.HasValue);
        RuleFor(x => x.CupoPorUsuario).GreaterThan(0).When(x => x.CupoPorUsuario.HasValue);
    }
}
