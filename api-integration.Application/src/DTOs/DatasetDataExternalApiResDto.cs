using System.Text.Json.Serialization;

namespace api_integration.Application.src.DTOs
{
    public class DatasetDataExternalApiResDto
    {
        [JsonPropertyName("data")]
        public List<DataPointDto> Data { get; set; } = [];

        [JsonPropertyName("pagination")]
        public PaginationDto Pagination { get; set; } = new();
    }
}