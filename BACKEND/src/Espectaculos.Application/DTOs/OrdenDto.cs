namespace Espectaculos.Application.DTOs;

public record OrdenDto(
    Guid Id,
    string EmailComprador,
    DateTime Fecha,
    decimal Total,
    List<OrdenItemDto> Items
);
