using System.ComponentModel.DataAnnotations;

namespace api_integration.Application.src.DTOs.Fingrid
{
    public record MetaDataUpdateDto(
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "DatasetId must be greater than 0")]
        int DatasetId,
        
        [Required]
        DateTime ModifiedAtUtc,
        
        [Required]
        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        string Type,
        
        [Required]
        [StringLength(20)]
        string Status,
        
        [Required]
        [StringLength(100)]
        string Organization,
        
        [Required]
        [StringLength(200)]
        string NameEn,
        
        [Required]
        [StringLength(1000)]
        string DescriptionEn,
        
        [Required]
        [StringLength(100)]
        string DataPeriodEn,
        
        [StringLength(100)]
        string? UpdateCadenceEn,
        
        [Required]
        [StringLength(50)]
        string UnitEn,
        
        [Required]
        [StringLength(500)]
        string ContactPersons,
        
        [Required]
        [StringLength(100)]
        string LicenseName,
        
        [Required]
        [Url]
        string LicenseTermsLink,
        
        [Required]
        [MinLength(1)]
        List<string> KeyWordsEn,
        
        [Required]
        [MinLength(1)]
        List<string> ContentGroupsEn,
        
        [Required]
        [MinLength(1)]
        List<string> AvailableFormats,
        
        DateTime? DataAvailableFromUtc
    );
}