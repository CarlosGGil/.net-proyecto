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
    public class EspacioReglaDeAccesoRepository : BaseEfRepository<EspacioReglaDeAcceso, (Guid EspacioId, Guid ReglaId)>, IEspacioReglaDeAccesoRepository
    {
        public EspacioReglaDeAccesoRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<IReadOnlyList<EspacioReglaDeAcceso>> ListByEspacioAsync(Guid espacioId, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(x => x.EspacioId == espacioId).ToListAsync(ct);

        public async Task<IReadOnlyList<EspacioReglaDeAcceso>> ListByReglaAsync(Guid reglaId, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(x => x.ReglaId == reglaId).ToListAsync(ct);
    }
}