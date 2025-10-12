namespace Espectaculos.Application.DTOs;

public record EventoDto(
    Guid Id,
    string Titulo,
    string Descripcion,
    DateTime Fecha,
    string Lugar,
    bool Disponible
);
