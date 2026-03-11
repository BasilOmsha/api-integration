using api_integration.Domain.src.Entities.Fingrid;

namespace api_integration.Domain.src.Interfaces.Repositories
{
    public interface IMetaDataRepository
    {
        Task<MetaData?> GetByDatasetIdAsync(int datasetId);
        Task AddAsync(MetaData metaData);
        Task UpdateAsync(MetaData metaData);
    }
}