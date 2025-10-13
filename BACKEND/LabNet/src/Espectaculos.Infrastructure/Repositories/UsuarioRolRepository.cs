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
    public class UsuarioRolRepository : BaseEfRepository<UsuarioRol, (Guid UsuarioId, Guid RolId)>, IUsuarioRolRepository
    {
        public UsuarioRolRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<UsuarioRol?> GetAsync(Guid usuarioId, Guid rolId, CancellationToken ct = default)
            => await _db.Set<UsuarioRol>().FindAsync([usuarioId, rolId], ct).AsTask();

        public async Task<bool> AssignAsync(Guid usuarioId, Guid rolId, DateTime? fechaAsignado = null, CancellationToken ct = default)
        {
            var existing = await _db.Set<UsuarioRol>().FindAsync([usuarioId, rolId], ct).AsTask();
            if (existing is not null) return false;
            await _db.Set<UsuarioRol>().AddAsync(new UsuarioRol
            {
                UsuarioId = usuarioId,
                RolId = rolId,
                FechaAsignado = fechaAsignado ?? DateTime.UtcNow
            }, ct);
            return true;
        }

        public async Task<bool> RemoveAsync(Guid usuarioId, Guid rolId, CancellationToken ct = default)
        {
            var existing = await _db.Set<UsuarioRol>().FindAsync([usuarioId, rolId], ct).AsTask();
            if (existing is null) return false;
            _db.Set<UsuarioRol>().Remove(existing);
            return true;
        }

        public async Task<IReadOnlyList<Rol>> GetRolesForUserAsync(Guid usuarioId, CancellationToken ct = default)
            => await _db.Set<UsuarioRol>().AsNoTracking()
                    .Where(ur => ur.UsuarioId == usuarioId)
                    .Select(ur => ur.Rol)
                    .ToListAsync(ct);

        public async Task<IReadOnlyList<Usuario>> GetUsersForRoleAsync(Guid rolId, CancellationToken ct = default)
            => await _db.Set<UsuarioRol>().AsNoTracking()
                    .Where(ur => ur.RolId == rolId)
                    .Select(ur => ur.Usuario)
                    .ToListAsync(ct);
    }
}