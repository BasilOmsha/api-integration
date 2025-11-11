using api_integration.Application.src.DTOs;
using api_integration.UnitTests.src.ApplicationTests.Helpers;

namespace api_integration.UnitTests.src.ApplicationTests.DtoTests
{
    public class MetaDataCreateDtoTests
    {
        [Fact]
        public void ToEntity_WithValidData_ShouldCreateEntity()
        {
            // Arrange
            var dto = MetaDataTestHelpers.CreateValidCreateDto();

            // Act
            var entity = dto.ToEntity();

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(dto.DatasetId, entity.DatasetId);
            Assert.Equal(dto.Type, entity.Type);
            Assert.Equal(dto.License.Name, entity.License.Name);
            Assert.Equal(dto.KeyWordsEn.Count, entity.KeyWordsEn.Count);
        }

        [Theory]
        [InlineData(0, "DatasetId must be greater than 0")]
        [InlineData(-1, "DatasetId must be greater than 0")]
        [InlineData(-100, "DatasetId must be greater than 0")]
        public void DatasetId_WhenInvalid_ShouldFailValidation(int invalidId, string expectedError)
        {
            // Arrange
            var dto = MetaDataTestHelpers.CreateValidCreateDto();
            dto.DatasetId = invalidId;

            // Act
            var validationResults = MetaDataTestHelpers.ValidateDto(dto);

            // Assert
            Assert.True(validationResults.Count > 0);
            Assert.Contains(validationResults, v => v.ErrorMessage!.Contains(expectedError));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void RequiredStringFields_WhenNullOrEmpty_ShouldFailValidation(string? value)
        {
            // Arrange
            var dto = MetaDataTestHelpers.CreateValidCreateDto();
            dto.Type = value!;

            // Act
            var validationResults = MetaDataTestHelpers.ValidateDto(dto);

            // Assert
            Assert.True(validationResults.Count > 0);
            Assert.Contains(validationResults, v => 
                v.ErrorMessage!.Contains("required") || 
                v.ErrorMessage!.Contains("field is required"));
        }

        [Theory]
        [InlineData("Type", 51, "Type cannot exceed 50 characters")]
        [InlineData("Status", 21, "maximum length")]
        [InlineData("Organization", 101, "maximum length")]
        [InlineData("NameEn", 201, "maximum length")]
        [InlineData("DescriptionEn", 1001, "The field DescriptionEn must be a string with a maximum length of 1000.")]
        [InlineData("DataPeriodEn", 101, "maximum length")]
        [InlineData("UnitEn", 51, "maximum length")]
        [InlineData("ContactPersons", 501, "maximum length")]
        public void StringFields_WhenExceedingMaxLength_ShouldFailValidation(string fieldName, int length, string expectedErrorPart)
        {
            // Arrange
            var dto = MetaDataTestHelpers.CreateValidCreateDto();
            var longValue = new string('A', length);
            
            switch (fieldName)
            {
                case "Type": dto.Type = longValue; break;
                case "Status": dto.Status = longValue; break;
                case "Organization": dto.Organization = longValue; break;
                case "NameEn": dto.NameEn = longValue; break;
                case "DescriptionEn": dto.DescriptionEn = longValue; break;
                case "DataPeriodEn": dto.DataPeriodEn = longValue; break;
                case "UnitEn": dto.UnitEn = longValue; break;
                case "ContactPersons": dto.ContactPersons = longValue; break;
            }

            // Act
            var validationResults = MetaDataTestHelpers.ValidateDto(dto);

            // Assert
            Assert.True(validationResults.Count > 0);
            Assert.Contains(validationResults, v => v.ErrorMessage!.Contains(expectedErrorPart));
        }

        [Theory]
        [InlineData("KeyWordsEn")]
        [InlineData("ContentGroupsEn")]
        [InlineData("AvailableFormats")]
        public void RequiredCollections_WhenEmpty_ShouldFailValidation(string collectionName)
        {
            // Arrange
            var dto = MetaDataTestHelpers.CreateValidCreateDto();

            switch (collectionName)
            {
                case "KeyWordsEn": dto.KeyWordsEn = []; break;
                case "ContentGroupsEn": dto.ContentGroupsEn = []; break;
                case "AvailableFormats": dto.AvailableFormats = []; break;
            }

            // Act
            var validationResults = MetaDataTestHelpers.ValidateDto(dto);

            // Assert
            Assert.True(validationResults.Count > 0);
            Assert.Contains(validationResults, v => 
                v.ErrorMessage!.Contains("At least one") || 
                v.ErrorMessage!.Contains("required"));
        }

        [Fact]
        public void License_WhenNull_ShouldFailValidation()
        {
            // Arrange
            var dto = MetaDataTestHelpers.CreateValidCreateDto();
            dto.License = null!;

            // Act
            var validationResults = MetaDataTestHelpers.ValidateDto(dto);

            // Assert
            Assert.True(validationResults.Count > 0);
            Assert.Contains(validationResults, v => v.ErrorMessage!.Contains("required"));
        }

        [Fact]
        public void ValidDto_ShouldPassAllValidation()
        {
            // Arrange
            var dto = MetaDataTestHelpers.CreateValidCreateDto();

            // Act
            var validationResults = MetaDataTestHelpers.ValidateDto(dto);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            // Act
            var dto = new MetaDataCreateDto();

            // Assert
            Assert.Equal(0, dto.DatasetId);
            Assert.Equal(string.Empty, dto.Type);
            Assert.NotNull(dto.License);
            Assert.Equal(string.Empty, dto.License.Name);
            Assert.Equal(string.Empty, dto.License.TermsLink);
            Assert.Empty(dto.KeyWordsEn);
            Assert.Empty(dto.ContentGroupsEn);
            Assert.Empty(dto.AvailableFormats);
        }


    }
}