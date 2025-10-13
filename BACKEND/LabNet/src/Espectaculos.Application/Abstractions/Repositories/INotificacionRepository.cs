using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Domain.Entities;
using Espectaculos.Application.Abstractions.Repositories;

namespace Espectaculos.Application.Abstractions.Repositories
{
    public interface INotificacionRepository : IRepository<Notificacion, Guid>
    {
        Task<IReadOnlyList<Notificacion>> ListByDispositivoAsync(Guid dispositivoId, CancellationToken ct = default);
        Task<IReadOnlyList<Notificacion>> ListProgramadasHastaAsync(DateTime hastaUtc, CancellationToken ct = default);
    }
}