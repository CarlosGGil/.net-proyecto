namespace Espectaculos.Application.DTOs;

public record BeneficioDto(
    Guid Id,
    string Nombre,
    string? Descripcion,
    string Tipo,
    DateTime? VigenciaInicioUtc,
    DateTime? VigenciaFinUtc,
    int? CupoTotal,
    int? CupoPorUsuario,
    bool RequiereBiometria
);
