using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Domain.Entities;

namespace Espectaculos.Application.Abstractions.Repositories;

public interface IOrdenRepository
{
    Task<Orden?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Orden orden, CancellationToken ct = default);
    void Update(Orden orden);
    Task<List<Orden>> GetByEmailAsync(string email, CancellationToken ct = default);
}
