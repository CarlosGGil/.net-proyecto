using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Domain.Entities;
using Espectaculos.Application.Abstractions.Repositories;

namespace Espectaculos.Application.Abstractions.Repositories
{
    public interface IBeneficioRepository : IRepository<Beneficio, Guid>
    {
        Task<IReadOnlyList<Beneficio>> ListVigentesAsync(DateTime onDateUtc, CancellationToken ct = default);
        Task<IReadOnlyList<Beneficio>> SearchByNombreAsync(string term, CancellationToken ct = default);
    }
}