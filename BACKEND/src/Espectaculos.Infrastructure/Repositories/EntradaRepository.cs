using Espectaculos.Application.Abstractions.Repositories;
using Espectaculos.Domain.Entities;
using Espectaculos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Espectaculos.Infrastructure.Repositories;

public class EntradaRepository : IEntradaRepository
{
    private readonly EspectaculosDbContext _db;

    public EntradaRepository(EspectaculosDbContext db) => _db = db;

    public async Task<List<Entrada>> GetByEventoIdAsync(Guid eventoId, CancellationToken ct = default)
        => await _db.Entradas.AsNoTracking().Where(e => e.EventoId == eventoId).ToListAsync(ct);

    public async Task<Entrada?> GetByEventoAndTipoAsync(Guid eventoId, string tipo, CancellationToken ct = default)
        => await _db.Entradas.FirstOrDefaultAsync(e => e.EventoId == eventoId && e.Tipo == tipo, ct);

    public void Update(Entrada entity) => _db.Entradas.Update(entity);
}
