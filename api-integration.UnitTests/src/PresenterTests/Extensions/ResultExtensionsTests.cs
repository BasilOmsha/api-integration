using System.Text.Json;
using api_integration.Presenter.API.src.Extensions;
using api_integration.SharedKernel.src;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api_integration.UnitTests.src.PresenterTests.Extensions
{
     public class ResultExtensionsTests
    {
        //Success Result Tests
        [Fact]
        public void ToProblemDetails_WithSuccessResult_ThrowsInvalidOperationException()
        {
            var successResult = Result.Success();

            var exception = Assert.Throws<InvalidOperationException>(() => 
                successResult.ToProblemDetails());
            Assert.Equal("Can't convert success result to problem", exception.Message);
        }

        [Fact]
        public void ToProblemDetails_Generic_WithSuccessResult_ThrowsInvalidOperationException()
        {
            var successResult = Result.Success("test value");

            var exception = Assert.Throws<InvalidOperationException>(() => 
                successResult.ToProblemDetails());
            Assert.Equal("Can't convert success result to problem", exception.Message);
        }

        // Error Type Mapping Tests
        [Theory]
        [InlineData(ErrorType.Validation, 400, "Bad Request", "https://tools.ietf.org/html/rfc7231#section-6.5.1")]
        [InlineData(ErrorType.NotFound, 404, "Not Found", "https://tools.ietf.org/html/rfc7231#section-6.5.4")]
        [InlineData(ErrorType.Conflict, 409, "Conflict", "https://tools.ietf.org/html/rfc7231#section-6.5.8")]
        [InlineData(ErrorType.RateLimit, 429, "Rate Limit Exceeded", "https://tools.ietf.org/html/rfc6585#section-4")]
        [InlineData(ErrorType.Failure, 500, "Internal Server Error", "https://tools.ietf.org/html/rfc7231#section-6.6.1")]
        public void ToProblemDetails_MapsErrorTypeCorrectly(ErrorType errorType, int expectedStatusCode, 
            string expectedTitle, string expectedType)
        {
            var error = CreateErrorOfType(errorType);
            var result = Result.Failure(error);

            var actionResult = result.ToProblemDetails();

            var objectResult = Assert.IsType<ObjectResult>(actionResult);
            var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);

            Assert.Equal(expectedStatusCode, problemDetails.Status);
            Assert.Equal(expectedStatusCode, objectResult.StatusCode);
            Assert.Equal(expectedTitle, problemDetails.Title);
            Assert.Equal(expectedType, problemDetails.Type);
            Assert.Equal(error.Description, problemDetails.Detail);
        }

        [Fact]
        public void ToProblemDetails_Generic_MapsErrorTypeCorrectly()
        {
            var error = Error.NotFound("Test.NotFound", "Resource not found");
            var result = Result.Failure<string>(error);

            var actionResult = result.ToProblemDetails();

            var objectResult = Assert.IsType<ObjectResult>(actionResult);
            var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);

            Assert.Equal(404, problemDetails.Status);
            Assert.Equal(404, objectResult.StatusCode);
            Assert.Equal("Not Found", problemDetails.Title);
            Assert.Equal(error.Description, problemDetails.Detail);
        }

        // HttpContext Integration Tests
        [Fact]
        public void ToProblemDetails_WithHttpContext_SetsInstanceCorrectly()
        {
            var httpContext = CreateMockHttpContext("GET", "/api/test", "?param=value");
            var error = Error.Validation("Test.Error", "Validation failed");
            var result = Result.Failure(error);

            var actionResult = result.ToProblemDetails(httpContext);

            var objectResult = Assert.IsType<ObjectResult>(actionResult);
            var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);

            Assert.Equal("GET /api/test ?param=value", problemDetails.Instance);
        }

        [Fact]
        public void ToProblemDetails_WithoutHttpContext_SetsEmptyInstance()
        {
            var error = Error.Validation("Test.Error", "Validation failed");
            var result = Result.Failure(error);

            var actionResult = result.ToProblemDetails();

            var objectResult = Assert.IsAssignableFrom<ActionResult>(actionResult);
            var problemDetails = Assert.IsType<ProblemDetails>(((ObjectResult)objectResult).Value);

            Assert.Equal(string.Empty, problemDetails.Instance);
        }

        [Theory]
        [InlineData("POST", "/api/users", "")]
        [InlineData("DELETE", "/api/users/123", "?force=true")]
        [InlineData("PUT", "/api/update", "?version=2")]
        public void ToProblemDetails_WithVariousHttpMethods_SetsInstanceCorrectly(
            string method, string path, string queryString)
        {
            var httpContext = CreateMockHttpContext(method, path, queryString);
            var error = Error.NotFound("Test.NotFound", "Not found");
            var result = Result.Failure(error);

            var actionResult = result.ToProblemDetails(httpContext);

            var objectResult = Assert.IsType<ObjectResult>(actionResult);
            var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);

            Assert.Equal($"{method} {path} {queryString}", problemDetails.Instance);
        }

        // Extensions Property Tests
        [Fact]
        public void ToProblemDetails_SetsExtensionsCorrectly()
        {
            var error = Error.Validation("Test.Error", "Validation error");
            var result = Result.Failure(error);

            var actionResult = result.ToProblemDetails();

            var objectResult = Assert.IsType<ObjectResult>(actionResult);
            var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);

            Assert.NotNull(problemDetails.Extensions);
            Assert.Equal(false, problemDetails.Extensions["isSuccess"]);
            Assert.Equal(true, problemDetails.Extensions["isFailure"]);
            Assert.Null(problemDetails.Extensions["value"]);
            
            var errors = Assert.IsType<Error[]>(problemDetails.Extensions["errors"]);
            Assert.Single(errors);
            Assert.Equal(error, errors[0]);
        }

        [Fact]
        public void ToProblemDetails_Generic_SetsExtensionsCorrectly()
        {
            var error = Error.NotFound("Test.NotFound", "Item not found");
            var result = Result.Failure<string>(error);

            var actionResult = result.ToProblemDetails();

            var objectResult = Assert.IsType<ObjectResult>(actionResult);
            var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);

            Assert.NotNull(problemDetails.Extensions);
            Assert.Equal(false, problemDetails.Extensions["isSuccess"]);
            Assert.Equal(true, problemDetails.Extensions["isFailure"]);
            Assert.Null(problemDetails.Extensions["value"]);
        }

        // Integration Tests
        [Fact]
        public void ToProblemDetails_ProducesSerializableResult()
        {
            var error = Error.Conflict("Test.Conflict", "Resource conflict");
            var result = Result.Failure(error);

            var actionResult = result.ToProblemDetails();
            var objectResult = Assert.IsType<ObjectResult>(actionResult);
            var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            // Should be serializable without throwing
            var json = JsonSerializer.Serialize(problemDetails);
            Assert.NotNull(json);
            Assert.Contains("Resource conflict", json);
            Assert.Contains("409", json);
        }

        [Fact]
        public void ToProblemDetails_WorksWithComplexError()
        {
            var errorMessage = "Complex error with special characters: àáâãäå æç èéêë ìíîï ñ òóôõö ùúûü ýÿ";
            var error = Error.Failure("Complex.Error", errorMessage);
            var result = Result.Failure(error);

            var actionResult = result.ToProblemDetails();

            var objectResult = Assert.IsType<ObjectResult>(actionResult);
            var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);

            Assert.Equal(errorMessage, problemDetails.Detail);
            Assert.Equal(500, problemDetails.Status);
        }

        // Helper Methods
        private static Error CreateErrorOfType(ErrorType errorType) => errorType switch
        {
            ErrorType.Validation => Error.Validation("Test.Validation", "Validation error"),
            ErrorType.NotFound => Error.NotFound("Test.NotFound", "Not found error"),
            ErrorType.Conflict => Error.Conflict("Test.Conflict", "Conflict error"),
            ErrorType.RateLimit => Error.RateLimit("Test.RateLimit", "Rate limit error"),
            ErrorType.Failure => Error.Failure("Test.Failure", "General failure"),
            _ => throw new ArgumentOutOfRangeException(nameof(errorType))
        };

        private static DefaultHttpContext CreateMockHttpContext(string method, string path, string queryString)
        {
            var context = new DefaultHttpContext();
            context.Request.Method = method;
            context.Request.Path = path;
            context.Request.QueryString = new QueryString(queryString);
            return context;
        }
    }
}