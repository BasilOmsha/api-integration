using System.Text.Json;
using api_integration.Application.src.Mappers;
using api_integration.Application.src.DTOs;
using api_integration.UnitTests.src.ApplicationTests.Helpers;

namespace api_integration.UnitTests.src.ApplicationTests.DtoTests
{
    public class MetaDataMapperTests
    {
        [Fact]
        public void ToInternalDto_WithValidExternalDto_ShouldMapAllProperties()
        {
            // Arrange
            var json = MetaDataTestHelpers.GetValidExternalApiJson();
            var external = JsonSerializer.Deserialize<MetaDataExternalApiResDto>(json)!;

            // Act
            var result = external.ToInternalDto();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(external.Id, result.DatasetId);
            Assert.Equal(external.ModifiedAtUtc, result.ModifiedAtUtc);
            Assert.Equal(external.Type, result.Type);
            Assert.Equal(external.Status, result.Status);
            Assert.Equal(external.Organization, result.Organization);
            Assert.Equal(external.NameEn, result.NameEn);
            Assert.Equal(external.DescriptionEn, result.DescriptionEn);
            Assert.Equal(external.DataPeriodEn, result.DataPeriodEn);
            Assert.Equal(external.UpdateCadenceEn, result.UpdateCadenceEn);
            Assert.Equal(external.UnitEn, result.UnitEn);
            Assert.Equal(external.ContactPersons, result.ContactPersons);
            Assert.Equal(external.DataAvailableFromUtc, result.DataAvailableFromUtc);
        }

        [Fact]
        public void ToInternalDto_ShouldMapLicenseCorrectly()
        {
            // Arrange
            var external = MetaDataTestHelpers.CreateValidExternalDto();


            // Act
            var result = external.ToInternalDto();

            // Assert
            Assert.NotNull(result.License);
            Assert.Equal(external.License.Name, result.License.Name);
            Assert.Equal(external.License.TermsLink, result.License.TermsLink);
        }

        [Fact]
        public void ToInternalDto_ShouldMapCollectionsCorrectly()
        {
            // Arrange
            var external = MetaDataTestHelpers.CreateValidExternalDto();

            // Act
            var result = external.ToInternalDto();

            // Assert
            Assert.Equal(external.KeyWordsEn.Count, result.KeyWordsEn.Count);
            Assert.Equal(external.ContentGroupsEn.Count, result.ContentGroupsEn.Count);
            Assert.Equal(external.AvailableFormats.Count, result.AvailableFormats.Count);

            foreach (var keyword in external.KeyWordsEn)
            {
                Assert.Contains(keyword, result.KeyWordsEn);
            }

            foreach (var format in external.AvailableFormats)
            {
                Assert.Contains(format, result.AvailableFormats);
            }
        }
        
        [Fact]
        public void ToInternalDto_FromDeserializedJson_ShouldWork()
        {
            // Arrange
            var json = MetaDataTestHelpers.GetValidExternalApiJson();
            var external = MetaDataTestHelpers.DeserializeFromJson(json);

            // Act
            var result = external.ToInternalDto();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(363, result.DatasetId);
            Assert.Equal("timeseries", result.Type);
            Assert.Equal("Creative Commons Attribution", result.License.Name);
            Assert.Equal(2, result.KeyWordsEn.Count);
            Assert.Equal(4, result.AvailableFormats.Count);
        }

        [Fact]
        public void ToInternalDto_WithNullOptionalFields_ShouldHandleGracefully()
        {
            // Arrange
            var external = new MetaDataExternalApiResDto
            {
                Id = 123,
                ModifiedAtUtc = DateTime.UtcNow,
                Type = "test",
                Status = "active",
                Organization = "org",
                NameEn = "name",
                DescriptionEn = "desc",
                DataPeriodEn = "1h",
                UnitEn = "unit",
                ContactPersons = "contact",
                License = new ExternalLicenseDto { Name = "license", TermsLink = "link" },
                KeyWordsEn = ["keyword"],
                ContentGroupsEn = ["group"],
                AvailableFormats = ["json"],
                UpdateCadenceEn = null,
                DataAvailableFromUtc = null
            };

            // Act
            var result = external.ToInternalDto();

            // Assert
            Assert.Null(result.UpdateCadenceEn);
            Assert.Null(result.DataAvailableFromUtc);
        }

        [Fact]
        public void ToInternalDto_WithEmptyCollections_ShouldCreateEmptyCollections()
        {
            // Arrange
            var external = new MetaDataExternalApiResDto
            {
                Id = 123,
                ModifiedAtUtc = DateTime.UtcNow,
                Type = "test",
                Status = "active",
                Organization = "org",
                NameEn = "name",
                DescriptionEn = "desc",
                DataPeriodEn = "1h",
                UnitEn = "unit",
                ContactPersons = "contact",
                License = new ExternalLicenseDto { Name = "license", TermsLink = "link" },
                KeyWordsEn = [],
                ContentGroupsEn = [],
                AvailableFormats = []
            };

            // Act
            var result = external.ToInternalDto();

            // Assert
            Assert.Empty(result.KeyWordsEn);
            Assert.Empty(result.ContentGroupsEn);
            Assert.Empty(result.AvailableFormats);
        }
    }
}