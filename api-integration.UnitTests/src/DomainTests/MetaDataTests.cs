using api_integration.Domain.src.Entities;
using api_integration.Domain.src.ValueObjects;

namespace api_integration.UnitTests.src.DomainTests;

public class MetaDataTests
{
    [Fact]
    public void CreateMetaData_WithValidData_ShouldCreateMetaData()
    {
        // Arrange
        var datasetId = 123;
        var modifiedAtUtc = DateTime.UtcNow;
        var type = "Sample Type";
        var status = "Active";
        var organization = "Sample Organization";
        var nameEn = "Sample Name";
        var descriptionEn = "Sample Description";
        var dataPeriodEn = "2020-2023";
        var updateCadenceEn = "Monthly";
        var unitEn = "kWh";
        var contactPersons = "John Doe";
        var license = new License("Sample License", "http://example.com/license");
        var keyWordsEn = new List<string> { "energy", "consumption" };
        var contentGroupsEn = new List<string> { "group1", "group2" };
        var availableFormats = new List<string> { "JSON", "CSV" };
        var dataAvailableFromUtc = new DateTime(2020, 1, 1);

        // Act
        var metaData = new MetaData(
            datasetId,
            modifiedAtUtc,
            type,
            status,
            organization,
            nameEn,
            descriptionEn,
            dataPeriodEn,
            updateCadenceEn,
            unitEn,
            contactPersons,
            license,
            keyWordsEn,
            contentGroupsEn,
            availableFormats,
            dataAvailableFromUtc
        );

        // Assert
        Assert.NotNull(metaData);
        Assert.Equal(datasetId, metaData.DatasetId);
        Assert.Equal(modifiedAtUtc, metaData.ModifiedAtUtc);
        Assert.Equal(type, metaData.Type);
        Assert.Equal(status, metaData.Status);
        Assert.Equal(organization, metaData.Organization);
        Assert.Equal(nameEn, metaData.NameEn);
        Assert.Equal(descriptionEn, metaData.DescriptionEn);
        Assert.Equal(dataPeriodEn, metaData.DataPeriodEn);
        Assert.Equal(updateCadenceEn, metaData.UpdateCadenceEn);
        Assert.Equal(unitEn, metaData.UnitEn);
        Assert.Equal(contactPersons, metaData.ContactPersons);
        Assert.Equal(license, metaData.License);
        Assert.Equal(keyWordsEn, metaData.KeyWordsEn);
        Assert.Equal(contentGroupsEn, metaData.ContentGroupsEn);
        Assert.Equal(availableFormats, metaData.AvailableFormats);
        Assert.Equal(dataAvailableFromUtc, metaData.DataAvailableFromUtc);
    }

    [Fact]
    public void CreateMetaData_WithEmptyConstructor_ShouldInitializeCollections()
    {
        // Act
        var metaData = new MetaData()
        {
            // Set required properties
            Type = "timeseries",
            Status = "active",
            Organization = "TestOrg",
            NameEn = "Test Name",
            DescriptionEn = "Test Description",
            DataPeriodEn = "1 hour",
            UnitEn = "kWh",
            ContactPersons = "test@example.com",
            License = new License("MIT", "https://mit.edu")
        };

        // Assert
        Assert.NotNull(metaData);
        Assert.NotNull(metaData.KeyWordsEn);
        Assert.NotNull(metaData.ContentGroupsEn);
        Assert.NotNull(metaData.AvailableFormats);
        Assert.Empty(metaData.KeyWordsEn);
        Assert.Empty(metaData.ContentGroupsEn);
        Assert.Empty(metaData.AvailableFormats);
    }

    [Fact]
    public void CreateMetaData_WithId_ShouldUseProvidedId()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var license = new License("MIT", "https://mit.edu");
        var keyWords = new List<string> { "test" };
        var contentGroups = new List<string> { "group1" };
        var formats = new List<string> { "json" };

        // Act
        var metaData = new MetaData(
            expectedId,
            123,
            DateTime.UtcNow,
            "timeseries",
            "active",
            "TestOrg",
            "Test Name",
            "Test Description",
            "1 hour",
            "daily",
            "kWh",
            "test@example.com",
            license,
            keyWords,
            contentGroups,
            formats,
            DateTime.UtcNow
        );

        // Assert
        Assert.Equal(expectedId, metaData.Id);
    }

    [Fact]
    public void CreateMetaData_WithNullType_ShouldThrowArgumentNullException()
    {
        // Arrange
        var license = new License("MIT", "https://mit.edu");
        var keyWords = new List<string> { "test" };
        var contentGroups = new List<string> { "group1" };
        var formats = new List<string> { "json" };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new MetaData(
            123,
            DateTime.UtcNow,
            null!,
            "active",
            "TestOrg",
            "Test Name",
            "Test Description",
            "1 hour",
            "daily",
            "kWh",
            "test@example.com",
            license,
            keyWords,
            contentGroups,
            formats,
            DateTime.UtcNow
        ));
    }

    [Fact]
    public void CreateMetaData_WithEmptyAvailableFormats_ShouldThrowArgumentException()
    {
        var license = new License("MIT", "https://mit.edu");
        var keyWords = new List<string> { "test" };
        var contentGroups = new List<string> { "group1" };
        var emptyFormats = new List<string>();

        Assert.Throws<ArgumentException>(() => new MetaData(
            123,
            DateTime.UtcNow,
            "timeseries",
            "active",
            "TestOrg",
            "Test Name",
            "Test Description",
            "1 hour",
            "daily",
            "kWh",
            "test@example.com",
            license,
            keyWords,
            contentGroups,
            emptyFormats,
            DateTime.UtcNow
        ));
    }

    [Fact]
    public void CreateMetaData_WithZeroDatasetId_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var license = new License("MIT", "https://mit.edu");
        var keyWords = new List<string> { "test" };
        var contentGroups = new List<string> { "group1" };
        var formats = new List<string> { "json" };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new MetaData(
            0,
            DateTime.UtcNow,
            "timeseries",
            "active",
            "TestOrg",
            "Test Name",
            "Test Description",
            "1 hour",
            "daily",
            "kWh",
            "test@example.com",
            license,
            keyWords,
            contentGroups,
            formats,
            DateTime.UtcNow
        ));
    }

    [Fact]
    public void CreateMetaData_WithNullLicense_ShouldThrowArgumentNullException()
    {
        // Arrange
        var keyWords = new List<string> { "test" };
        var contentGroups = new List<string> { "group1" };
        var formats = new List<string> { "json" };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new MetaData(
            123,
            DateTime.UtcNow,
            "timeseries",
            "active",
            "TestOrg",
            "Test Name",
            "Test Description",
            "1 hour",
            "daily",
            "kWh",
            "test@example.com",
            null!,
            keyWords,
            contentGroups,
            formats,
            DateTime.UtcNow
        ));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateMetaData_WithEmptyOrWhitespaceType_ShouldThrowArgumentException(string invalidType)
    {
        // Arrange
        var license = new License("MIT", "https://mit.edu");
        var keyWords = new List<string> { "test" };
        var contentGroups = new List<string> { "group1" };
        var formats = new List<string> { "json" };

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new MetaData(
            123,
            DateTime.UtcNow,
            invalidType,
            "active",
            "TestOrg",
            "Test Name",
            "Test Description",
            "1 hour",
            "daily",
            "kWh",
            "test@example.com",
            license,
            keyWords,
            contentGroups,
            formats,
            DateTime.UtcNow
        ));
    }
}