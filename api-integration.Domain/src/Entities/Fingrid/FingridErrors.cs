using api_integration.SharedKernel.src;

namespace api_integration.Domain.src.Entities.Fingrid
{
    public static class FingridErrors
    {
        public static readonly Error DatasetNotFound = Error.NotFound("Dataset.NotFound", "The requested dataset was not found.");
        public static readonly Error InvalidFormat = Error.Validation("Dataset.InvalidFormat", "The requested dataset ID has an invalid format.");
        public static readonly Error ZeroOrNegative = Error.Validation("Dataset.InvalidFormat", "The dataset ID can't be 0 or smaller.");
        public static readonly Error DatasetIdRequired = Error.Validation( "Dataset.IdRequired", "Dataset ID is required. Please provide a valid dataset ID in the URL.");
        public static readonly Error ExternalApiError = Error.Failure("ExternalApi.Error", "External API returned an error.");
        public static readonly Error DeserializationFailure = Error.Failure("Deserialization.Failure", "Failed to deserialize response from external API.");
        public static readonly Error ExternalApiException = Error.Failure("ExternalApi.Exception", "An exception occurred while calling the external API.");
        public static readonly Error UnauthorizedAccess = Error.Failure("ExternalApi.Unauthorized", "Unauthorized access to the external API.");
        public static readonly Error RateLimitExceeded = Error.RateLimit("ExternalApi.RateLimitExceeded", "Rate limit exceeded when accessing the external API.");
        public static readonly Error RequestTimeout = Error.Failure("ExternalApi.RequestTimeout", "The request to the external API timed out.");
        public static Error NetworkError(string message) => Error.Failure("ExternalApi.NetworkError", $"Network error occurred: {message}");
    }
}