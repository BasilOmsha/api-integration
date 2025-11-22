namespace api_integration.SharedKernel.src
{
    public record Error
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
        public static readonly Error NullValue = new("Error.NullValue", "Null value was provided",  ErrorType.Failure);

        private Error(string code, string? description, ErrorType errorType)
        {
            Code = code;
            Description = description  ?? string.Empty;
            Type = errorType;
        }

        public string Code { get; }
        public string Description { get; }
        public ErrorType Type { get; }

        // Expose factory methods
        public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);
        public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);
        public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);
        public static Error RateLimit(string code, string description) => new(code, description, ErrorType.RateLimit);
        public static Error Failure(string code, string description) => new(code, description, ErrorType.Failure);

    }

    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        RateLimit = 4
    }

}

