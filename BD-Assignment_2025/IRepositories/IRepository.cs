using System.Linq.Expressions;

namespace BD_Assignment_2025.IRepositories
{
    public interface IRepository<TKey, TElement>
        where TElement : class
    {
        ValueTask<TElement> CreateAsync(
            TKey key,
            TElement element,
            CancellationToken cancellationToken = default
        );
        ValueTask<bool> DeleteAsync(TKey key, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<TSelctor>> PaginateAsync<TSelctor>(
            Func<KeyValuePair<TKey, TElement>, TSelctor> Selctor,
            int page = 0,
            int pageSize = 0,
            Func<KeyValuePair<TKey, TElement>, bool> predicate = null!,
            CancellationToken cancellationToken = default
        );
        ValueTask<bool> IsAnyExist(Func<KeyValuePair<TKey, TElement>, bool> predicate);
        ValueTask<int> CountAsync();
        ValueTask<TElement> UpdateAsync(
            TKey key,
            TElement element,
            CancellationToken cancellationToken = default
        );
    }
}
