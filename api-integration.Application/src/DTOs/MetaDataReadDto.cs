using api_integration.Application.src.Interfaces.IDtos;
using api_integration.Domain.src.Entities;
using api_integration.Domain.src.ValueObjects;

namespace api_integration.Application.src.DTOs
{
    public class MetaDataReadDto : IBaseReadDto<MetaData>
    {
        public Guid Id { get; set; }
        public int DatasetId { get; set; }
        public DateTime ModifiedAtUtc { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string DataPeriodEn { get; set; } = string.Empty;
        public string? UpdateCadenceEn { get; set; }
        public string UnitEn { get; set; } = string.Empty;
        public string ContactPersons { get; set; } = string.Empty;
        public License? License { get; set; }
        public List<string> KeyWordsEn { get; set; } = [];
        public List<string> ContentGroupsEn { get; set; } = [];
        public List<string> AvailableFormats { get; set; } = [];
        public DateTime? DataAvailableFromUtc { get; set; }
    }
}