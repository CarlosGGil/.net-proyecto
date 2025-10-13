using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Domain.Entities;
using Espectaculos.Application.Abstractions.Repositories;

namespace Espectaculos.Application.Abstractions.Repositories
{
    public interface IOrdenItemRepository : IRepository<OrdenItem, Guid>
    {
        Task<IReadOnlyList<OrdenItem>> ListByOrdenAsync(Guid ordenId, CancellationToken ct = default);
    }
}