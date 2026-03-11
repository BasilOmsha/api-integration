using api_integration.Application.src.DTOs;
using api_integration.Domain.src.Entities.Fingrid;

namespace api_integration.Application.src.Mappers
{
    public static class CachedDataPointMapper
    {
        public static CachedDataPointCreateDto ToInternalDto(this DataPointDto external)
        {
            return new CachedDataPointCreateDto
            {
                DatasetId = external.DatasetId,
                StartTime = external.StartTime,
                EndTime = external.EndTime,
                Value = external.Value
            };
        }

        public static CachedDataPointReadDto ToReadDto(this CachedDataPoint entity)
        {
            return new CachedDataPointReadDto
            {
                Id = entity.Id,
                DatasetId = entity.DatasetId,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Value = entity.Value,
                CachedAtUtc = entity.CachedAtUtc
            };
        }
    }
}