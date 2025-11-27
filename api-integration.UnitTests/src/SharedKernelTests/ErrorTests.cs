using api_integration.SharedKernel.src;

namespace api_integration.UnitTests.src.SharedKernelTests
{
    public class ErrorTests
    {
        //Factory Method Tests
        [Fact]
        public void NotFound_CreatesErrorWithCorrectProperties()
        {
            var code = "Test.NotFound";
            var description = "Test not found";

            var error = Error.NotFound(code, description);
            Assert.Equal(code, error.Code);
            Assert.Equal(description, error.Description);
            Assert.Equal(ErrorType.NotFound, error.Type);
        }

        [Fact]
        public void Validation_CreatesErrorWithCorrectProperties()
        {
            var code = "Test.Validation";
            var description = "Invalid input";

            var error = Error.Validation(code, description);
            Assert.Equal(code, error.Code);
            Assert.Equal(description, error.Description);
            Assert.Equal(ErrorType.Validation, error.Type);
        }

        [Fact]
        public void Conflict_CreatesErrorWithCorrectProperties()
        {
            var code = "Test.Conflict";
            var description = "Resource conflict";

            var error = Error.Conflict(code, description);
            Assert.Equal(code, error.Code);
            Assert.Equal(description, error.Description);
            Assert.Equal(ErrorType.Conflict, error.Type);
        }

        [Fact]
        public void RateLimit_CreatesErrorWithCorrectProperties()
        {
            var code = "Test.RateLimit";
            var description = "Too many requests";

            var error = Error.RateLimit(code, description);
            Assert.Equal(code, error.Code);
            Assert.Equal(description, error.Description);
            Assert.Equal(ErrorType.RateLimit, error.Type);
        }

        [Fact]
        public void Failure_CreatesErrorWithCorrectProperties()
        {
            var code = "Test.Failure";
            var description = "General failure";

            var error = Error.Failure(code, description);
            Assert.Equal(code, error.Code);
            Assert.Equal(description, error.Description);
            Assert.Equal(ErrorType.Failure, error.Type);
        }

        [Fact]
        public void None_HasExpectedProperties()
        {
            Assert.Equal(string.Empty, Error.None.Code);
            Assert.Equal(string.Empty, Error.None.Description);
            Assert.Equal(ErrorType.Failure, Error.None.Type);
        }

        [Fact]
        public void NullValue_HasExpectedProperties()
        {
            Assert.Equal("Error.NullValue", Error.NullValue.Code);
            Assert.Equal("Null value was provided", Error.NullValue.Description);
            Assert.Equal(ErrorType.Failure, Error.NullValue.Type);
        }

        // Edge Cases
        [Fact]
        public void Error_WithNullDescription_SetsEmptyString()
        {
            var error = Error.Validation("Test.Code", null!);
            Assert.Equal(string.Empty, error.Description);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ValidCode")]
        public void Error_AcceptsVariousCodeFormats(string code)
        {
            var error = Error.Validation(code, "Description");
            Assert.Equal(code, error.Code);
        }

        //Equality Tests
        [Fact]
        public void Errors_WithSameProperties_AreEqual()
        {
            var error1 = Error.NotFound("Test.Code", "Test description");
            var error2 = Error.NotFound("Test.Code", "Test description");

            Assert.Equal(error1, error2);
            Assert.True(error1 == error2);
            Assert.False(error1 != error2);
        }

        [Fact]
        public void Errors_WithDifferentProperties_AreNotEqual()
        {
            var error1 = Error.NotFound("Test.Code1", "Description");
            var error2 = Error.NotFound("Test.Code2", "Description");

            Assert.NotEqual(error1, error2);
            Assert.False(error1 == error2);
            Assert.True(error1 != error2);
        }
    }
}