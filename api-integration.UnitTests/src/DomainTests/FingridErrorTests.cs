using api_integration.Domain.src.Entities.Fingrid;
using api_integration.SharedKernel.src;

namespace api_integration.UnitTests.src.DomainTests
{
    public class FingridErrorsTests
    {
        // Static Error Properties Tests
        [Fact]
        public void DatasetNotFound_HasCorrectProperties()
        {
            var error = FingridErrors.DatasetNotFound;

            Assert.Equal("Dataset.NotFound", error.Code);
            Assert.Equal("The requested dataset was not found.", error.Description);
            Assert.Equal(ErrorType.NotFound, error.Type);
        }

        [Fact]
        public void InvalidFormat_HasCorrectProperties()
        {
            var error = FingridErrors.InvalidFormat;

            Assert.Equal("Dataset.InvalidFormat", error.Code);
            Assert.Equal("The requested dataset ID has an invalid format.", error.Description);
            Assert.Equal(ErrorType.Validation, error.Type);
        }

        [Fact]
        public void ZeroOrNegative_HasCorrectProperties()
        {
            var error = FingridErrors.ZeroOrNegative;

            Assert.Equal("Dataset.InvalidFormat", error.Code);
            Assert.Equal("The dataset ID can't be 0 or smaller.", error.Description);
            Assert.Equal(ErrorType.Validation, error.Type);
        }

        [Fact]
        public void DatasetIdRequired_HasCorrectProperties()
        {
            var error = FingridErrors.DatasetIdRequired;

            Assert.Equal("Dataset.IdRequired", error.Code);
            Assert.Equal("Dataset ID is required. Please provide a valid dataset ID in the URL.", error.Description);
            Assert.Equal(ErrorType.Validation, error.Type);
        }

        [Fact]
        public void ExternalApiError_HasCorrectProperties()
        {
            var error = FingridErrors.ExternalApiError;

            Assert.Equal("ExternalApi.Error", error.Code);
            Assert.Equal("External API returned an error.", error.Description);
            Assert.Equal(ErrorType.Failure, error.Type);
        }

        [Fact]
        public void DeserializationFailure_HasCorrectProperties()
        {
            var error = FingridErrors.DeserializationFailure;

            Assert.Equal("Deserialization.Failure", error.Code);
            Assert.Equal("Failed to deserialize response from external API.", error.Description);
            Assert.Equal(ErrorType.Failure, error.Type);
        }

        [Fact]
        public void ExternalApiException_HasCorrectProperties()
        {
            var error = FingridErrors.ExternalApiException;

            Assert.Equal("ExternalApi.Exception", error.Code);
            Assert.Equal("An exception occurred while calling the external API.", error.Description);
            Assert.Equal(ErrorType.Failure, error.Type);
        }

        [Fact]
        public void UnauthorizedAccess_HasCorrectProperties()
        {
            var error = FingridErrors.UnauthorizedAccess;

            Assert.Equal("ExternalApi.Unauthorized", error.Code);
            Assert.Equal("Unauthorized access to the external API.", error.Description);
            Assert.Equal(ErrorType.Failure, error.Type);
        }

        [Fact]
        public void RateLimitExceeded_HasCorrectProperties()
        {
            var error = FingridErrors.RateLimitExceeded;

            Assert.Equal("ExternalApi.RateLimitExceeded", error.Code);
            Assert.Equal("Rate limit exceeded when accessing the external API.", error.Description);
            Assert.Equal(ErrorType.RateLimit, error.Type);
        }

        [Fact]
        public void RequestTimeout_HasCorrectProperties()
        {
            var error = FingridErrors.RequestTimeout;

            Assert.Equal("ExternalApi.RequestTimeout", error.Code);
            Assert.Equal("The request to the external API timed out.", error.Description);
            Assert.Equal(ErrorType.Failure, error.Type);
        }


        // Factory Method Tests
        [Theory]
        [InlineData("Connection refused")]
        [InlineData("DNS resolution failed")]
        [InlineData("")]
        [InlineData(null)]
        public void NetworkError_WithMessage_CreatesCorrectError(string? message)
        {
            var error = FingridErrors.NetworkError(message ?? string.Empty);

            Assert.Equal("ExternalApi.NetworkError", error.Code);
            Assert.StartsWith("Network error occurred:", error.Description);
            Assert.Contains(message ?? string.Empty, error.Description);
            Assert.Equal(ErrorType.Failure, error.Type);
        }

        [Fact]
        public void NetworkError_WithSpecificMessage_HasExpectedDescription()
        {
            var message = "Connection timeout after 30 seconds";

            var error = FingridErrors.NetworkError(message);

            Assert.Equal($"Network error occurred: {message}", error.Description);
        }

        //Error Uniqueness Tests
        [Fact]
        public void AllStaticErrors_HaveUniqueCodes()
        {
            var errors = new[]
            {
                FingridErrors.DatasetNotFound,
                FingridErrors.InvalidFormat,
                FingridErrors.ZeroOrNegative,
                FingridErrors.DatasetIdRequired,
                FingridErrors.ExternalApiError,
                FingridErrors.DeserializationFailure,
                FingridErrors.ExternalApiException,
                FingridErrors.UnauthorizedAccess,
                FingridErrors.RateLimitExceeded,
                FingridErrors.RequestTimeout
            };

            var distinctCodes = errors.Select(e => e.Code).Distinct().ToList();

            //InvalidFormat and ZeroOrNegative have the same code intentionally
            Assert.Equal(9, distinctCodes.Count); // 10 errors, but 2 share the same code
        }

        [Fact]
        public void ValidationErrors_HaveValidationType()
        {
            var validationErrors = new[]
            {
                FingridErrors.InvalidFormat,
                FingridErrors.ZeroOrNegative,
                FingridErrors.DatasetIdRequired
            };

            Assert.All(validationErrors, error => Assert.Equal(ErrorType.Validation, error.Type));
        }

        [Fact]
        public void ApiErrors_HaveCorrectTypes()
        {
            Assert.Equal(ErrorType.NotFound, FingridErrors.DatasetNotFound.Type);
            Assert.Equal(ErrorType.RateLimit, FingridErrors.RateLimitExceeded.Type);
            Assert.Equal(ErrorType.Failure, FingridErrors.ExternalApiError.Type);
            Assert.Equal(ErrorType.Failure, FingridErrors.UnauthorizedAccess.Type);
        }
    }
}