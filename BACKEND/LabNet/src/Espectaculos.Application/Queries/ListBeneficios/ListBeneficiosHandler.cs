using Espectaculos.Application.Abstractions.Repositories;
using Espectaculos.Application.DTOs;

namespace Espectaculos.Application.Queries.ListBeneficios;

public sealed class ListBeneficiosHandler
{
    private readonly IBeneficioRepository _repo;

    public ListBeneficiosHandler(IBeneficioRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<BeneficioDto>> HandleAsync(ListBeneficiosQuery q, CancellationToken ct = default)
    {
        var ahora = DateTime.UtcNow;
        var beneficios = await _repo.ListVigentesAsync(ahora, ct);
        return beneficios.Select(b => new BeneficioDto(
            b.BeneficioId,
            b.Nombre,
            b.Descripcion,
            b.Tipo.ToString(),
            b.VigenciaInicio,
            b.VigenciaFin,
            b.CupoTotal,
            b.CupoPorUsuario,
            b.RequiereBiometria));
    }
}
