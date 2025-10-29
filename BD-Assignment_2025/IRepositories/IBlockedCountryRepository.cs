namespace BD_Assignment_2025.IRepositories;

public interface IBlockedCountryRepository : IRepository<string, BlockCountry>
{
    ValueTask RemoveExpiredBlocks();
}
