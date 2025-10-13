using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Domain.Entities;
using Espectaculos.Application.Abstractions.Repositories;

namespace Espectaculos.Application.Abstractions.Repositories
{
    public interface IEventoAccesoRepository : IRepository<EventoAcceso, (Guid EventoId, DateTime MomentoDeAcceso, Guid CredencialId, Guid EspacioId)>
    {
        Task<IReadOnlyList<EventoAcceso>> ListByEventoAsync(Guid eventoId, DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default);
        Task<IReadOnlyList<EventoAcceso>> ListByCredencialAsync(Guid credencialId, DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default);
    }
}