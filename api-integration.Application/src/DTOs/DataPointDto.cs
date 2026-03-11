using System.Text.Json.Serialization;

namespace api_integration.Application.src.DTOs
{
    public class DataPointDto
    {
        [JsonPropertyName("datasetId")]
        public int DatasetId { get; set; }

        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("endTime")]
        public DateTime EndTime { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }
    }
}