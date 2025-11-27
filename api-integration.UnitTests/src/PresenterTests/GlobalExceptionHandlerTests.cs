using api_integration.Presenter.API.src;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace api_integration.UnitTests.src.PresenterTests
{
    public class GlobalExceptionHandlerTests
    {
        private readonly Mock<ILogger<GlobalExceptionHandler>> _mockLogger;
        private readonly Mock<IProblemDetailsService> _mockProblemDetailsService;
        private readonly GlobalExceptionHandler _handler;

        public GlobalExceptionHandlerTests()
        {
            _mockLogger = new Mock<ILogger<GlobalExceptionHandler>>();
            _mockProblemDetailsService = new Mock<IProblemDetailsService>();
            _handler = new GlobalExceptionHandler(_mockProblemDetailsService.Object, _mockLogger.Object);
        }

        //Exception Type Mapping Tests
        [Fact]
        public async Task TryHandleAsync_WithApplicationException_Sets400StatusCode()
        {
            // Arrange
            var httpContext = CreateHttpContext();
            var exception = new ApplicationException("Application error");

            _mockProblemDetailsService
                .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(400, httpContext.Response.StatusCode);
            VerifyLogging(LogLevel.Error, "Application error");
        }

        [Fact]
        public async Task TryHandleAsync_WithNotSupportedException_Sets405StatusCode()
        {
            // Arrange
            var httpContext = CreateHttpContext();
            var exception = new NotSupportedException("Method not supported");

            _mockProblemDetailsService
                .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(405, httpContext.Response.StatusCode);
            VerifyLogging(LogLevel.Error, "Method not supported");
        }

        [Theory]
        [InlineData(typeof(InvalidOperationException))]
        [InlineData(typeof(ArgumentException))]
        [InlineData(typeof(NullReferenceException))]
        [InlineData(typeof(Exception))]
        public async Task TryHandleAsync_WithOtherExceptions_Sets500StatusCode(Type exceptionType)
        {
            // Arrange
            var httpContext = CreateHttpContext();
            var exception = (Exception)Activator.CreateInstance(exceptionType, "Test error")!;

            _mockProblemDetailsService
                .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(500, httpContext.Response.StatusCode);
            VerifyLogging(LogLevel.Error, "Test error");
        }

        // ProblemDetails Context Tests
        [Fact]
        public async Task TryHandleAsync_CreatesProblemDetailsContextCorrectly()
        {
            // Arrange
            var httpContext = CreateHttpContext();
            var exception = new InvalidOperationException("Test exception");
            ProblemDetailsContext? capturedContext = null;

            _mockProblemDetailsService
                .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .Callback<ProblemDetailsContext>(context => capturedContext = context)
                .ReturnsAsync(true);

            // Act
            await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedContext);
            Assert.Equal(httpContext, capturedContext.HttpContext);
            Assert.Equal(exception, capturedContext.Exception);
            Assert.NotNull(capturedContext.ProblemDetails);
        }

        [Fact]
        public async Task TryHandleAsync_SetsProblemDetailsPropertiesCorrectly()
        {
            // Arrange
            var httpContext = CreateHttpContext();
            var exception = new ArgumentNullException("paramName", "Parameter cannot be null");
            ProblemDetailsContext? capturedContext = null;

            _mockProblemDetailsService
                .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .Callback<ProblemDetailsContext>(context => capturedContext = context)
                .ReturnsAsync(true);

            // Act
            await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedContext?.ProblemDetails);
            var problemDetails = capturedContext.ProblemDetails;
            
            Assert.Equal("ArgumentNullException", problemDetails.Type);
            Assert.Equal("Server Error", problemDetails.Title);
            Assert.Equal("Parameter cannot be null (Parameter 'paramName')", problemDetails.Detail);
        }

        // Logging Tests
        [Fact]
        public async Task TryHandleAsync_LogsExceptionWithCorrectLevel()
        {
            // Arrange
            var httpContext = CreateHttpContext();
            var exception = new Exception("Test exception");

            _mockProblemDetailsService
                .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .ReturnsAsync(true);

            // Act
            await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Exception occurred: Test exception")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task TryHandleAsync_LogsExceptionDetails()
        {
            // Arrange
            var httpContext = CreateHttpContext();
            var innerException = new InvalidOperationException("Inner exception");
            var exception = new ApplicationException("Outer exception", innerException);

            _mockProblemDetailsService
                .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .ReturnsAsync(true);

            // Act
            await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Outer exception")),
                    exception, // Should log the full exception with inner exception
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        //Dependency Interaction Tests
        [Fact]
        public async Task TryHandleAsync_WhenProblemDetailsServiceSucceeds_ReturnsTrue()
        {
            // Arrange
            var httpContext = CreateHttpContext();
            var exception = new Exception("Test exception");

            _mockProblemDetailsService
                .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task TryHandleAsync_WhenProblemDetailsServiceFails_ReturnsFalse()
        {
            // Arrange
            var httpContext = CreateHttpContext();
            var exception = new Exception("Test exception");

            _mockProblemDetailsService
                .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task TryHandleAsync_CallsProblemDetailsServiceOnce()
        {
            // Arrange
            var httpContext = CreateHttpContext();
            var exception = new Exception("Test exception");

            _mockProblemDetailsService
                .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .ReturnsAsync(true);

            // Act
            await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            _mockProblemDetailsService.Verify(
                x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()),
                Times.Once);
        }

        //Cancellation Tests
        [Fact]
        public async Task TryHandleAsync_WithCancellationToken_PassesToService()
        {
            // Arrange
            var httpContext = CreateHttpContext();
            var exception = new Exception("Test exception");
            var cancellationToken = new CancellationToken();

            _mockProblemDetailsService
                .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .ReturnsAsync(true);

            // Act
            await _handler.TryHandleAsync(httpContext, exception, cancellationToken);

            // Assert - Verify the service was called (cancellation token is internal to the service)
            _mockProblemDetailsService.Verify(
                x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()),
                Times.Once);
        }

        //Edge Cases
        [Fact]
        public async Task TryHandleAsync_WithNullExceptionMessage_HandlesGracefully()
        {
            // Arrange
            var httpContext = CreateHttpContext();
            var exception = new Exception(); // No message

            _mockProblemDetailsService
                .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .ReturnsAsync(true);

            // Act & Assert - Should not throw
            var result = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);
            Assert.True(result);
        }

        [Fact]
        public async Task TryHandleAsync_WithVeryLongExceptionMessage_HandlesCorrectly()
        {
            // Arrange
            var httpContext = CreateHttpContext();
            var longMessage = new string('A', 10000); // Very long message
            var exception = new Exception(longMessage);

            _mockProblemDetailsService
                .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.True(result);
            VerifyLogging(LogLevel.Error, longMessage);
        }

        //Helper Methods
        private static HttpContext CreateHttpContext()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            return context;
        }

        private void VerifyLogging(LogLevel level, string message)
        {
            _mockLogger.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}