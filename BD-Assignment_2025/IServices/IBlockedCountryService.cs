using System.Diagnostics.Metrics;
using BD_Assignment_2025.Entities;
using BD_Assignment_2025.Responses;
using BD_Assignment_2025.Result;

namespace BD_Assignment_2025.IServices
{
    public interface IBlockedCountryService
    {
        ValueTask<ResponseOf<CountryInfo>> GetCountryInfoByIPAsync(
            string ip = null!,
            CancellationToken cancellationToken = default
        );

        ValueTask<ResponseOf<BlockCountry>> AddBlockCountryAsync(
            BlockCountry country,
            CancellationToken cancellationToken = default
        );
        public ValueTask<Response> DeleteBlockedCountry(
            string countryCode,
            CancellationToken cancellationToken = default
        );
        public ValueTask<ResponseOf<BlockCountry>> TemporarilyBlockCountryAsync(
            string countryCode,
            double durationMinutes,
            CancellationToken cancellationToken = default
        );

        ValueTask<ResponseOf<GetAllBlockedCountryResult>> GetAllBlockedCountries(
            int page = 1,
            int pageSize = 10,
            string? search = null,
            CancellationToken cancellationToken = default
        );

        ValueTask<Response> VerifyIPIsBlocked(string ip = null!);
        ValueTask<ResponseOf<GetAllLogsCountryResult>> LogFailedBlockedAttempts(
            int page = 1,
            int pageSize = 10
        );
    }
}
