using Espectaculos.Application.Abstractions;
using Espectaculos.Domain.Entities;
using FluentValidation;

namespace Espectaculos.Application.Commands.CreateUpdateBeneficio;

    public class CreateUpdateBeneficioHandler
    {
    private readonly IUnitOfWork _uow;
    private readonly IValidator<CreateUpdateBeneficioCommand> _validator;

    public CreateUpdateBeneficioHandler(IUnitOfWork uow, IValidator<CreateUpdateBeneficioCommand> validator)
    {
        _uow = uow;
        _validator = validator;
    }

    public async Task<Guid> HandleAsync(CreateUpdateBeneficioCommand cmd, CancellationToken ct = default)
    {
        await _validator.ValidateAndThrowAsync(cmd, ct);

        if (cmd.Id.HasValue)
        {
            var existing = await _uow.Beneficios.GetByIdAsync(cmd.Id.Value, ct);
            if (existing is null) throw new KeyNotFoundException("Beneficio no encontrado");
            existing.Nombre = cmd.Nombre;
            existing.Descripcion = cmd.Descripcion;
            existing.Tipo = cmd.Tipo;
            existing.VigenciaInicio = cmd.VigenciaInicioUtc;
            existing.VigenciaFin = cmd.VigenciaFinUtc;
            existing.CupoTotal = cmd.CupoTotal;
            existing.CupoPorUsuario = cmd.CupoPorUsuario;
            existing.RequiereBiometria = cmd.RequiereBiometria;
            existing.CriterioElegibilidad = cmd.CriterioElegibilidad;

            _uow.Beneficios.Update(existing);
            await _uow.SaveChangesAsync(ct);
            return existing.BeneficioId;
        }

        var b = new Beneficio
        {
            BeneficioId = Guid.NewGuid(),
            Tipo = cmd.Tipo,
            Nombre = cmd.Nombre,
            Descripcion = cmd.Descripcion,
            VigenciaInicio = cmd.VigenciaInicioUtc,
            VigenciaFin = cmd.VigenciaFinUtc,
            CupoTotal = cmd.CupoTotal,
            CupoPorUsuario = cmd.CupoPorUsuario,
            RequiereBiometria = cmd.RequiereBiometria,
            CriterioElegibilidad = cmd.CriterioElegibilidad
        };

        await _uow.Beneficios.AddAsync(b, ct);
        await _uow.SaveChangesAsync(ct);
        return b.BeneficioId;
    }
}
