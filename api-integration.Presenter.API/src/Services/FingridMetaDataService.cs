using api_integration.Application.src.DTOs;
using api_integration.Application.src.Interfaces;
using api_integration.Application.src.Mappers;
using api_integration.Domain.src.Interfaces.Repositories;
using api_integration.SharedKernel.src;

namespace api_integration.Presenter.API.src.Services
{
    public class FingridMetaDataService : IFingridMetaDataService
    {
        private readonly IFingridService _fingridService;
        private readonly IMetaDataRepository _metaDataRepository;
        private readonly ILogger<FingridMetaDataService> _logger;

        public FingridMetaDataService(
            IFingridService fingridService,
            IMetaDataRepository metaDataRepository,
            ILogger<FingridMetaDataService> logger)
        {
            _fingridService = fingridService ?? throw new ArgumentNullException(nameof(fingridService));
            _metaDataRepository = metaDataRepository ?? throw new ArgumentNullException(nameof(metaDataRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<MetaDataReadDto>> GetMetaDataAsync(int datasetId)
        {
            // Check cache first
            var cached = await _metaDataRepository.GetByDatasetIdAsync(datasetId);
            if (cached != null)
            {
                _logger.LogInformation("Cache hit for metadata dataset {datasetId}", datasetId);
                return cached.ToReadDto();
            }

            // Cache miss — fetch from external API
            _logger.LogInformation("Cache miss for metadata dataset {datasetId}, fetching from external API", datasetId);
            var result = await _fingridService.GetExternalMetaDataByIdAsync(datasetId);
            if (result.IsFailure) return result.Error;

            // Store in cache
            var entity = result.Value.ToInternalDto().ToEntity();
            await _metaDataRepository.AddAsync(entity);

            return entity.ToReadDto();
        }
    }
}