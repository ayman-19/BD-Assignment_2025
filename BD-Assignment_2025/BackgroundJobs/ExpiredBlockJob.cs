namespace BD_Assignment_2025.BackgroundJobs;

public class ExpiredBlockJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ExpiredBlockJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IBlockedCountryRepository>();
        await repository.RemoveExpiredBlocks();
    }
}
