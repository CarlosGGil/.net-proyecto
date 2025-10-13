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
    public class BeneficioUsuarioRepository : BaseEfRepository<BeneficioUsuario, (Guid BeneficioId, Guid UsuarioId)>, IBeneficioUsuarioRepository
    {
        public BeneficioUsuarioRepository(EspectaculosDbContext db) : base(db) { }

        public async Task<IReadOnlyList<BeneficioUsuario>> ListByBeneficioAsync(Guid beneficioId, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(x => x.BeneficioId == beneficioId).ToListAsync(ct);

        public async Task<IReadOnlyList<BeneficioUsuario>> ListByUsuarioAsync(Guid usuarioId, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(x => x.UsuarioId == usuarioId).ToListAsync(ct);
    }
}