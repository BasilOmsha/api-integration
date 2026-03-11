using api_integration.Application.src.Interfaces.IDtos;
using api_integration.Domain.src.Entities.Fingrid;

namespace api_integration.Application.src.DTOs
{
    public class CachedDataPointCreateDto : IBaseCreateDto<CachedDataPoint>
    {
        public int DatasetId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Value { get; set; }

        public CachedDataPoint ToEntity() => new(DatasetId, StartTime, EndTime, Value);
    }
}