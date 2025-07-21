using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class BaseRepository<T>(ApplicationDbContext context) : IBaseRepository<T>, IBaseQuery<T> where T : class
{
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T> Create(T entity, CancellationToken cancellation)
    {
        await _dbSet.AddAsync(entity, cancellation);

        await context.SaveChangesAsync(cancellation);

        var entry = context.Entry(entity);

        foreach (var navigation in entry.Navigations)
            if (!navigation.IsLoaded)
                await navigation.LoadAsync(cancellation);

        return entity;
    }

    public async Task<IEnumerable<T>> CreateRange(IEnumerable<T> entities, CancellationToken cancellation)
    {
        await _dbSet.AddRangeAsync(entities, cancellation);

        await context.SaveChangesAsync(cancellation);

        return entities;
    }

    public async Task<T> Delete(T entity, CancellationToken cancellation)
    {
        _dbSet.Remove(entity);

        await context.SaveChangesAsync(cancellation);

        return entity;
    }

    public async Task<IEnumerable<T>> DeleteRange(IEnumerable<T> entities, CancellationToken cancellation)
    {
        _dbSet.RemoveRange(entities);

        await context.SaveChangesAsync(cancellation);

        return entities;
    }

    public async Task<Option<T>> Get(
        CancellationToken cancellation,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        IQueryable<T> query = _dbSet.AsQueryable()
                                    .AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        if (include != null)
            query = include(query);

        T? entity = await query.FirstOrDefaultAsync(cancellation);

        return entity is null ? Option.None<T>() : Option.Some(entity);
    }

    public async Task<IEnumerable<T>> GetMany(
        CancellationToken cancellation,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        int? skip = null,
        int? take = null)
    {
        IQueryable<T> query = _dbSet.AsQueryable().AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        if (include != null)
            query = include(query);

        if (skip != null)
            query = query.Skip(skip.Value);

        if (take != null)
            query = query.Take(take.Value);

        if (orderBy != null)
            query = orderBy(query);

        return await query.ToListAsync(cancellation);
    }

    public async Task<T> Update(T entity, CancellationToken cancellation)
    {
        _dbSet.Update(entity);

        await context.SaveChangesAsync(cancellation);

        var entry = context.Entry(entity);

        foreach (var navigation in entry.Navigations)
            if (!navigation.IsLoaded)
                await navigation.LoadAsync(cancellation);

        return entity;
    }

    public async Task<IEnumerable<T>> UpdateRange(IEnumerable<T> entities, CancellationToken cancellation)
    {
        _dbSet.UpdateRange(entities);

        await context.SaveChangesAsync(cancellation);

        return entities;
    }
}
