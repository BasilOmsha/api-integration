using System.Text.Json;
using api_integration.Application.src.DTOs;
using api_integration.Domain.src.ValueObjects;

namespace api_integration.UnitTests.src.ApplicationTests.Helpers
{
    public class MetaDataTestHelpers
    {
        public static string GetValidExternalApiJson()
        {
            return """
            {
                "id": 363,
                "modifiedAtUtc": "2024-10-30T07:25:28.000Z",
                "type": "timeseries",
                "status": "active",
                "organization": "Datahub",
                "nameFi": "Sähkönkulutus Suomen jakeluverkoissa yhteensä",
                "nameEn": "Total electricity consumption in Finnish distribution networks",
                "descriptionFi": "Suomessa sijaitsevien sähkön kulutuskäyttöpaikkojen mittausten yhteenlasketut summatiedot tunneittain jakeluverkoissa.",
                "descriptionEn": "Aggregate hourly metering data for electricity accounting points in Finnish distribution networks.",
                "dataPeriodFi": "1 h",
                "dataPeriodEn": "1 h",
                "updateCadenceFi": null,
                "updateCadenceEn": null,
                "unitFi": "kWh",
                "unitEn": "kWh",
                "contactPersons": "Datahub asiakaspalvelu: https://support.datahub.fi/fingrid?id=csm_public_form",
                "license": {
                    "name": "Creative Commons Attribution",
                    "termsLink": "https://creativecommons.org/licenses/by/4.0/"
                },
                "keyWordsFi": ["Sähkönkulutus", "Datahub"],
                "keyWordsEn": ["Electricity consumption", "Datahub"],
                "contentGroupsFi": ["Kulutus ja tuotanto"],
                "contentGroupsEn": ["Consumption and production"],
                "availableFormats": ["json", "csv", "xlsx", "xml"],
                "dataAvailableFromUtc": null
            }
            """;
        }

        public static MetaDataExternalApiResDto CreateValidExternalDto()
        {
            return new MetaDataExternalApiResDto
            {
                Id = 363,
                ModifiedAtUtc = new DateTime(2024, 10, 30, 7, 25, 28, DateTimeKind.Utc),
                Type = "timeseries",
                Status = "active",
                Organization = "Datahub",
                NameFi = "Sähkönkulutus Suomen jakeluverkoissa yhteensä",
                NameEn = "Total electricity consumption in Finnish distribution networks",
                DescriptionFi = "Suomessa sijaitsevien sähkön kulutuskäyttöpaikkojen mittausten yhteenlasketut summatiedot tunneittain jakeluverkoissa.",
                DescriptionEn = "Aggregate hourly metering data for electricity accounting points in Finnish distribution networks.",
                DataPeriodFi = "1 h",
                DataPeriodEn = "1 h",
                UpdateCadenceFi = null,
                UpdateCadenceEn = null,
                UnitFi = "kWh",
                UnitEn = "kWh",
                ContactPersons = "Datahub asiakaspalvelu: https://support.datahub.fi/fingrid?id=csm_public_form",
                License = new ExternalLicenseDto
                {
                    Name = "Creative Commons Attribution",
                    TermsLink = "https://creativecommons.org/licenses/by/4.0/"
                },
                KeyWordsFi = ["Sähkönkulutus", "Datahub"],
                KeyWordsEn = ["Electricity consumption", "Datahub"],
                ContentGroupsFi = ["Kulutus ja tuotanto"],
                ContentGroupsEn = ["Consumption and production"],
                AvailableFormats = ["json", "csv", "xlsx", "xml"],
                DataAvailableFromUtc = null
            };
        }

        public static MetaDataCreateDto CreateValidCreateDto()
        {
            return new MetaDataCreateDto
            {
                DatasetId = 363,
                ModifiedAtUtc = new DateTime(2024, 10, 30, 7, 25, 28, DateTimeKind.Utc),
                Type = "timeseries",
                Status = "active",
                Organization = "Datahub",
                NameEn = "Total electricity consumption in Finnish distribution networks",
                DescriptionEn = "Aggregate hourly metering data for electricity accounting points in Finnish distribution networks.",
                DataPeriodEn = "1 h",
                UpdateCadenceEn = null,
                UnitEn = "kWh",
                ContactPersons = "Datahub asiakaspalvelu: https://support.datahub.fi/fingrid?id=csm_public_form",
                License = new License("Creative Commons Attribution", "https://creativecommons.org/licenses/by/4.0/"),
                KeyWordsEn = ["Electricity consumption", "Datahub"],
                ContentGroupsEn = ["Consumption and production"],
                AvailableFormats = ["json", "csv", "xlsx", "xml"],
                DataAvailableFromUtc = null
            };
        }

        public static MetaDataReadDto CreateValidReadDto(Guid? id = null)
        {
            return new MetaDataReadDto
            {
                Id = id ?? Guid.NewGuid(),
                DatasetId = 363,
                ModifiedAtUtc = new DateTime(2024, 10, 30, 7, 25, 28, DateTimeKind.Utc),
                Type = "timeseries",
                Status = "active",
                Organization = "Datahub",
                NameEn = "Total electricity consumption in Finnish distribution networks",
                DescriptionEn = "Aggregate hourly metering data for electricity accounting points in Finnish distribution networks.",
                DataPeriodEn = "1 h",
                UpdateCadenceEn = null,
                UnitEn = "kWh",
                ContactPersons = "Datahub asiakaspalvelu: https://support.datahub.fi/fingrid?id=csm_public_form",
                License = new License("Creative Commons Attribution", "https://creativecommons.org/licenses/by/4.0/"),
                KeyWordsEn = ["Electricity consumption", "Datahub"],
                ContentGroupsEn = ["Consumption and production"],
                AvailableFormats = ["json", "csv", "xlsx", "xml"],
                DataAvailableFromUtc = null
            };
        }

        public static MetaDataExternalApiResDto DeserializeFromJson(string json)
        {            
            return JsonSerializer.Deserialize<MetaDataExternalApiResDto>(json) 
                ?? throw new InvalidOperationException("Failed to deserialize JSON");
        }
    }
}