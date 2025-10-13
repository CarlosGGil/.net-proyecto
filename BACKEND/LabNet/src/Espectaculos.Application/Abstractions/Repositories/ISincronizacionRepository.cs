using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Domain.Entities;
using Espectaculos.Application.Abstractions.Repositories;

namespace Espectaculos.Application.Abstractions.Repositories
{
    public interface ISincronizacionRepository : IRepository<Sincronizacion, Guid>
    {
        Task<IReadOnlyList<Sincronizacion>> ListRecientesByDispositivoAsync(Guid dispositivoId, int take = 50, CancellationToken ct = default);
    }
}