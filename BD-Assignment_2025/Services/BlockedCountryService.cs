using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using BD_Assignment_2025.Entities;
using BD_Assignment_2025.Exceptions;
using BD_Assignment_2025.IRepositories;
using BD_Assignment_2025.IServices;
using BD_Assignment_2025.Responses;
using BD_Assignment_2025.Result;

namespace BD_Assignment_2025.Services
{
    public sealed record BlockedCountryService : IBlockedCountryService
    {
        private readonly IBlockedCountryRepository _blockedCountryRepository;
        private readonly IHttpClientFactory _factory;
        private readonly IHttpContextAccessor _accessor;
        private readonly IConfiguration _configuration;
        private readonly ConcurrentBag<BlockedAttempt> _logs;

        public BlockedCountryService(
            IBlockedCountryRepository blockedCountryRepository,
            IHttpClientFactory factory,
            IHttpContextAccessor accessor,
            IConfiguration configuration,
            ConcurrentBag<BlockedAttempt> logs
        )
        {
            _blockedCountryRepository = blockedCountryRepository;
            _factory = factory;
            _accessor = accessor;
            _configuration = configuration;
            _logs = logs;
        }

        public async ValueTask<ResponseOf<BlockCountry>> AddBlockCountryAsync(
            BlockCountry country,
            CancellationToken cancellationToken = default
        )
        {
            if (!IsValidCountryCode(country.Code))
                throw new IPNotValidException("Invalid Country Codes.");

            if (
                await _blockedCountryRepository.IsAnyExist(c =>
                    c.Value.Code.ToLower().Equals(country.Code.ToLower())
                )
            )
                throw new ValidationException("Country Code is Exist.");
            BlockCountry countryInfo = await _blockedCountryRepository.CreateAsync(
                country.Code,
                country,
                cancellationToken
            );
            return new()
            {
                Message = "Success.",
                StatusCode = (int)HttpStatusCode.OK,
                Success = true,
                Result = countryInfo!,
            };
        }

        public async ValueTask<Response> DeleteBlockedCountry(
            string countryCode,
            CancellationToken cancellationToken = default
        )
        {
            if (
                !await _blockedCountryRepository.IsAnyExist(c =>
                    c.Value.Code.ToLower().Equals(countryCode.ToLower())
                )
            )
                throw new ValidationException("Country Code is Not Exist.");
            var result = await _blockedCountryRepository.DeleteAsync(countryCode);
            return new Response
            {
                Message = result ? "Success." : "Not Found.",
                StatusCode = result ? (int)HttpStatusCode.OK : (int)HttpStatusCode.NotFound,
                Success = result,
            };
        }

        public async ValueTask<ResponseOf<GetAllBlockedCountryResult>> GetAllBlockedCountries(
            int page = 1,
            int pageSize = 10,
            string? search = null,
            CancellationToken cancellationToken = default
        )
        {
            int Page = page <= 0 ? 1 : page;
            int PageSize = pageSize <= 0 ? 5 : pageSize;

            var result = await _blockedCountryRepository.PaginateAsync(
                bc => new BlockCountry(bc.Value.Code, bc.Value.Duration),
                Page,
                PageSize,
                bc =>
                    bc.Value.Code.ToLower()
                        .Contains(
                            string.IsNullOrWhiteSpace(search!.ToLower()) ? "" : search!.ToLower()
                        ),
                cancellationToken
            );
            var TotalCountry = await _blockedCountryRepository.CountAsync();
            return new()
            {
                Message = "Success.",
                StatusCode = (int)HttpStatusCode.OK,
                Success = true,
                Result = new GetAllBlockedCountryResult(
                    Page,
                    PageSize,
                    TotalCountry,
                    (int)Math.Ceiling(TotalCountry / (double)PageSize),
                    result
                ),
            };
        }

        public async ValueTask<ResponseOf<CountryInfo>> GetCountryInfoByIPAsync(
            string ip = null!,
            CancellationToken cancellationToken = default
        )
        {
            return new()
            {
                Message = "Success.",
                StatusCode = (int)HttpStatusCode.OK,
                Success = true,
                Result = await GetCountryInfo(ip),
            };
        }

        public async ValueTask<ResponseOf<BlockCountry>> TemporarilyBlockCountryAsync(
            string countryCode,
            double durationMinutes,
            CancellationToken cancellationToken = default
        )
        {
            if (durationMinutes < 1 || durationMinutes > 1440)
                throw new IPNotValidException("Duration must be between 1 and 1440 minutes.");

            if (!IsValidCountryCode(countryCode))
                throw new IPNotValidException("Invalid Country Codes.");

            if (
                await _blockedCountryRepository.IsAnyExist(c =>
                    c.Value.Code.ToLower().Equals(countryCode.ToLower())
                    && c.Value.Duration is not null
                )
            )
                throw new ValidationException("Conflict if already temporarily blocked.");

            var countryInfo = await _blockedCountryRepository.UpdateAsync(
                countryCode,
                new BlockCountry(countryCode, DateTime.UtcNow.AddMinutes(durationMinutes)),
                cancellationToken
            );
            return new()
            {
                Message = "Success.",
                StatusCode = (int)HttpStatusCode.OK,
                Success = true,
                Result = countryInfo!,
            };
        }

        private bool IsValidIp(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return false;

            const string ipv4Pattern =
                @"^(25[0-5]|2[0-4]\d|1\d{2}|\d{1,2})\.(25[0-5]|2[0-4]\d|1\d{2}|\d{1,2})\.(25[0-5]|2[0-4]\d|1\d{2}|\d{1,2})\.(25[0-5]|2[0-4]\d|1\d{2}|\d{1,2})$";
            const string ipv6Pattern =
                @"^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))$";

            return Regex.IsMatch(ip, ipv4Pattern) || Regex.IsMatch(ip, ipv6Pattern);
        }

        private bool IsValidCountryCode(string countryCode) =>
            new Regex("^[A-Z]{2}$").IsMatch(countryCode);

        public async ValueTask<Response> VerifyIPIsBlocked(string ip = null)
        {
            var countryInfo = await GetCountryInfo(ip);

            var isBlocked = await _blockedCountryRepository.IsAnyExist(b =>
                b.Key.ToUpper().Equals(countryInfo.Code.ToUpper())
            );
            _logs.Add(
                new BlockedAttempt(
                    countryInfo.IP,
                    countryInfo.Code,
                    isBlocked,
                    _accessor.HttpContext.Request.Headers["User-Agent"]
                )
            );
            return new()
            {
                Message = isBlocked ? "Access Blocked." : "Access allowed.",
                StatusCode = isBlocked ? StatusCodes.Status403Forbidden : StatusCodes.Status200OK,
                Success = true,
            };
        }

        public async ValueTask<ResponseOf<GetAllLogsCountryResult>> LogFailedBlockedAttempts(
            int page = 1,
            int pageSize = 10
        )
        {
            int Page = page <= 0 ? 1 : page;
            int PageSize = pageSize <= 0 ? 5 : pageSize;
            var result = _logs.Skip((page - 1) * pageSize).Take(PageSize);
            var TotalCountry = _logs.Count();
            return await Task.Run(
                () =>
                    new ResponseOf<GetAllLogsCountryResult>()
                    {
                        Message = "Success.",
                        StatusCode = (int)HttpStatusCode.OK,
                        Success = true,
                        Result = new GetAllLogsCountryResult(
                            Page,
                            PageSize,
                            TotalCountry,
                            (int)Math.Ceiling(TotalCountry / (double)PageSize),
                            result
                        ),
                    }
            );
        }

        private async Task<CountryInfo> GetCountryInfo(string ip)
        {
            var httpClient = _factory.CreateClient();

            if (string.IsNullOrWhiteSpace(ip))
                ip = _accessor.HttpContext!.Connection.RemoteIpAddress?.ToString() ?? "";

            if (ip == "::1")
                ip = await httpClient.GetStringAsync("https://api.ipify.org");

            if (!IsValidIp(ip))
                throw new IPNotValidException("IP Not Valid.");

            string url = _configuration["URL"] + $"apiKey={_configuration["ApiKey"]}&ip={ip}";

            var result = await httpClient.GetAsync(url);
            result.EnsureSuccessStatusCode();
            var response = await result.Content.ReadAsStringAsync();

            var countryInfo = JsonSerializer.Deserialize<CountryInfo>(
                response,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            return countryInfo!;
        }
    }
}
