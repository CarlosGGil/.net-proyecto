using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Domain.Entities;
using Espectaculos.Application.Abstractions.Repositories;

namespace Espectaculos.Application.Abstractions.Repositories
{
    public interface IDispositivoRepository : IRepository<Dispositivo, Guid>
    {
        Task<IReadOnlyList<Dispositivo>> ListByUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
        Task<IReadOnlyList<Dispositivo>> ListActivosByUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
    }
}