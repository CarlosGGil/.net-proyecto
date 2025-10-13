using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Domain.Entities;
using Espectaculos.Application.Abstractions.Repositories;

namespace Espectaculos.Application.Abstractions.Repositories
{
    public interface IRolRepository : IRepository<Rol, Guid>
    {
        Task<Rol?> GetByTipoAsync(string tipo, CancellationToken ct = default);
        Task<IReadOnlyList<Rol>> ListByPrioridadAsync(int? min = null, int? max = null, CancellationToken ct = default);
    }
}