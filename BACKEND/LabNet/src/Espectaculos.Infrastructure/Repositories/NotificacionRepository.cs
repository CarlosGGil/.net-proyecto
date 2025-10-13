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
    public class NotificacionRepository : BaseEfRepository<Notificacion, Guid>, INotificacionRepository
    {
        public NotificacionRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<IReadOnlyList<Notificacion>> ListByDispositivoAsync(Guid dispositivoId, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(n => n.DispositivoId == dispositivoId).ToListAsync(ct);

        public async Task<IReadOnlyList<Notificacion>> ListProgramadasHastaAsync(DateTime hastaUtc, CancellationToken ct = default)
            => await _set.AsNoTracking()
                         .Where(n => n.ProgramadaPara != null && n.ProgramadaPara <= hastaUtc)
                         .ToListAsync(ct);
    }
}