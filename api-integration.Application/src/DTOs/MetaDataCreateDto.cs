using System.ComponentModel.DataAnnotations;
using api_integration.Application.src.Interfaces.IDtos;
using api_integration.Domain.src.Entities.Fingrid;
using api_integration.Domain.src.ValueObjects;

namespace api_integration.Application.src.DTOs
{
    public class MetaDataCreateDto : IBaseCreateDto<MetaData>
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "DatasetId must be greater than 0")]
        public int DatasetId { get; set; }

        [Required]
        public DateTime ModifiedAtUtc { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string Type { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Organization { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string NameEn { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string DescriptionEn { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DataPeriodEn { get; set; } = string.Empty;

        [StringLength(100)]
        public string? UpdateCadenceEn { get; set; }

        [Required]
        [StringLength(50)]
        public string UnitEn { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string ContactPersons { get; set; } = string.Empty;

        [Required]
        public License License { get; set; } = new("", "");

        [Required]
        [MinLength(1, ErrorMessage = "At least one keyword is required")]
        public List<string> KeyWordsEn { get; set; } = [];

        [Required]
        [MinLength(1, ErrorMessage = "At least one content group is required")]
        public List<string> ContentGroupsEn { get; set; } = [];

        [Required]
        [MinLength(1, ErrorMessage = "At least one available format is required")]
        public List<string> AvailableFormats { get; set; } = [];

        public DateTime? DataAvailableFromUtc { get; set; }

        public MetaData ToEntity()
        {
            return new MetaData(
                DatasetId,
                ModifiedAtUtc,
                Type,
                Status,
                Organization,
                NameEn,
                DescriptionEn,
                DataPeriodEn,
                UpdateCadenceEn,
                UnitEn,
                ContactPersons,
                License,
                KeyWordsEn,
                ContentGroupsEn,
                AvailableFormats,
                DataAvailableFromUtc
            );
        }
    }
}