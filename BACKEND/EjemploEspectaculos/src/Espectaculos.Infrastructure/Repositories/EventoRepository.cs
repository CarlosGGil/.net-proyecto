using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Espectaculos.Application.Abstractions.Repositories;
using Espectaculos.Application.Common;
using Espectaculos.Application.DTOs;
using Espectaculos.Domain.Entities;
using Espectaculos.Infrastructure.Persistence;

namespace Espectaculos.Infrastructure.Repositories
{
    public sealed class EventoRepository : IEventoRepository
    {
        private readonly EspectaculosDbContext _db;

        public EventoRepository(EspectaculosDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResult<EventoDto>> SearchAsync(
            string? q,
            string? sort,
            string? dir,
            int page,
            int pageSize,
            bool onlyPublished,
            bool onlyDisponibles,
            CancellationToken cancellationToken = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _db.Eventos.AsNoTracking();

            // Eliminado filtro por onlyPublished (publicado) según nuevo requerimiento

            if (onlyDisponibles)
                query = query.Where(e =>
                    e.Fecha > DateTime.UtcNow &&
                    (e.Entradas.Sum(x => (int?)x.StockDisponible) ?? 0) > 0);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var qq = q.Trim();
                query = query.Where(e =>
                    EF.Functions.ILike(e.Titulo, $"%{qq}%") ||
                    EF.Functions.ILike(e.Descripcion, $"%{qq}%") ||
                    EF.Functions.ILike(e.Lugar, $"%{qq}%"));
            }

            var ascending = string.Equals(dir, "asc", StringComparison.OrdinalIgnoreCase);

            query = (sort?.ToLowerInvariant()) switch
            {
                "titulo" => ascending ? query.OrderBy(e => e.Titulo) : query.OrderByDescending(e => e.Titulo),
                "lugar"  => ascending ? query.OrderBy(e => e.Lugar)  : query.OrderByDescending(e => e.Lugar),
                "fecha"  => ascending ? query.OrderBy(e => e.Fecha)  : query.OrderByDescending(e => e.Fecha),
                _        => query.OrderByDescending(e => e.Fecha)
            };

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EventoDto(
                    e.Id,
                    e.Titulo,
                    e.Descripcion,
                    e.Fecha,
                    e.Lugar,
                    (e.Fecha > DateTime.UtcNow) && ((e.Entradas.Sum(x => (int?)x.StockDisponible) ?? 0) > 0)
                ))
                .ToListAsync(cancellationToken);

            return new PagedResult<EventoDto>(items, total, page, pageSize);
        }

        public async Task<Evento?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _db.Eventos.AsNoTracking().Include(e => e.Entradas).FirstOrDefaultAsync(e => e.Id == id, ct);

        public async Task AddAsync(Evento entity, CancellationToken ct = default)
            => await _db.Eventos.AddAsync(entity, ct);

        public void Update(Evento entity)
            => _db.Eventos.Update(entity);
    }
}
