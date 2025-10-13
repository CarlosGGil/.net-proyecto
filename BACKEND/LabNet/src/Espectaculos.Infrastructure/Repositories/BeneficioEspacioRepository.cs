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
    public class BeneficioEspacioRepository : BaseEfRepository<BeneficioEspacio, (Guid BeneficioId, Guid EspacioId)>, IBeneficioEspacioRepository
    {
        public BeneficioEspacioRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<IReadOnlyList<BeneficioEspacio>> ListByBeneficioAsync(Guid beneficioId, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(x => x.BeneficioId == beneficioId).ToListAsync(ct);

        public async Task<IReadOnlyList<BeneficioEspacio>> ListByEspacioAsync(Guid espacioId, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(x => x.EspacioId == espacioId).ToListAsync(ct);
    }
}