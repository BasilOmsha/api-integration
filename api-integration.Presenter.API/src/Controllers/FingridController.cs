using api_integration.Application.src.DTOs;
using api_integration.Application.src.Interfaces;
using api_integration.Domain.src.Entities.Fingrid;
using api_integration.Presenter.API.src.Extensions;
using api_integration.SharedKernel.src;
using Microsoft.AspNetCore.Mvc;

namespace api_integration.Presenter.API.src.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class FingridMetaDataController : ControllerBase
    {
        private readonly IFingridService _fingridService;
        public FingridMetaDataController(IFingridService fingridService)
        {
             _fingridService = fingridService ?? throw new ArgumentNullException(nameof(fingridService));
       }
        
        /// <summary>
        /// Gets Fingrid metadata by dataset ID from the Fingrid external API
        /// </summary>
        /// <param name="datasetId">The dataset identifier (must be a positive integer)</param>
        [HttpGet("{datasetId?}")]
        [ProducesResponseType(typeof(MetaDataExternalApiResDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MetaDataExternalApiResDto>> GetMetaDataById(string? datasetId)
        {
            if (string.IsNullOrWhiteSpace(datasetId))
            {
                var nullError = Result.Failure<MetaDataExternalApiResDto>(FingridErrors.DatasetIdRequired);
                return nullError.ToProblemDetails(HttpContext);
            }

            if (!int.TryParse(datasetId.Trim(), out int parsedDatasetId))
            {
                var formatError = Result.Failure<MetaDataExternalApiResDto>(
                    FingridErrors.InvalidFormat);
                return formatError.ToProblemDetails(HttpContext);
            }

            var result = await _fingridService.GetExternalMetaDataByIdAsync(parsedDatasetId);
            return result.IsSuccess ? Ok(result) : result.ToProblemDetails(HttpContext);
        }
    }
}