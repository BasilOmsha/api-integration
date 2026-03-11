using api_integration.Domain.src.Interfaces.Repositories;

namespace api_integration.Presenter.API.src.Services;

public class CacheCleanupBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CacheCleanupBackgroundService> _logger;

    private static readonly TimeSpan Interval = TimeSpan.FromHours(2);
    private static readonly TimeSpan RetentionPeriod = TimeSpan.FromHours(6);

    public CacheCleanupBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<CacheCleanupBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Cache cleanup service started. Interval: {Interval}, Retention: {Retention}",
            Interval, RetentionPeriod);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(Interval, stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<ICachedDataPointRepository>();

                var cutoff = DateTime.UtcNow - RetentionPeriod;
                var deletedCount = await repository.DeleteOlderThanAsync(cutoff);

                _logger.LogInformation("Cache cleanup completed. Deleted {Count} records older than {Cutoff:u}",
                    deletedCount, cutoff);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache cleanup failed. Will retry at next interval.");
            }
        }

        _logger.LogInformation("Cache cleanup service stopped.");
    }
}
