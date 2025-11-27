using api_integration.SharedKernel.src;

namespace api_integration.UnitTests.src.SharedKernelTests
{
    public class ResultTests
    {
        // Success Result Tests
        [Fact]
        public void Success_CreatesSuccessfulResult()
        {
            var result = Result.Success();

            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(Error.None, result.Error);
        }

        [Fact]
        public void Success_Generic_CreatesSuccessfulResultWithValue()
        {
           
            var value = "test value";

            var result = Result.Success(value);

            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(value, result.Value);
            Assert.Equal(Error.None, result.Error);
        }

        [Fact]
        public void Success_WithNullValue_CreatesSuccessfulResult()
        {
            var result = Result.Success<string?>(null);

            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }


        // Failure Result Tests
        [Fact]
        public void Failure_CreatesFailedResult()
        {
           
            var error = Error.Validation("Test.Error", "Test error");

            var result = Result.Failure(error);

            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(error, result.Error);
        }

        [Fact]
        public void Failure_Generic_CreatesFailedResultWithoutValue()
        {
           
            var error = Error.NotFound("Test.NotFound", "Item not found");

            var result = Result.Failure<string>(error);

            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(error, result.Error);
        }

        [Fact]
        public void Failure_Generic_AccessingValue_ThrowsException()
        {
           
            var error = Error.Validation("Test.Error", "Test error");
            var result = Result.Failure<string>(error);
 
            var exception = Assert.Throws<InvalidOperationException>(() => result.Value);
            Assert.Equal("The value of a failure result can't be accessed.", exception.Message);
        }


        // Constructor Validation Tests
        [Fact]
        public void Result_SuccessWithNonNoneError_ThrowsException()
        {
            var error = Error.Validation("Test.Error", "Test error");

            var exception = Assert.Throws<InvalidOperationException>(() => 
                new TestResult(true, error));
            Assert.Equal("A successful result cannot have an error.", exception.Message);
        }

        [Fact]
        public void Result_FailureWithNoneError_ThrowsException()
        { 
            var exception = Assert.Throws<InvalidOperationException>(() => 
                new TestResult(false, Error.None));
            Assert.Equal("A failure result must have an error.", exception.Message);
        }

        // Helper class to test protected constructor
        private class TestResult : Result
        {
            public TestResult(bool isSuccess, Error error) : base(isSuccess, error) { }
        }


        // Implicit Conversion Tests
        [Fact]
        public void Result_ImplicitConversionFromError_CreatesFailureResult()
        {
           
            var error = Error.NotFound("Test.NotFound", "Not found");

            Result result = error;

            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(error, result.Error);
        }

        [Fact]
        public void ResultT_ImplicitConversionFromValue_CreatesSuccessResult()
        {
           
            var value = "test value";

            Result<string> result = value;

            Assert.True(result.IsSuccess);
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void ResultT_ImplicitConversionFromNullValue_CreatesFailureResult()
        {
            Result<string> result = (string?)null;

            Assert.False(result.IsSuccess);
            Assert.Equal(Error.NullValue, result.Error);
        }

        [Fact]
        public void ResultT_ImplicitConversionFromError_CreatesFailureResult()
        {
           
            var error = Error.Validation("Test.Error", "Validation failed");

            Result<string> result = error;

            Assert.False(result.IsSuccess);
            Assert.Equal(error, result.Error);
        }


        //Value Type Tests
        [Fact]
        public void ResultT_WithValueType_WorksCorrectly()
        {
            var successResult = Result.Success(42);
            var failureResult = Result.Failure<int>(Error.NotFound("Test.NotFound", "Not found"));

            Assert.True(successResult.IsSuccess);
            Assert.Equal(42, successResult.Value);

            Assert.False(failureResult.IsSuccess);
            Assert.Throws<InvalidOperationException>(() => failureResult.Value);
        }

        [Fact]
        public void ResultT_WithComplexType_WorksCorrectly()
        {
           
            var testObject = new TestClass { Id = 1, Name = "Test" };

            var result = Result.Success(testObject);

            Assert.True(result.IsSuccess);
            Assert.Equal(testObject.Id, result.Value.Id);
            Assert.Equal(testObject.Name, result.Value.Name);
        }

        private class TestClass
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

    }
}