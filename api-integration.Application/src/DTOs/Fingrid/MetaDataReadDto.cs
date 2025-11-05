namespace api_integration.Application.src.DTOs.Fingrid
{
     public record MetaDataReadDto(
        Guid Id,
        int DatasetId,
        DateTime ModifiedAtUtc,
        string Type,
        string Status,
        string Organization,
        string NameEn,
        string DescriptionEn,
        string DataPeriodEn,
        string? UpdateCadenceEn,
        string UnitEn,
        string ContactPersons,
        string LicenseName,
        string LicenseTermsLink,
        List<string> KeyWordsEn,
        List<string> ContentGroupsEn,
        List<string> AvailableFormats,
        DateTime? DataAvailableFromUtc
    );
}