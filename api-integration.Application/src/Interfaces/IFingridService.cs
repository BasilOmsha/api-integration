using api_integration.Application.src.DTOs;
using api_integration.SharedKernel.src;

namespace api_integration.Application.src.Interfaces
{
    /// <summary>
    /// Service interface for interacting with the Fingrid external API
    /// </summary>
    public interface IFingridService
    {
          Task<Result<MetaDataExternalApiResDto>> GetExternalMetaDataByIdAsync(int id);
    }
}