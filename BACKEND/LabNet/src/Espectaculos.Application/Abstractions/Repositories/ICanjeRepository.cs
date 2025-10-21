using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Domain.Entities;
using Espectaculos.Application.Abstractions.Repositories;

namespace Espectaculos.Application.Abstractions.Repositories
{
    public interface ICanjeRepository : IRepository<Canje, Guid>
    {
        Task<IReadOnlyList<Canje>> ListByUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
        Task<IReadOnlyList<Canje>> ListByBeneficioAsync(Guid beneficioId, CancellationToken ct = default);
        Task<int> CountByUsuarioOnRangeAsync(Guid usuarioId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);
        // Cuenta canjes CONFIRMADOS para un beneficio en un rango de fechas
        Task<int> CountConfirmedByBeneficioOnRangeAsync(Guid beneficioId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);
        // Cuenta canjes CONFIRMADOS para un usuario y beneficio en un rango de fechas
        Task<int> CountConfirmedByUsuarioAndBeneficioOnRangeAsync(Guid usuarioId, Guid beneficioId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);
    }
}