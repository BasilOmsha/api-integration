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
        private readonly IFingridDataService _fingridDataService;
        private static readonly Regex DatasetIdRegex = new(@"^[1-9]\d{0,2}$", RegexOptions.Compiled); // 1-999
        private readonly IFingridMetaDataService _fingridMetaDataService;

        public FingridMetaDataController(IFingridService fingridService, IFingridDataService fingridDataService, IFingridMetaDataService fingridMetaDataService)
        {
             _fingridService = fingridService ?? throw new ArgumentNullException(nameof(fingridService));
            _fingridMetaDataService = fingridMetaDataService ?? throw new ArgumentNullException(nameof(fingridMetaDataService));
             _fingridDataService = fingridDataService ?? throw new ArgumentNullException(nameof(fingridDataService));
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
        // public async Task<ActionResult<MetaDataExternalApiResDto>> GetMetaDataById([FromRoute] string? datasetId)
        public async Task<ActionResult<MetaDataReadDto>> GetMetaDataById([FromRoute] string? datasetId)
        {
            if (string.IsNullOrWhiteSpace(datasetId))
            {
                var nullError = Result.Failure<MetaDataReadDto>(FingridErrors.DatasetIdRequired);
                return nullError.ToProblemDetails(HttpContext);
            }

            var decodedId = HttpUtility.UrlDecode(datasetId);
            var trimmedId = decodedId.Trim();

            if (!DatasetIdRegex.IsMatch(trimmedId) || !int.TryParse(trimmedId, out var parsedMetaId))
            {
                var formatError = Result.Failure<MetaDataReadDto>(FingridErrors.InvalidFormat);
                return formatError.ToProblemDetails(HttpContext);
            }

            var result = await _fingridMetaDataService.GetMetaDataAsync(parsedMetaId);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails(HttpContext);
        }

        /// <summary>
        /// Gets Fingrid time-series data by dataset ID, checks cache first
        /// </summary>
        /// <param name="datasetId">The dataset identifier (must be a positive integer between 1 and 999)</param>
        /// <param name="startTime">Start time in UTC (ISO 8601)</param>
        /// <param name="endTime">End time in UTC (ISO 8601)</param>
        [HttpGet("{datasetId?}/data")]
        [EnableRateLimiting("fingrid-external-api")]
        [ProducesResponseType(typeof(List<CachedDataPointReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CachedDataPointReadDto>>> GetDataByDatasetId(
            [FromRoute] string? datasetId,
            [FromQuery] DateTime? startTime,
            [FromQuery] DateTime? endTime)
        {
            if (string.IsNullOrWhiteSpace(datasetId))
            {
                var nullError = Result.Failure<List<CachedDataPointReadDto>>(FingridErrors.DatasetIdRequired);
                return nullError.ToProblemDetails(HttpContext);
            }

            var decodedId = HttpUtility.UrlDecode(datasetId);
            var trimmedId = decodedId.Trim();

            if (!DatasetIdRegex.IsMatch(trimmedId) || !int.TryParse(trimmedId, out var parsedDataId))
            {
                var formatError = Result.Failure<List<CachedDataPointReadDto>>(FingridErrors.InvalidFormat);
                return formatError.ToProblemDetails(HttpContext);
            }

            if (startTime == null || endTime == null)
            {
                var dateError = Result.Failure<List<CachedDataPointReadDto>>(FingridErrors.DateRangeRequired);
                return dateError.ToProblemDetails(HttpContext);
            }

            if (startTime >= endTime)
            {
                var rangeError = Result.Failure<List<CachedDataPointReadDto>>(FingridErrors.InvalidDateRange);
                return rangeError.ToProblemDetails(HttpContext);
            }

            var start = DateTime.SpecifyKind(startTime.Value, DateTimeKind.Utc);
            var end = DateTime.SpecifyKind(endTime.Value, DateTimeKind.Utc);

            var result = await _fingridDataService.GetDataAsync(parsedDataId, start, end);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails(HttpContext);
        }
    }
}