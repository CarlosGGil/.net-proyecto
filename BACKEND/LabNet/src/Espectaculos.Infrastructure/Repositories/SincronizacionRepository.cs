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
    public class SincronizacionRepository : BaseEfRepository<Sincronizacion, Guid>, ISincronizacionRepository
    {
        public SincronizacionRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<IReadOnlyList<Sincronizacion>> ListRecientesByDispositivoAsync(Guid dispositivoId, int take = 50, CancellationToken ct = default)
            => await _set.AsNoTracking()
                         .Where(s => s.DispositivoId == dispositivoId)
                         .OrderByDescending(s => s.CreadoEn)
                         .Take(take)
                         .ToListAsync(ct);
    }
}