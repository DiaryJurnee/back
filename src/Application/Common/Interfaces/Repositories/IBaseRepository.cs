namespace Application.Common.Interfaces.Repositories;

public interface IBaseRepository<T>
{
    Task<T> Create(T entity, CancellationToken cancellation);
    Task<T> Update(T entity, CancellationToken cancellation);
    Task<T> Delete(T entity, CancellationToken cancellation);
    Task<IEnumerable<T>> CreateRange(IEnumerable<T> entities, CancellationToken cancellation);
    Task<IEnumerable<T>> UpdateRange(IEnumerable<T> entities, CancellationToken cancellation);
    Task<IEnumerable<T>> DeleteRange(IEnumerable<T> entities, CancellationToken cancellation);
}
