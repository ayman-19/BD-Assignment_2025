namespace BD_Assignment_2025.BackgroundJobs;

public class ExpiredBlockJob(IServiceScopeFactory scopeFactory) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IBlockedCountryRepository>();
        await repository.RemoveExpiredBlocks();
    }
}
