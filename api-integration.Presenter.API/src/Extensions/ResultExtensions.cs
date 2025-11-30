using api_integration.SharedKernel.src;
using Microsoft.AspNetCore.Mvc;

namespace api_integration.Presenter.API.src.Extensions
{   
    public static class ResultExtensions
    {
        /// <summary>
        /// Converts a failed Result to ProblemDetails ActionResult
        /// </summary>
        public static ActionResult ToProblemDetails(this Result result, HttpContext? httpContext = null)
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException("Can't convert success result to problem");
            }

            var problemDetails = new ProblemDetails
            {
                Status = GetStatusCode(result.Error.Type),
                Title = GetTitle(result.Error.Type),
                Type = GetType(result.Error.Type),
                Detail = result.Error.Description,
                Instance =  GetInstance(httpContext),
                Extensions = new Dictionary<string, object?>
                {
                    { "isSuccess", result.IsSuccess },
                    { "isFailure", result.IsFailure},
                    { "errors", new[] { result.Error } },
                    { "value", null }
                }
            };

            return new ObjectResult(problemDetails)
            {
                StatusCode = GetStatusCode(result.Error.Type)
            };
        }

        /// <summary>
        /// Converts a failed Result&lt;T&gt; to ProblemDetails ActionResult
        /// </summary>
        public static ActionResult ToProblemDetails<T>(this Result<T> result, HttpContext? httpContext = null)
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException("Can't convert success result to problem");
            }

            // Delegate to the base Result method
            return ((Result)result).ToProblemDetails(httpContext);
        }

        private static string GetInstance(HttpContext? httpContext)
        {
            if (httpContext == null)
                return string.Empty;

            var request = httpContext.Request;
            return $"{request.Method} {request.Path} {request.QueryString}";
        }

        private static int GetStatusCode(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.RateLimit => StatusCodes.Status429TooManyRequests,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

        private static string GetTitle(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => "Bad Request",
                ErrorType.NotFound => "Not Found",
                ErrorType.Conflict => "Conflict",
                ErrorType.RateLimit => "Rate Limit Exceeded",
                ErrorType.Unauthorized => "Unauthorized",
                _ => "Internal Server Error"
            };

        private static string GetType(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                ErrorType.RateLimit => "https://tools.ietf.org/html/rfc6585#section-4",
                ErrorType.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
                _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };
    }
}