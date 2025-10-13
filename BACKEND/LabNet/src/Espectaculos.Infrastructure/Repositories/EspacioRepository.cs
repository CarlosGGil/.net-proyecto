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
    public class EspacioRepository : BaseEfRepository<Espacio, Guid>, IEspacioRepository
    {
        public EspacioRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<IReadOnlyList<Espacio>> ListActivosAsync(CancellationToken ct = default)
            => await _set.AsNoTracking().Where(e => e.Activo).ToListAsync(ct);
    }
}