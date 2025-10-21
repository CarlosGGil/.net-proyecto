using FluentValidation;

namespace Espectaculos.Application.Commands.RedeemBeneficio;

public class RedeemBeneficioValidator : AbstractValidator<RedeemBeneficioCommand>
{
    public RedeemBeneficioValidator()
    {
        RuleFor(x => x.BeneficioId).NotEmpty();
        RuleFor(x => x.UsuarioId).NotEmpty();
    }
}
