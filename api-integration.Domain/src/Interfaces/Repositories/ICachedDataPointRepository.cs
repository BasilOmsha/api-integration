using api_integration.Domain.src.Entities.Fingrid;

namespace api_integration.Domain.src.Interfaces.Repositories
{
    public interface ICachedDataPointRepository
    {
        Task<List<CachedDataPoint>> GetByDatasetIdAndRangeAsync(int datasetId, DateTime startTime, DateTime endTime);
        Task UpsertRangeAsync(int datasetId, List<CachedDataPoint> dataPoints);
        Task<int> DeleteOlderThanAsync(DateTime cutoff);
    }
}