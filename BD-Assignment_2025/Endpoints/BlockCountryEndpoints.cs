using BD_Assignment_2025.Entities;
using BD_Assignment_2025.IEndpoints;
using BD_Assignment_2025.IServices;

namespace BD_Assignment_2025.Endpoints
{
    public class BlockCountryEndpoints : IEndpoint
    {
        public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            RouteGroupBuilder group = endpoints.MapGroup("/api/countries").WithTags("Country");

            group.MapGet(
                "GetCountryInfoByIPAsync/{ip}",
                async (
                    string ip,
                    IBlockedCountryService _blockedCountryService,
                    CancellationToken cancellationToken
                ) =>
                    Results.Ok(
                        await _blockedCountryService.GetCountryInfoByIPAsync(ip, cancellationToken)
                    )
            );

            group.MapPost(
                "AddBlockedCountry/{countryCode}",
                async (
                    string countryCode,
                    IBlockedCountryService _blockedCountryService,
                    CancellationToken cancellationToken
                ) =>
                    Results.Ok(
                        await _blockedCountryService.AddBlockCountryAsync(
                            new BlockCountry(countryCode, null),
                            cancellationToken
                        )
                    )
            );
            group.MapGet(
                "GetAllBlockedCountries/{page}/{pageSize}/{search}",
                async (
                    int page,
                    int pageSize,
                    string search,
                    IBlockedCountryService _blockedCountryService,
                    CancellationToken cancellationToken
                ) =>
                    Results.Ok(
                        await _blockedCountryService.GetAllBlockedCountries(
                            page,
                            pageSize,
                            search,
                            cancellationToken
                        )
                    )
            );
            group.MapDelete(
                "DeleteBlockedCountry/{countryCode}",
                async (
                    string countryCode,
                    IBlockedCountryService _blockedCountryService,
                    CancellationToken cancellationToken
                ) =>
                    Results.Ok(
                        await _blockedCountryService.DeleteBlockedCountry(
                            countryCode,
                            cancellationToken
                        )
                    )
            );

            group.MapPut(
                "TemporarilyBlockCountryAsync/{countryCode}/{durationMinutes}",
                async (
                    string countryCode,
                    double durationMinutes,
                    IBlockedCountryService _blockedCountryService,
                    CancellationToken cancellationToken
                ) =>
                    Results.Ok(
                        await _blockedCountryService.TemporarilyBlockCountryAsync(
                            countryCode,
                            durationMinutes,
                            cancellationToken
                        )
                    )
            );

            group.MapGet(
                "VerifyIPIsBlockedAsync/{ip}",
                async (string ip, IBlockedCountryService _blockedCountryService) =>
                    Results.Ok(await _blockedCountryService.VerifyIPIsBlocked(ip))
            );

            group.MapGet(
                "LogFailedBlockedAttemptsAsync/{page}/{pageSize}",
                async (
                    int page,
                    int pageSize,
                    IBlockedCountryService _blockedCountryService,
                    CancellationToken cancellationToken
                ) =>
                    Results.Ok(
                        await _blockedCountryService.LogFailedBlockedAttempts(page, pageSize)
                    )
            );
        }
    }
}
