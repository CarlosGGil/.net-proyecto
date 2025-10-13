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
    public class UsuarioRepository : BaseEfRepository<Usuario, Guid>, IUsuarioRepository
    {
        public UsuarioRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
            => await _db.Set<Usuario>().AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

        public async Task<Usuario?> GetWithRolesAsync(Guid usuarioId, CancellationToken ct = default)
            => await _db.Set<Usuario>()
                        .AsNoTracking()
                        .Include(u => u.UsuarioRoles)
                            .ThenInclude(ur => ur.Rol)
                        .FirstOrDefaultAsync(u => u.UsuarioId == usuarioId, ct);

        public async Task<Usuario?> GetWithDispositivosAsync(Guid usuarioId, CancellationToken ct = default)
            => await _db.Set<Usuario>()
                        .AsNoTracking()
                        .Include(u => u.Dispositivos)
                        .FirstOrDefaultAsync(u => u.UsuarioId == usuarioId, ct);

        public async Task<(IReadOnlyList<Usuario> Items, int Total)> SearchAsync(string? term, int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            var q = _db.Set<Usuario>().AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(term))
                q = q.Where(u => u.Nombre.Contains(term) || u.Apellido.Contains(term) || u.Email.Contains(term) || u.Documento.Contains(term));
            var total = await q.CountAsync(ct);
            var items = await q.OrderBy(u => u.Apellido).ThenBy(u => u.Nombre)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync(ct);
            return (items, total);
        }
    }
}