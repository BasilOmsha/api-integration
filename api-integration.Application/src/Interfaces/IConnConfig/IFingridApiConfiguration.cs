namespace api_integration.Application.src.Interfaces.IConnConfig
{
    public interface IFingridApiConfiguration
    {
        string? BaseUrl { get; }
        string? ApiKey { get; }
        int TimeoutSeconds { get; }
        int MaxRetryAttempts { get; }
        int RetryDelayMs { get; }
    }
}