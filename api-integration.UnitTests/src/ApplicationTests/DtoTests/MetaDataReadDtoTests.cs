using api_integration.Application.src.DTOs;
using api_integration.Application.src.Interfaces.IDtos;
using api_integration.Domain.src.Entities;
using api_integration.Domain.src.ValueObjects;
using api_integration.UnitTests.src.ApplicationTests.Helpers;

namespace api_integration.UnitTests.src.ApplicationTests.DtoTests
{
    public class MetaDataReadDtoTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            // Act
            var dto = new MetaDataReadDto();

            // Assert
            Assert.Equal(Guid.Empty, dto.Id);
            Assert.Equal(0, dto.DatasetId);
            Assert.Equal(string.Empty, dto.Type);
            Assert.Equal(string.Empty, dto.Status);
            Assert.Equal(string.Empty, dto.Organization);
            Assert.Equal(string.Empty, dto.NameEn);
            Assert.Equal(string.Empty, dto.DescriptionEn);
            Assert.Null(dto.License);
            Assert.Empty(dto.KeyWordsEn);
            Assert.Empty(dto.ContentGroupsEn);
            Assert.Empty(dto.AvailableFormats);
        }

        [Fact]
        public void Properties_ShouldAllowSettingAllValues()
        {
            // Arrange
            var id = Guid.NewGuid();
            var modifiedAt = DateTime.UtcNow;
            var license = new License("Test License", "https://example.com");

            // Act
            var dto = new MetaDataReadDto
            {
                Id = id,
                DatasetId = 123,
                ModifiedAtUtc = modifiedAt,
                Type = "timeseries",
                Status = "active",
                Organization = "Test Org",
                NameEn = "Test Name",
                DescriptionEn = "Test Description",
                DataPeriodEn = "1h",
                UpdateCadenceEn = "Daily",
                UnitEn = "kWh",
                ContactPersons = "test@example.com",
                License = license,
                KeyWordsEn = ["test", "keyword"],
                ContentGroupsEn = ["group1"],
                AvailableFormats = ["json"],
                DataAvailableFromUtc = DateTime.UtcNow
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(123, dto.DatasetId);
            Assert.Equal(modifiedAt, dto.ModifiedAtUtc);
            Assert.Equal("timeseries", dto.Type);
            Assert.Equal("active", dto.Status);
            Assert.Equal(license, dto.License);
            Assert.Equal(2, dto.KeyWordsEn.Count);
            Assert.Contains("test", dto.KeyWordsEn);
            Assert.Contains("keyword", dto.KeyWordsEn);
        }

        [Fact]
        public void OptionalProperties_ShouldAllowNullValues()
        {
            // Act
            var dto = new MetaDataReadDto
            {
                UpdateCadenceEn = null,
                License = null,
                DataAvailableFromUtc = null
            };

            // Assert
            Assert.Null(dto.UpdateCadenceEn);
            Assert.Null(dto.License);
            Assert.Null(dto.DataAvailableFromUtc);
        }

        [Fact]
        public void ShouldImplementIBaseReadDto()
        {
            // Act
            var dto = new MetaDataReadDto();

            // Assert
            Assert.IsAssignableFrom<IBaseReadDto<MetaData>>(dto);
        }

        [Fact]
        public void InterfaceImplementation_IdProperty_ShouldBeAccessible()
        {
            // Arrange
            var expectedId = Guid.NewGuid();
            IBaseReadDto<MetaData> dto = new MetaDataReadDto { Id = expectedId };

            // Act & Assert
            Assert.Equal(expectedId, dto.Id);
        }

        [Fact]
        public void Collections_ShouldBeMutable()
        {
            // Arrange
            var dto = new MetaDataReadDto();

            // Act
            dto.KeyWordsEn.Add("test");
            dto.ContentGroupsEn.Add("group");
            dto.AvailableFormats.Add("json");

            // Assert
            Assert.Contains("test", dto.KeyWordsEn);
            Assert.Contains("group", dto.ContentGroupsEn);
            Assert.Contains("json", dto.AvailableFormats);
        }

        [Fact]
        public void WithValidTestHelper_ShouldHaveAllRequiredProperties()
        {
            // Act
            var dto = MetaDataTestHelpers.CreateValidReadDto();

            // Assert
            Assert.NotEqual(Guid.Empty, dto.Id);
            Assert.True(dto.DatasetId > 0);
            Assert.False(string.IsNullOrEmpty(dto.Type));
            Assert.NotNull(dto.License);
            Assert.False(string.IsNullOrEmpty(dto.License.Name));
            Assert.NotEmpty(dto.KeyWordsEn);
            Assert.NotEmpty(dto.AvailableFormats);
        }

        [Fact]
        public void CreateValidReadDto_WithSpecificId_ShouldUseProvidedId()
        {
            // Arrange
            var specificId = Guid.NewGuid();

            // Act
            var dto = MetaDataTestHelpers.CreateValidReadDto(specificId);

            // Assert
            Assert.Equal(specificId, dto.Id);
        }

        [Fact]
        public void CreateValidReadDto_WithoutId_ShouldGenerateNewId()
        {
            // Act
            var dto1 = MetaDataTestHelpers.CreateValidReadDto();
            var dto2 = MetaDataTestHelpers.CreateValidReadDto();

            // Assert
            Assert.NotEqual(Guid.Empty, dto1.Id);
            Assert.NotEqual(Guid.Empty, dto2.Id);
            Assert.NotEqual(dto1.Id, dto2.Id);
        }

        [Fact]
        public void AllProperties_ShouldMatchTestHelperValues()
        {
            // Act
            var dto = MetaDataTestHelpers.CreateValidReadDto();

            // Assert
            Assert.Equal(363, dto.DatasetId);
            Assert.Equal("timeseries", dto.Type);
            Assert.Equal("active", dto.Status);
            Assert.Equal("Datahub", dto.Organization);
            Assert.Equal("Total electricity consumption in Finnish distribution networks", dto.NameEn);
            Assert.Equal("1 h", dto.DataPeriodEn);
            Assert.Equal("kWh", dto.UnitEn);
            Assert.Equal("Creative Commons Attribution", dto.License!.Name);
            Assert.Equal("https://creativecommons.org/licenses/by/4.0/", dto.License.TermsLink);
            Assert.Equal(2, dto.KeyWordsEn.Count);
            Assert.Contains("Electricity consumption", dto.KeyWordsEn);
            Assert.Contains("Datahub", dto.KeyWordsEn);
        }
    }
}