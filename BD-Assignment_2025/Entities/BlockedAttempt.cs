namespace BD_Assignment_2025.Entities
{
    public record BlockedAttempt(
        string IP,
        string CountryCode,
        bool IsBlocked,
        string UserAgent,
        DateTime Timestamp = default
    );
}
