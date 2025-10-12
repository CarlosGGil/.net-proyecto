namespace Espectaculos.Application.DTOs;

public record EntradaDto(
    Guid Id,
    Guid EventoId,
    string Tipo,
    decimal Precio,
    int StockTotal,
    int StockDisponible
);
