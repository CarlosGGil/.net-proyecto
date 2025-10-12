using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Application.Abstractions.Repositories;
using Espectaculos.Domain.Entities;
using Espectaculos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Espectaculos.Infrastructure.Repositories;

public sealed class OrdenRepository : IOrdenRepository
{
    private readonly EspectaculosDbContext _db;

    public OrdenRepository(EspectaculosDbContext db)
    {
        _db = db;
    }

    public async Task<Orden?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Ordenes
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task AddAsync(Orden orden, CancellationToken ct = default)
        => await _db.Ordenes.AddAsync(orden, ct);

    public void Update(Orden orden)
        => _db.Ordenes.Update(orden);

    public async Task<List<Orden>> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _db.Ordenes
            .AsNoTracking()
            .Include(o => o.Items)
            .Where(o => o.EmailComprador == email)
            .OrderByDescending(o => o.Fecha)
            .ToListAsync(ct);
}
