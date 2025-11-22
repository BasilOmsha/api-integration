using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace api_integration.Presenter.API.src
{
    /// <summary>
    /// Global exception handler for unhandled exceptions
    /// </summary>
    internal sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetailsService;

        public GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            // httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.StatusCode = exception switch
            {
                ApplicationException => StatusCodes.Status400BadRequest,
                NotSupportedException => StatusCodes.Status405MethodNotAllowed,
                _ => StatusCodes.Status500InternalServerError
            };
                

            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext= httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    
                    Type = exception.GetType().Name,
                    Title = "Server Error",
                    Detail = exception.Message,
                }
            });
        }
    }
}