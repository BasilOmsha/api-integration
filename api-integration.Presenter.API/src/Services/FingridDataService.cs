using api_integration.Application.src.DTOs;
using api_integration.Application.src.Interfaces;
using api_integration.Application.src.Mappers;
using api_integration.Domain.src.Entities.Fingrid;
using api_integration.Domain.src.Interfaces.Repositories;
using api_integration.SharedKernel.src;

namespace api_integration.Presenter.API.src.Services
{
    public class FingridDataService : IFingridDataService
    {
        private readonly IFingridService _fingridService;
        private readonly ICachedDataPointRepository _dataPointRepository;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<FingridDataService> _logger;

        public FingridDataService(
            IFingridService fingridService,
            ICachedDataPointRepository dataPointRepository,
            IServiceScopeFactory scopeFactory,
            ILogger<FingridDataService> logger)
        {
            _fingridService = fingridService ?? throw new ArgumentNullException(nameof(fingridService));
            _dataPointRepository = dataPointRepository ?? throw new ArgumentNullException(nameof(dataPointRepository));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<List<CachedDataPointReadDto>>> GetDataAsync(int datasetId, DateTime startTime, DateTime endTime)
        {
            var cached = await _dataPointRepository.GetByDatasetIdAndRangeAsync(datasetId, startTime, endTime);
            
            if (cached.Count > 0)
            {
                var firstCached = cached.First();
                var lastCached = cached.Last();
                
                // Only use cache if it covers the entire requested range
                if (firstCached.StartTime <= startTime && lastCached.EndTime >= endTime)
                {
                    _logger.LogInformation("Cache hit for dataset {datasetId}", datasetId);
                    return cached.Select(d => d.ToReadDto()).ToList();
                }
                
                _logger.LogInformation("Partial cache hit for dataset {datasetId}, fetching full range from external API", datasetId);
            }

            // 2. Cache miss or partial hit — fetch from Fingrid external API
            _logger.LogInformation("Cache miss for dataset {datasetId}, fetching from external API", datasetId);
            var result = await _fingridService.GetDataByDatasetIdAsync(datasetId, startTime, endTime);
            if (result.IsFailure) return result.Error;

            // 3. Deduplicate by StartTime (API pagination may return overlaps)
            List<CachedDataPoint> entities = [.. result.Value.Data
                .GroupBy(d => d.StartTime)
                .Select(g => g.Last())
                .Select(d => d.ToInternalDto().ToEntity())
                .OrderBy(d => d.StartTime)];

            if (entities.Count > 0)
            {
                // Fire-and-forget: save to cache in the background so the client
                // gets the response immediately without waiting for the DB write.
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var repo = scope.ServiceProvider.GetRequiredService<ICachedDataPointRepository>();
                        await repo.UpsertRangeAsync(datasetId, entities);
                        _logger.LogInformation("Background cache write completed for dataset {datasetId} ({count} rows)",
                            datasetId, entities.Count);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Background cache write failed for dataset {datasetId}", datasetId);
                    }
                });
            }
            else
            {
                _logger.LogWarning("No data points returned from external API for dataset {datasetId} between {start} and {end}", datasetId, startTime, endTime);
            }

            return entities.Select(d => d.ToReadDto()).ToList();
        }
    }
}