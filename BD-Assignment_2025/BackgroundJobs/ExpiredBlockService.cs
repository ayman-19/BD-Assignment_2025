using BD_Assignment_2025.IRepositories;

namespace BD_Assignment_2025.BackgroundJobs
{
    public class ExpiredBlockService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ExpiredBlockService(IServiceScopeFactory scopeFactory) =>
            _scopeFactory = scopeFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var repository =
                        scope.ServiceProvider.GetRequiredService<IBlockedCountryRepository>();
                    await repository.RemoveExpiredBlocks();
                }
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
