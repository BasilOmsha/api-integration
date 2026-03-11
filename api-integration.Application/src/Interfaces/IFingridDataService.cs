using api_integration.Application.src.DTOs;
using api_integration.SharedKernel.src;

namespace api_integration.Application.src.Interfaces
{
    public interface IFingridDataService
    {
        Task<Result<List<CachedDataPointReadDto>>> GetDataAsync(int datasetId, DateTime startTime, DateTime endTime);
    }
}