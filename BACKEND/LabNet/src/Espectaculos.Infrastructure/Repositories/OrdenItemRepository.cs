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
    public class OrdenItemRepository : BaseEfRepository<OrdenItem, Guid>, IOrdenItemRepository
    {
        public OrdenItemRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<IReadOnlyList<OrdenItem>> ListByOrdenAsync(Guid ordenId, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(oi => oi.OrdenId == ordenId).ToListAsync(ct);
    }
}