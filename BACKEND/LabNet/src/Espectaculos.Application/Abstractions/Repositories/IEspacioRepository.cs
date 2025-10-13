using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Domain.Entities;
using Espectaculos.Application.Abstractions.Repositories;

namespace Espectaculos.Application.Abstractions.Repositories
{
    public interface IEspacioRepository : IRepository<Espacio, Guid>
    {
        Task<IReadOnlyList<Espacio>> ListActivosAsync(CancellationToken ct = default);
    }
}