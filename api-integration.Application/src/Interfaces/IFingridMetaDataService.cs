using api_integration.Application.src.DTOs;
using api_integration.SharedKernel.src;

namespace api_integration.Application.src.Interfaces
{
    public interface IFingridMetaDataService
    {
        Task<Result<MetaDataReadDto>> GetMetaDataAsync(int datasetId);
    }
}