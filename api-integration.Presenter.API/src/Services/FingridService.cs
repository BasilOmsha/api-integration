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

        // public async Task<Error<MetaDataExternalApiResDto>> GetExternalMetaDataAsync()
        //  public async Task<Result<MetaDataExternalApiResDto>> GetExternalMetaDataAsync()
        // {
            
        //     var stopwatch = Stopwatch.StartNew();
            
        //     try
        //     {
        //         _logger.LogInformation("Making request to dataset {datasetId}", datasetId);

        //         var response = await _httpClient.GetAsync($"{datasetId}/");
        //         stopwatch.Stop();

        //         _logger.LogInformation("Response: {StatusCode} in {ElapsedMs}ms", 

        //         response.StatusCode, stopwatch.ElapsedMilliseconds);
        //         if (!response.IsSuccessStatusCode)
        //         {
        //             var errorContent = await response.Content.ReadAsStringAsync();

        //             if (response.StatusCode == HttpStatusCode.NotFound)
        //             {
        //                 _logger.LogWarning("Dataset not found: {datasetId}", datasetId);
        //                 // throw new InvalidOperationException($"Dataset not found");
        //                 // return new Error("Dataset.NotFound", "The requested dataset was not found.");
        //                 // return Result<MetaDataExternalApiResDto>.Failure(new Error("Dataset.NotFound", "The requested dataset was not found."));
        //                 return FingridErrors.DatasetNotFound(datasetId);
        //             }
        //             _logger.LogError("External API returned error: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);
        //             // throw new HttpRequestException(
        //             //     $"External API returned error: {response.StatusCode}");
        //             // return Result<MetaDataExternalApiResDto>.Failure(new Error("ExternalApi.Error", $"External API returned error: {response.StatusCode}"));
        //             return FingridErrors.ExternalApiError;
        //         }

        //         var json = await response.Content.ReadAsStringAsync();
                
                
        //         var result = JsonSerializer.Deserialize<MetaDataExternalApiResDto>(json, new JsonSerializerOptions());

        //         if (result == null)
        //         {
        //             _logger.LogError("Failed to deserialize response from external API");
        //             // throw new InvalidOperationException($"Failed to deserialize response");
        //             // return Result<MetaDataExternalApiResDto>.Failure(new Error("Deserialization.Failure", "Failed to deserialize response from external API."));
        //             // return Result<MetaDataExternalApiResDto>.Failure(FingridErrors.DeserializationFailure);
        //             return FingridErrors.DeserializationFailure;
        //         }

        //         // return Result<MetaDataExternalApiResDto>.Success(result);
        //         _logger.LogInformation("Successfully retrieved metadata for dataset {datasetId}", datasetId);
        //         return result;
        //     }
        //     catch (Exception)
        //     {
        //         stopwatch.Stop();
        //         _logger.LogError("Exception occurred while retrieving metadata for dataset {datasetId} in {ElapsedMs}ms", datasetId, stopwatch.ElapsedMilliseconds);
        //         // throw;
        //         // return Result<MetaDataExternalApiResDto>.Failure(new Error("ExternalApi.Exception", ex.Message));
        //         return FingridErrors.ExternalApiException;
        //     }
        // }
    }
}