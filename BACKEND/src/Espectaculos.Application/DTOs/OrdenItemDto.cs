namespace Espectaculos.Application.DTOs;

public record OrdenItemDto(
    Guid Id,
    Guid OrdenId,
    Guid EventoId,
    string EntradaTipo,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal
);
