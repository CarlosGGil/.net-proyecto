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
    public class EventoAccesoRepository : BaseEfRepository<EventoAcceso, (Guid EventoId, DateTime MomentoDeAcceso, Guid CredencialId, Guid EspacioId)>, IEventoAccesoRepository
    {
        public EventoAccesoRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<IReadOnlyList<EventoAcceso>> ListByEventoAsync(Guid eventoId, DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default)
        {
            var q = _set.AsNoTracking().Where(x => x.EventoId == eventoId);
            if (fromUtc.HasValue) q = q.Where(x => x.MomentoDeAcceso >= fromUtc.Value);
            if (toUtc.HasValue)   q = q.Where(x => x.MomentoDeAcceso <= toUtc.Value);
            return await q.ToListAsync(ct);
        }

        public async Task<IReadOnlyList<EventoAcceso>> ListByCredencialAsync(Guid credencialId, DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default)
        {
            var q = _set.AsNoTracking().Where(x => x.CredencialId == credencialId);
            if (fromUtc.HasValue) q = q.Where(x => x.MomentoDeAcceso >= fromUtc.Value);
            if (toUtc.HasValue)   q = q.Where(x => x.MomentoDeAcceso <= toUtc.Value);
            return await q.ToListAsync(ct);
        }
    }
}