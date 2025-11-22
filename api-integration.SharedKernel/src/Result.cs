using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace api_integration.SharedKernel.src
{

    // public class Result<T>
    // {
    //     public T? Value { get; init; }
    //     public bool IsSuccess {get;}
    //     public bool IsFailure => ! IsSuccess;
    //     public Error Error {get;}

    //     private Result(bool isSuccess, T? value, Error error)
    //     {
    //          if (isSuccess && error != Error.None)
    //         {
    //             throw new InvalidOperationException("A successful result cannot have an error.");
    //         }
            
    //         if (!isSuccess && error == Error.None)
    //         {
    //             throw new InvalidOperationException("A failure result must have an error.");
    //         }

    //         IsSuccess = isSuccess;
    //         Value = value;
    //         Error = error;
    //     }

    //     public static Result<T> Success(T value) => new(true, value, Error.None);
    //     public static Result<T> Failure(Error error) => new(false, default, error);

    //     // implicit allows to return the error or value directly without calling Success or Failure
    //     public static implicit operator Result<T>(T value) => Success(value);
    //     public static implicit operator Result<T>(Error error) => Failure(error);
    // }
    public class Result
    {
        protected internal Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None)
            {
                throw new InvalidOperationException("A successful result cannot have an error.");
            }
            
            if (!isSuccess && error == Error.None)
            {
                throw new InvalidOperationException("A failure result must have an error.");
            }

            IsSuccess = isSuccess;
            Error = error;
        }
        public bool IsSuccess { get; }
        public bool IsFailure => ! IsSuccess;
        public Error Error { get; }

        public static Result Success() => new(true, Error.None);
        public static Result<T> Success<T>(T value) => new(value, true,  Error.None);
        public static Result Failure(Error error) => new(false, error);
        public static Result<T> Failure<T>(Error error) => new(default, false, error);

        // implicit allows to return the error or value directly without calling Failure
         public static implicit operator Result(Error error) => Failure(error);

    }

    public class Result<T> : Result
    {
        private readonly T? _value;

        protected internal Result(T? value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        [NotNull]
        // [JsonIgnore]
        public T Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failure result can't be accessed.");
        // public T? Value => IsSuccess? _value : default;
        // [JsonPropertyName("value")]
        // public T? SafeValue => IsSuccess ? _value : default;
        
        public static implicit operator Result<T>(T? value) =>
            value is not null ? Success(value) : Failure<T>(Error.NullValue);

        public static implicit operator Result<T>(Error error) => Failure<T>(error);
    }

}