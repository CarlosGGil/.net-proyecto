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
    public class ReglaDeAccesoRepository : BaseEfRepository<ReglaDeAcceso, Guid>, IReglaDeAccesoRepository
    {
        public ReglaDeAccesoRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<IReadOnlyList<ReglaDeAcceso>> ListVigentesAsync(DateTime onDateUtc, CancellationToken ct = default)
            => await _set.AsNoTracking()
                         .Where(r => (!r.VigenciaInicio.HasValue || r.VigenciaInicio <= onDateUtc)
                                  && (!r.VigenciaFin.HasValue     || r.VigenciaFin   >= onDateUtc))
                         .ToListAsync(ct);
    }
}