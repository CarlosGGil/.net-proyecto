using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Application.Abstractions;
using Espectaculos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Espectaculos.Infrastructure.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly EspectaculosDbContext _db;

    public AvailabilityService(EspectaculosDbContext db)
    {
        _db = db;
    }

    public async Task RecomputeForEventAsync(Guid eventoId, CancellationToken cancellationToken = default)
    {
        var evento = await _db.Eventos
            .Include(e => e.Entradas)
            .FirstOrDefaultAsync(e => e.Id == eventoId, cancellationToken);

        if (evento is null) return;

        var stock = evento.Entradas.Sum(x => x.StockDisponible);
        var now = DateTime.UtcNow;
        var nuevoDisponible = evento.Fecha > now && stock > 0;

        if (evento.Disponible != nuevoDisponible)
        {
            evento.Disponible = nuevoDisponible;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> RecomputeAllAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // Stock por evento (AsNoTracking y agrupado para evitar N+1)
        var stockPorEvento = await _db.Entradas
            .AsNoTracking()
            .GroupBy(e => e.EventoId)
            .Select(g => new { EventoId = g.Key, Stock = g.Sum(x => x.StockDisponible) })
            .ToDictionaryAsync(x => x.EventoId, x => x.Stock, cancellationToken);

        var eventos = await _db.Eventos.ToListAsync(cancellationToken);

        int updates = 0;
        foreach (var e in eventos)
        {
            var stock = stockPorEvento.TryGetValue(e.Id, out var s) ? s : 0;
            var nuevo = e.Fecha > now && stock > 0;
            if (e.Disponible != nuevo)
            {
                e.Disponible = nuevo;
                updates++;
            }
        }

        if (updates > 0)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

        return updates;
    }
}
