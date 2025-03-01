using System.Collections.Concurrent;
using BD_Assignment_2025.IRepositories;

namespace BD_Assignment_2025.Repositories
{
    public class Repository<TKey, TElement> : IRepository<TKey, TElement>
        where TElement : class
    {
        private readonly ConcurrentDictionary<TKey, TElement> _blockedCountries;

        public Repository(ConcurrentDictionary<TKey, TElement> blockedCountries) =>
            _blockedCountries = blockedCountries;

        public async ValueTask<int> CountAsync() => await Task.Run(() => _blockedCountries.Count);

        public async ValueTask<TElement> CreateAsync(
            TKey key,
            TElement element,
            CancellationToken cancellationToken = default
        ) => _blockedCountries.TryAdd(key, element) ? await Task.Run(() => element) : default!;

        public async ValueTask<bool> DeleteAsync(
            TKey key,
            CancellationToken cancellationToken = default
        )
        {
            var result = _blockedCountries.TryRemove(key, out _);
            return await Task.FromResult(result);
        }

        public async ValueTask<bool> IsAnyExist(
            Func<KeyValuePair<TKey, TElement>, bool> predicate
        ) => await Task.Run(() => _blockedCountries.Any(predicate));

        public async Task<IReadOnlyCollection<TSelctor>> PaginateAsync<TSelctor>(
            Func<KeyValuePair<TKey, TElement>, TSelctor> Selctor,
            int page = 0,
            int pageSize = 0,
            Func<KeyValuePair<TKey, TElement>, bool> predicate = null!,
            CancellationToken cancellationToken = default
        )
        {
            var src = _blockedCountries.AsQueryable();
            if (predicate is not null)
                src = src.Where(predicate).AsQueryable();
            return await Task.Run(
                () => src.Skip((page - 1) * pageSize).Take(pageSize).Select(Selctor).ToList(),
                cancellationToken
            );
        }

        public async ValueTask<TElement> UpdateAsync(
            TKey key,
            TElement element,
            CancellationToken cancellationToken = default
        ) => await Task.Run(() => _blockedCountries[key] = element);
    }
}
