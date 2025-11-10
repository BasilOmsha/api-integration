using System.Text.Json;
using api_integration.Application.src.DTOs;
using api_integration.UnitTests.src.ApplicationTests.Helpers;

namespace api_integration.UnitTests.src.ApplicationTests
{
    public class MetaDataExternalApiResDtoTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            // Act
            var dto = new MetaDataExternalApiResDto();

            // Assert
            Assert.Equal(0, dto.Id);
            Assert.Equal(string.Empty, dto.Type);
            Assert.Equal(string.Empty, dto.Status);
            Assert.Equal(string.Empty, dto.Organization);
            Assert.Equal(string.Empty, dto.NameEn);
            Assert.Null(dto.NameFi);
            Assert.NotNull(dto.License);
            Assert.Equal(string.Empty, dto.License.Name);
            Assert.Empty(dto.KeyWordsEn);
            Assert.Null(dto.KeyWordsFi);
            Assert.Empty(dto.AvailableFormats);
        }

        [Fact]
        public void DeserializeFromValidJson_ShouldMapAllProperties()
        {
            // Arrange
            var json = MetaDataTestHelpers.GetValidExternalApiJson();

            // Act
            var result = JsonSerializer.Deserialize<MetaDataExternalApiResDto>(json);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(363, result.Id);
            Assert.Equal(new DateTime(2024, 10, 30, 7, 25, 28, DateTimeKind.Utc), result.ModifiedAtUtc);
            Assert.Equal("timeseries", result.Type);
            Assert.Equal("active", result.Status);
            Assert.Equal("Datahub", result.Organization);
            Assert.Equal("Total electricity consumption in Finnish distribution networks", result.NameEn);
            Assert.Equal("Sähkönkulutus Suomen jakeluverkoissa yhteensä", result.NameFi);
            Assert.Equal("1 h", result.DataPeriodEn);
            Assert.Null(result.UpdateCadenceEn);
            Assert.Equal("kWh", result.UnitEn);
        }

        [Fact]
        public void DeserializeFromValidJson_ShouldMapLicense()
        {
            // Arrange
            var json = MetaDataTestHelpers.GetValidExternalApiJson();

            // Act
            var result = JsonSerializer.Deserialize<MetaDataExternalApiResDto>(json);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.License);
            Assert.Equal("Creative Commons Attribution", result.License.Name);
            Assert.Equal("https://creativecommons.org/licenses/by/4.0/", result.License.TermsLink);
        }

        [Fact]
        public void DeserializeFromValidJson_ShouldMapCollections()
        {
            // Arrange
            var json = MetaDataTestHelpers.GetValidExternalApiJson();

            // Act
            var result = JsonSerializer.Deserialize<MetaDataExternalApiResDto>(json);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.KeyWordsEn.Count);
            Assert.Contains("Electricity consumption", result.KeyWordsEn);
            Assert.Contains("Datahub", result.KeyWordsEn);
            
            Assert.NotNull(result.KeyWordsFi);
            Assert.Equal(2, result.KeyWordsFi.Count);
            Assert.Contains("Sähkönkulutus", result.KeyWordsFi);
            
            Assert.Single(result.ContentGroupsEn);
            Assert.Contains("Consumption and production", result.ContentGroupsEn);
            
            Assert.Equal(4, result.AvailableFormats.Count);
            Assert.Contains("json", result.AvailableFormats);
            Assert.Contains("csv", result.AvailableFormats);
            Assert.Contains("xlsx", result.AvailableFormats);
            Assert.Contains("xml", result.AvailableFormats);
        }

        [Theory]
        [InlineData("")]
        [InlineData("invalid json")]
        [InlineData("{")]
        public void DeserializeFromInvalidJson_ShouldThrowJsonException(string invalidJson)
        {
            // Act & Assert
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<MetaDataExternalApiResDto>(invalidJson));
        }

        [Fact]
        public void DeserializeFromEmptyObject_ShouldUseDefaults()
        {
            // Arrange
            var emptyJson = "{}";

            // Act
            var result = JsonSerializer.Deserialize<MetaDataExternalApiResDto>(emptyJson);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Id);
            Assert.Equal(string.Empty, result.Type);
            Assert.NotNull(result.License);
            Assert.Equal(string.Empty, result.License.Name);
            Assert.Empty(result.KeyWordsEn);
        }

        [Fact]
        public void SerializeToJson_ShouldUseCorrectPropertyNames()
        {
            // Arrange
            var dto = new MetaDataExternalApiResDto
            {
                Id = 123,
                Type = "test"
            };

            // Act
            var json = JsonSerializer.Serialize(dto);

            // Assert
            Assert.Contains("\"id\":", json);
            Assert.Contains("\"type\":", json);
            Assert.DoesNotContain("\"Id\":", json);
            Assert.DoesNotContain("\"Type\":", json);
        }
    }
}