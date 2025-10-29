namespace BD_Assignment_2025.Endpoints;

public class BlockCountryEndpoints : IEndpoint
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/api/countries").WithTags("Country");

        group.MapPost(
            "Block",
            async (
                BlockCountry country,
                IBlockedCountryService _blockedCountryService,
                CancellationToken cancellationToken
            ) =>
                Results.Ok(
                    await _blockedCountryService.AddBlockCountryAsync(country, cancellationToken)
                )
        );

        group.MapDelete(
            "Block/{countryCode}",
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

        group.MapGet(
            "Blocked",
            async (
                [FromQuery] int? page,
                [FromQuery] int? pageSize,
                [FromQuery] string? search,
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

        group.MapGet(
            "ip/lookup",
            async (
                [FromQuery] string ipAddress,
                IBlockedCountryService _blockedCountryService,
                CancellationToken cancellationToken
            ) =>
                Results.Ok(
                    await _blockedCountryService.GetCountryInfoByIPAsync(
                        ipAddress,
                        cancellationToken
                    )
                )
        );

        group.MapGet(
            "ip/check-block",
            async (HttpContext context, IBlockedCountryService _blockedCountryService) =>
                Results.Ok(
                    await _blockedCountryService.VerifyIPIsBlocked(
                        context.Connection.RemoteIpAddress?.ToString() ?? string.Empty
                    )
                )
        );

        group.MapGet(
            "logs/blocked-attempts",
            async (
                [FromQuery] int? page,
                [FromQuery] int? pageSize,
                IBlockedCountryService _blockedCountryService,
                CancellationToken cancellationToken
            ) => Results.Ok(await _blockedCountryService.LogFailedBlockedAttempts(page, pageSize))
        );

        group.MapPost(
            "temporal-block",
            async (
                [FromQuery] string countryCode,
                [FromQuery] double durationMinutes,
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
    }
}
