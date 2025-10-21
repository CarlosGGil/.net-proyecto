using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Domain.Entities;
using Espectaculos.Application.Abstractions.Repositories;

namespace Espectaculos.Application.Abstractions.Repositories
{
    public interface IBeneficioRepository : IRepository<Beneficio, Guid>
    {
        Task<IReadOnlyList<Beneficio>> ListVigentesAsync(DateTime onDateUtc, CancellationToken ct = default);
        Task<IReadOnlyList<Beneficio>> SearchByNombreAsync(string term, CancellationToken ct = default);

        // Conteo de canjes que ya hizo un usuario para un beneficio (por validar CupoPorUsuario)
        Task<int> GetRedemptionsCountByUserAsync(Guid beneficioId, Guid usuarioId, CancellationToken ct = default);

        // Intento atómico de consumir 1 unidad del cupo global. Devuelve true si se consumió.
        Task<bool> TryConsumeGlobalCupoAsync(Guid beneficioId, CancellationToken ct = default);

        // Persistir registro de canje (auditoría)
        Task AddCanjeAsync(Canje canje, CancellationToken ct = default);

        // Actualizar entidad Beneficio (si se mantiene CupoTotal en la entidad)
        Task UpdateAsync(Beneficio beneficio, CancellationToken ct = default);

    }
}