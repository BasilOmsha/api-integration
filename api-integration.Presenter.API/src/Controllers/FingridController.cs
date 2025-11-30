using System.Text.RegularExpressions;
using System.Web;
using api_integration.Application.src.DTOs;
using api_integration.Application.src.Interfaces;
using api_integration.Domain.src.Entities.Fingrid;
using api_integration.Presenter.API.src.Extensions;
using api_integration.SharedKernel.src;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace api_integration.Presenter.API.src.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class FingridMetaDataController : ControllerBase
    {
        private readonly IFingridService _fingridService;
        private static readonly Regex DatasetIdRegex = new(@"^[1-9]\d{0,2}$", RegexOptions.Compiled); // 1-999

        public FingridMetaDataController(IFingridService fingridService)
        {
             _fingridService = fingridService ?? throw new ArgumentNullException(nameof(fingridService));
        }
        
        /// <summary>
        /// Gets Fingrid metadata by dataset ID from the Fingrid external API
        /// </summary>
        /// <param name="datasetId">The dataset identifier (must be a positive integer between 1 and 999)</param>
        /// <returns>Metadata for the specified dataset</returns>
        [HttpGet("{datasetId?}")]
        [EnableRateLimiting("fingrid-external-api")]
        [ProducesResponseType(typeof(MetaDataExternalApiResDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MetaDataExternalApiResDto>> GetMetaDataById([FromRoute] string? datasetId)
        {
            if (string.IsNullOrWhiteSpace(datasetId))
            {
                var nullError = Result.Failure<MetaDataExternalApiResDto>(FingridErrors.DatasetIdRequired);
                return nullError.ToProblemDetails(HttpContext);
            }
            var decodedId = HttpUtility.UrlDecode(datasetId);
             var trimmedId = decodedId.Trim();

            if (!DatasetIdRegex.IsMatch(trimmedId))
            {
                var formatError = Result.Failure<MetaDataExternalApiResDto>(
                    FingridErrors.InvalidFormat);
                return formatError.ToProblemDetails(HttpContext);
            }

            var result = await _fingridService.GetExternalMetaDataByIdAsync(int.Parse(trimmedId));
            return result.IsSuccess ? Ok(result) : result.ToProblemDetails(HttpContext);
        }
    }
}