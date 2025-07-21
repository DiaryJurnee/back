using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IBaseQuery<T>
{
    Task<Option<T>> Get(CancellationToken cancellation,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
    Task<IEnumerable<T>> GetMany(CancellationToken cancellation,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        int? skip = null,
        int? take = null);
}
