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
    public class CredencialRepository : BaseEfRepository<Credencial, Guid>, ICredencialRepository
    {
        public CredencialRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<IReadOnlyList<Credencial>> ListVigentesAsync(DateTime onDateUtc, CancellationToken ct = default)
            => await _set.AsNoTracking()
                         .Where(c => (!c.FechaExpiracion.HasValue || c.FechaExpiracion >= onDateUtc)
                                  &&  c.FechaEmision <= onDateUtc)
                         .ToListAsync(ct);
    }
}