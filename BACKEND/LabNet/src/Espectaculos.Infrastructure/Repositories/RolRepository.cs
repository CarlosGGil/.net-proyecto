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
    public class RolRepository : BaseEfRepository<Rol, Guid>, IRolRepository
    {
        public RolRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<Rol?> GetByTipoAsync(string tipo, CancellationToken ct = default)
            => await _set.AsNoTracking().FirstOrDefaultAsync(r => r.Tipo == tipo, ct);

        public async Task<IReadOnlyList<Rol>> ListByPrioridadAsync(int? min = null, int? max = null, CancellationToken ct = default)
        {
            var q = _set.AsNoTracking().AsQueryable();
            if (min.HasValue) q = q.Where(r => r.Prioridad >= min.Value);
            if (max.HasValue) q = q.Where(r => r.Prioridad <= max.Value);
            return await q.ToListAsync(ct);
        }
    }
}