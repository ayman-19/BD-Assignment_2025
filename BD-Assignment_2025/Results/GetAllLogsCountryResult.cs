namespace BD_Assignment_2025.Result
{
	public sealed record GetAllLogsCountryResult(
        int Page,
        int PageSize,
        int TotalItems,
        int TotalPages,
        object Data
    );
}
