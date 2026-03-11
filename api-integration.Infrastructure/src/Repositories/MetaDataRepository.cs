using api_integration.Domain.src.Entities.Fingrid;
using api_integration.Domain.src.Interfaces.Repositories;
using api_integration.Infrastructure.src.Data;
using Microsoft.EntityFrameworkCore;

namespace api_integration.Infrastructure.src.Repositories
{
    public class MetaDataRepository : IMetaDataRepository
    {
        private readonly AppDbContext _context;

        public MetaDataRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MetaData?> GetByDatasetIdAsync(int datasetId) =>
            await _context.MetaDataCache.FirstOrDefaultAsync(m => m.DatasetId == datasetId);

        public async Task AddAsync(MetaData metaData)
        {
            await _context.MetaDataCache.AddAsync(metaData);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MetaData metaData)
        {
            _context.MetaDataCache.Update(metaData);
            await _context.SaveChangesAsync();
        }
    }
}