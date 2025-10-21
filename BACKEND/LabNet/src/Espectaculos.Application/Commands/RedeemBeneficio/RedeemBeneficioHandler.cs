<<<<<<< Updated upstream
using Espectaculos.Application.Abstractions;
using Espectaculos.Domain.Entities;
using Espectaculos.Domain.Enums;
using FluentValidation;

namespace Espectaculos.Application.Commands.RedeemBeneficio;

    public class RedeemBeneficioHandler
    {
    private readonly IUnitOfWork _uow;
    private readonly IValidator<RedeemBeneficioCommand> _validator;

    public RedeemBeneficioHandler(IUnitOfWork uow, IValidator<RedeemBeneficioCommand> validator)
    {
        _uow = uow;
        _validator = validator;
    }

    public async Task<Guid> HandleAsync(RedeemBeneficioCommand cmd, CancellationToken ct = default)
    {
        await _validator.ValidateAndThrowAsync(cmd, ct);

        // 1) Obtener beneficio
        var beneficio = await _uow.Beneficios.GetByIdAsync(cmd.BeneficioId, ct);
        if (beneficio is null) throw new KeyNotFoundException("Beneficio no encontrado");

        var now = cmd.FechaUtc;
        // 2) Vigencia
        if (beneficio.VigenciaInicio.HasValue && beneficio.VigenciaInicio > now)
            throw new InvalidOperationException("Beneficio aún no vigente");
        if (beneficio.VigenciaFin.HasValue && beneficio.VigenciaFin < now)
            throw new InvalidOperationException("Beneficio expirado");

        // 3) Cupo total
        if (beneficio.CupoTotal.HasValue)
        {
            var desde = now.Date;
            var hasta = now.Date.AddDays(1).AddTicks(-1);
            var totalConfirmados = await _uow.Canjes.CountConfirmedByBeneficioOnRangeAsync(beneficio.BeneficioId, desde, hasta, ct);
            if (totalConfirmados >= beneficio.CupoTotal.Value)
                throw new InvalidOperationException("Cupo total agotado");
        }

        // 4) Cupo por usuario
        if (beneficio.CupoPorUsuario.HasValue)
        {
            var desde = now.Date;
            var hasta = now.Date.AddDays(1).AddTicks(-1);
            var userCount = await _uow.Canjes.CountConfirmedByUsuarioAndBeneficioOnRangeAsync(cmd.UsuarioId, beneficio.BeneficioId, desde, hasta, ct);
            if (userCount >= beneficio.CupoPorUsuario.Value)
                throw new InvalidOperationException("Límite por usuario alcanzado");
        }

        // 5) Validar existencia de usuario
        var usuario = await _uow.Usuarios.GetByIdAsync(cmd.UsuarioId, ct);
        if (usuario is null) throw new KeyNotFoundException("Usuario no encontrado");

        // 6) Registrar canje confirmado
        var canje = new Canje
        {
            CanjeId = Guid.NewGuid(),
            BeneficioId = beneficio.BeneficioId,
            UsuarioId = cmd.UsuarioId,
            Fecha = now,
            Estado = EstadoCanje.Confirmado,
            VerificacionBiometrica = cmd.VerificacionBiometrica
        };
        try
        {
            await _uow.Canjes.AddAsync(canje, ct);
            await _uow.SaveChangesAsync(ct);
            return canje.CanjeId;
        }
        catch (Exception ex)
        {
            // No referenciamos EF Core en la capa Application; convertir cualquier error de persistencia
            // en una excepción de negocio para que la capa WebApi la transforme en una respuesta adecuada.
            throw new InvalidOperationException("Error al registrar el canje: posible violación de integridad referencial o conflicto en la base de datos.", ex);
        }
=======
using MediatR;
using Espectaculos.Application.Abstractions.Repositories;
using Espectaculos.Domain.Entities;

namespace Espectaculos.Application.Commands.RedeemBeneficio;

public class RedeemBeneficioHandler : IRequestHandler<RedeemBeneficioCommand, bool>
{
    private readonly IBeneficioRepository _repo;

    public RedeemBeneficioHandler(IBeneficioRepository repo) => _repo = repo;

    public async Task<bool> Handle(RedeemBeneficioCommand request, CancellationToken cancellationToken)
    {
        var ahora = DateTime.UtcNow;

        var beneficio = await _repo.GetByIdAsync(request.BeneficioId, cancellationToken);
        if (beneficio == null) return false;

        // Validar vigencia
        if (beneficio.VigenciaInicio.HasValue && beneficio.VigenciaInicio > ahora) return false;
        if (beneficio.VigenciaFin.HasValue && beneficio.VigenciaFin < ahora) return false;

        // Contar canjes del usuario
        var ya = await _repo.GetRedemptionsCountByUserAsync(request.BeneficioId, request.UsuarioId, cancellationToken);
        if (beneficio.CupoPorUsuario.HasValue && ya >= beneficio.CupoPorUsuario.Value) return false;

        // Intento atómico de consumir cupo global
        var consumido = await _repo.TryConsumeGlobalCupoAsync(request.BeneficioId, cancellationToken);
        if (!consumido) return false;

        // Registrar canje
        var canje = new Canje
        {
            CanjeId = Guid.NewGuid(),
            BeneficioId = request.BeneficioId,
            UsuarioId = request.UsuarioId,
            Fecha = DateTime.UtcNow,
            Estado = Espectaculos.Domain.Enums.EstadoCanje.Confirmado,
            VerificacionBiometrica = request.VerificacionBiometrica,
            Firma = request.Firma
        };

        await _repo.AddCanjeAsync(canje, cancellationToken);

        return true;
>>>>>>> Stashed changes
    }
}
