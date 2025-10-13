using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Espectaculos.Application.Abstractions.Repositories;
using Espectaculos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Espectaculos.Infrastructure.Repositories
{
    public class BaseEfRepository<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : class
    {
        protected readonly EspectaculosDbContext _db;
        protected readonly DbSet<TEntity> _set;

        public BaseEfRepository(EspectaculosDbContext db)
        {
            _db = db;
            _set = _db.Set<TEntity>();
        }

        public IQueryable<TEntity> Query => _set.AsQueryable();

        public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default)
            => await _set.FindAsync([id], ct).AsTask();

        public virtual async Task<TEntity?> FindAsync(CancellationToken ct = default, params object[] keyValues)
            => await _set.FindAsync(keyValues, ct).AsTask();

        public virtual async Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken ct = default)
            => await _set.AsNoTracking().ToListAsync(ct);

        public virtual async Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
            => await _set.AsNoTracking().Where(predicate).ToListAsync(ct);

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
            => await _set.AsNoTracking().AnyAsync(predicate, ct);

        // 🔧 Implement the interface member your IDE is complaining about:
        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
            => await _set.AsNoTracking().AnyAsync(predicate, ct);

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
            => predicate is null ? await _set.CountAsync(ct) : await _set.CountAsync(predicate, ct);

        public virtual async Task<(IReadOnlyList<TEntity> Items, int Total)> PageAsync(
            int page = 1,
            int pageSize = 20,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            CancellationToken ct = default)
        {
            IQueryable<TEntity> q = _set.AsNoTracking();
            if (filter is not null) q = q.Where(filter);
            var total = await q.CountAsync(ct);
            if (orderBy is not null) q = orderBy(q);
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, total);
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default)
        {
            await _set.AddAsync(entity, ct);
            return entity;
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
            => await _set.AddRangeAsync(entities, ct);

        public virtual void Update(TEntity entity) => _set.Update(entity);
        public virtual void Remove(TEntity entity) => _set.Remove(entity);
        public virtual void RemoveRange(IEnumerable<TEntity> entities) => _set.RemoveRange(entities);
    }
}
