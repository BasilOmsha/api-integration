using System.Diagnostics;
using System.Net;
using System.Text.Json;
using api_integration.Application.src.DTOs;
using api_integration.Application.src.Interfaces;
using api_integration.Domain.src.Entities.Fingrid;
using api_integration.SharedKernel.src;

namespace api_integration.Presenter.API.src.Services
{
    public class FingridService : IFingridService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FingridService> _logger;
        public FingridService(HttpClient httpClient, ILogger<FingridService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<MetaDataExternalApiResDto>> GetExternalMetaDataByIdAsync(int datasetId)
        {
            var stopwatch = Stopwatch.StartNew();

            if (datasetId <= 0)
            {
                return FingridErrors.ZeroOrNegative;
            }
            
            try
            {
                _logger.LogInformation("Making request to dataset {datasetId}", datasetId);

                var response = await _httpClient.GetAsync($"{datasetId}/");
                stopwatch.Stop();

                _logger.LogInformation("Response: {StatusCode} in {ElapsedMs}ms", 
                    response.StatusCode, stopwatch.ElapsedMilliseconds);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    return response.StatusCode switch
                    {
                        HttpStatusCode.NotFound => FingridErrors.DatasetNotFound,
                        HttpStatusCode.Unauthorized => FingridErrors.UnauthorizedAccess,
                        HttpStatusCode.TooManyRequests => FingridErrors.RateLimitExceeded,
                        _ => FingridErrors.ExternalApiError
                    };
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<MetaDataExternalApiResDto>(json, new JsonSerializerOptions());

                if (result == null)
                {
                    _logger.LogError("Failed to deserialize response from external API");
                    return FingridErrors.DeserializationFailure;
                }

                _logger.LogInformation("Successfully retrieved metadata for dataset {datasetId}", datasetId);
                return result;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                stopwatch.Stop();
                _logger.LogWarning("Request timeout for dataset {datasetId} after {ElapsedMs}ms", 
                    datasetId, stopwatch.ElapsedMilliseconds);
                return FingridErrors.RequestTimeout;
            }
            catch (HttpRequestException ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Network error for dataset {datasetId}", datasetId);
                return FingridErrors.NetworkError(ex.Message);
            }
            catch (JsonException ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "JSON parsing error for dataset {datasetId}", datasetId);
                return FingridErrors.DeserializationFailure;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Unexpected error for dataset {datasetId}", datasetId);
                return FingridErrors.ExternalApiException;
            }
        }
    }
}