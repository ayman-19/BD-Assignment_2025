using System.Collections.Concurrent;
using BD_Assignment_2025.Entities;
using BD_Assignment_2025.IRepositories;

namespace BD_Assignment_2025.Repositories
{
    public sealed class BlockedCountryRepository
        : Repository<string, BlockCountry>,
            IBlockedCountryRepository
    {
        private readonly ConcurrentDictionary<string, BlockCountry> _blockedCountries;

        public BlockedCountryRepository(ConcurrentDictionary<string, BlockCountry> blockedCountries)
            : base(blockedCountries) => _blockedCountries = blockedCountries;

        public async ValueTask RemoveExpiredBlocks()
        {
            var keys = _blockedCountries
                .Where(bc => bc.Value.Duration != null && bc.Value.Duration < DateTime.UtcNow)
                .Select(bc => bc.Key)
                .ToList();
            Parallel.ForEach(
                keys,
                key =>
                {
                    _blockedCountries.Remove(key, out _);
                }
            );
            await Task.CompletedTask;
        }
    }
}
