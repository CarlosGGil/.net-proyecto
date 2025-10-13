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
    public class BeneficioRepository : BaseEfRepository<Beneficio, Guid>, IBeneficioRepository
    {
        public BeneficioRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<IReadOnlyList<Beneficio>> ListVigentesAsync(DateTime onDateUtc, CancellationToken ct = default)
            => await _set.AsNoTracking()
                         .Where(b => (!b.VigenciaInicio.HasValue || b.VigenciaInicio <= onDateUtc)
                                  && (!b.VigenciaFin.HasValue     || b.VigenciaFin   >= onDateUtc))
                         .ToListAsync(ct);

        public async Task<IReadOnlyList<Beneficio>> SearchByNombreAsync(string term, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(b => b.Nombre.Contains(term)).ToListAsync(ct);
    }
}