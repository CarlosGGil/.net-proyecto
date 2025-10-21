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
    public class BeneficioRepository : BaseEfRepository<Beneficio, Guid>, IBeneficioRepository
    {
        public BeneficioRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<IReadOnlyList<Beneficio>> ListVigentesAsync(DateTime onDateUtc, CancellationToken ct = default)
            => await _set.AsNoTracking()
                         .Where(b => (!b.VigenciaInicio.HasValue || b.VigenciaInicio <= onDateUtc)
                                  && (!b.VigenciaFin.HasValue     || b.VigenciaFin   >= onDateUtc))
                         .ToListAsync(ct);

        public async Task<IReadOnlyList<Beneficio>> SearchByNombreAsync(string term, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(b => b.Nombre.Contains(term)).ToListAsync(ct);

        // Conteo de canjes que ya hizo un usuario para un beneficio
        public async Task<int> GetRedemptionsCountByUserAsync(Guid beneficioId, Guid usuarioId, CancellationToken ct = default)
            => await _db.Canjes.AsNoTracking()
                               .CountAsync(c => c.BeneficioId == beneficioId && c.UsuarioId == usuarioId, ct);

        // Intento atómico de consumir 1 unidad del cupo global.
        // Usa ExecuteUpdateAsync (EF Core) para hacer la resta en la base de datos de forma atómica.
        public async Task<bool> TryConsumeGlobalCupoAsync(Guid beneficioId, CancellationToken ct = default)
        {
            // Decrementa sólo si CupoTotal != null y > 0
            var updated = await _db.Beneficios
                                   .Where(b => b.BeneficioId == beneficioId && b.CupoTotal.HasValue && b.CupoTotal > 0)
                                   .ExecuteUpdateAsync(b => b.SetProperty(x => x.CupoTotal!, x => x.CupoTotal! - 1), ct);

            if (updated > 0) return true;

            // Si no se actualizó, puede ser porque CupoTotal es null (interpretemos null como "ilimitado")
            var existsWithInfinite = await _db.Beneficios
                                              .AsNoTracking()
                                              .AnyAsync(b => b.BeneficioId == beneficioId && !b.CupoTotal.HasValue, ct);
            return existsWithInfinite;
        }

        // Persistir registro de canje (auditoría)
        public async Task AddCanjeAsync(Canje canje, CancellationToken ct = default)
        {
            _db.Canjes.Add(canje);
            await _db.SaveChangesAsync(ct);
        }

        // Actualizar entidad Beneficio (si es necesario)
        public async Task UpdateAsync(Beneficio beneficio, CancellationToken ct = default)
        {
            _set.Update(beneficio);
            await _db.SaveChangesAsync(ct);
        }
    }
}