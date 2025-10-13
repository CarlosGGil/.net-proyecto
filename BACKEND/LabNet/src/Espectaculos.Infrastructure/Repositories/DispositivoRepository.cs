using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Domain.Entities;
using Espectaculos.Application.Abstractions.Repositories;
using Espectaculos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Espectaculos.Infrastructure.Repositories
{
    public class DispositivoRepository : BaseEfRepository<Dispositivo, Guid>, IDispositivoRepository
    {
        public DispositivoRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<IReadOnlyList<Dispositivo>> ListByUsuarioAsync(Guid usuarioId, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(d => d.UsuarioId == usuarioId).ToListAsync(ct);

        public async Task<IReadOnlyList<Dispositivo>> ListActivosByUsuarioAsync(Guid usuarioId, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(d => d.UsuarioId == usuarioId && d.Estado != 0).ToListAsync(ct); // ajusta condiciÃ³n de "activo"
    }
}