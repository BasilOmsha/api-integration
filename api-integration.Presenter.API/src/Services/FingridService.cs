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
                return FingridErrors.NetworkError;
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

         public async Task<Result<DatasetDataExternalApiResDto>> GetDataByDatasetIdAsync(int datasetId, DateTime startTime, DateTime endTime)
        {
            var stopwatch = Stopwatch.StartNew();

            if (datasetId <= 0) return FingridErrors.ZeroOrNegative;

            try
            {
                string startFormatted = startTime.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
                string endFormatted = endTime.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);

                var allData = new List<DataPointDto>();
                int currentPage = 1;
                int? lastPage = null;

                do
                {
                    string url = $"{datasetId}/data?startTime={startFormatted}&endTime={endFormatted}&format=json&pageSize=20000&page={currentPage}";

                    _logger.LogInformation("Fetching data for dataset {datasetId}, page {page}", datasetId, currentPage);
                    var response = await _httpClient.GetAsync(url);

                    _logger.LogInformation("Response: {StatusCode} in {ElapsedMs}ms", response.StatusCode, stopwatch.ElapsedMilliseconds);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError("External API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                        return response.StatusCode switch
                        {
                            HttpStatusCode.NotFound => FingridErrors.DatasetNotFound,
                            HttpStatusCode.Unauthorized => FingridErrors.UnauthorizedAccess,
                            HttpStatusCode.TooManyRequests => FingridErrors.RateLimitExceeded,
                            _ => FingridErrors.ExternalApiError
                        };
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    var pageResult = JsonSerializer.Deserialize<DatasetDataExternalApiResDto>(json, new JsonSerializerOptions());

                    if (pageResult == null) return FingridErrors.DeserializationFailure;

                    allData.AddRange(pageResult.Data);

                    // Log pagination info for debugging
                    _logger.LogInformation(
                        "Dataset {datasetId} page {page}/{lastPage}: received {count} items (total: {total})",
                        datasetId, pageResult.Pagination.CurrentPage, pageResult.Pagination.LastPage,
                        pageResult.Data.Count, pageResult.Pagination.Total);

                    lastPage = pageResult.Pagination.LastPage;
                    currentPage++;
                }
                while (currentPage <= (lastPage ?? 1));

                stopwatch.Stop();

                if (allData.Count == 0)
                {
                    _logger.LogWarning(
                        "External API returned no data for dataset {datasetId} between {start} and {end}",
                        datasetId, startFormatted, endFormatted);
                }

                return new DatasetDataExternalApiResDto { Data = allData };
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                stopwatch.Stop();
                return FingridErrors.RequestTimeout;
            }
            catch (HttpRequestException ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Network error for dataset {datasetId}", datasetId);
                return FingridErrors.NetworkError;
            }
            catch (JsonException)
            {
                stopwatch.Stop();
                return FingridErrors.DeserializationFailure;
            }
            catch (Exception)
            {
                stopwatch.Stop();
                return FingridErrors.ExternalApiException;
            }
        }
    }
}