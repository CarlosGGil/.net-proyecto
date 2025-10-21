using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Domain.Entities;
using Espectaculos.Application.Abstractions.Repositories;
using Espectaculos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Espectaculos.Infrastructure.Repositories
{
    public class CanjeRepository : BaseEfRepository<Canje, Guid>, ICanjeRepository
    {
        public CanjeRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<IReadOnlyList<Canje>> ListByUsuarioAsync(Guid usuarioId, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(c => c.UsuarioId == usuarioId).ToListAsync(ct);

        public async Task<IReadOnlyList<Canje>> ListByBeneficioAsync(Guid beneficioId, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(c => c.BeneficioId == beneficioId).ToListAsync(ct);

        public async Task<int> CountByUsuarioOnRangeAsync(Guid usuarioId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
            => await _set.AsNoTracking().CountAsync(c => c.UsuarioId == usuarioId && c.Fecha >= fromUtc && c.Fecha <= toUtc, ct);

        public async Task<int> CountConfirmedByBeneficioOnRangeAsync(Guid beneficioId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
            => await _set.AsNoTracking()
                         .CountAsync(c => c.BeneficioId == beneficioId && c.Estado == Domain.Enums.EstadoCanje.Confirmado && c.Fecha >= fromUtc && c.Fecha <= toUtc, ct);

        public async Task<int> CountConfirmedByUsuarioAndBeneficioOnRangeAsync(Guid usuarioId, Guid beneficioId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
            => await _set.AsNoTracking()
                         .CountAsync(c => c.UsuarioId == usuarioId && c.BeneficioId == beneficioId && c.Estado == Domain.Enums.EstadoCanje.Confirmado && c.Fecha >= fromUtc && c.Fecha <= toUtc, ct);
    }
}