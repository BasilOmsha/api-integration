using System.Text.Json.Serialization;

namespace api_integration.Application.src.DTOs
{
    public class MetaDataExternalApiResDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("modifiedAtUtc")]
        public DateTime ModifiedAtUtc { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("organization")]
        public string Organization { get; set; } = string.Empty;

        [JsonPropertyName("nameEn")]
        public string NameEn { get; set; } = string.Empty;

        [JsonPropertyName("nameFi")]
        public string? NameFi { get; set; }

        [JsonPropertyName("descriptionEn")]
        public string DescriptionEn { get; set; } = string.Empty;

        [JsonPropertyName("descriptionFi")]
        public string? DescriptionFi { get; set; }

        [JsonPropertyName("dataPeriodEn")]
        public string DataPeriodEn { get; set; } = string.Empty;

        [JsonPropertyName("dataPeriodFi")]
        public string? DataPeriodFi { get; set; }

        [JsonPropertyName("updateCadenceEn")]
        public string? UpdateCadenceEn { get; set; }

        [JsonPropertyName("updateCadenceFi")]
        public string? UpdateCadenceFi { get; set; }

        [JsonPropertyName("unitEn")]
        public string UnitEn { get; set; } = string.Empty;

        [JsonPropertyName("unitFi")]
        public string? UnitFi { get; set; }

        [JsonPropertyName("contactPersons")]
        public string ContactPersons { get; set; } = string.Empty;

        [JsonPropertyName("license")]
        public ExternalLicenseDto License { get; set; } = new();

        [JsonPropertyName("keyWordsEn")]
        public List<string> KeyWordsEn { get; set; } = [];

        [JsonPropertyName("keyWordsFi")]
        public List<string>? KeyWordsFi { get; set; }

        [JsonPropertyName("contentGroupsEn")]
        public List<string> ContentGroupsEn { get; set; } = [];

        [JsonPropertyName("contentGroupsFi")]
        public List<string>? ContentGroupsFi { get; set; }

        [JsonPropertyName("availableFormats")]
        public List<string> AvailableFormats { get; set; } = [];

        [JsonPropertyName("dataAvailableFromUtc")]
        public DateTime? DataAvailableFromUtc { get; set; }
    }

    public class ExternalLicenseDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("termsLink")]
        public string TermsLink { get; set; } = string.Empty;
    }
}