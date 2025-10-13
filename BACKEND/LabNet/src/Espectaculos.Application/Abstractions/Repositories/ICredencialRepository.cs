using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Domain.Entities;
using Espectaculos.Application.Abstractions.Repositories;

namespace Espectaculos.Application.Abstractions.Repositories
{
    public interface ICredencialRepository : IRepository<Credencial, Guid>
    {
        Task<IReadOnlyList<Credencial>> ListVigentesAsync(DateTime onDateUtc, CancellationToken ct = default);
    }
}