using System.Text.Json.Serialization;

namespace api_integration.Application.src.DTOs.Fingrid
{
    public record MetaDataApiResponseDto(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("modifiedAtUtc")] DateTime ModifiedAtUtc,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("organization")] string Organization,
        [property: JsonPropertyName("nameEn")] string NameEn,
        [property: JsonPropertyName("descriptionEn")] string DescriptionEn,
        [property: JsonPropertyName("dataPeriodEn")] string DataPeriodEn,
        [property: JsonPropertyName("updateCadenceEn")] string? UpdateCadenceEn,
        [property: JsonPropertyName("unitEn")] string UnitEn,
        [property: JsonPropertyName("contactPersons")] string ContactPersons,
        [property: JsonPropertyName("license")] FingridLicense License,
        [property: JsonPropertyName("keyWordsEn")] List<string> KeyWordsEn,
        [property: JsonPropertyName("contentGroupsEn")] List<string> ContentGroupsEn,
        [property: JsonPropertyName("availableFormats")] List<string> AvailableFormats,
        [property: JsonPropertyName("dataAvailableFromUtc")] DateTime? DataAvailableFromUtc
    );

    public record FingridLicense(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("termsLink")] string TermsLink
    );
}