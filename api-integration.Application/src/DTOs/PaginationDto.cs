using System.Text.Json.Serialization;

namespace api_integration.Application.src.DTOs
{
    public class PaginationDto
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("lastPage")]
        public int LastPage { get; set; }

        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("perPage")]
        public int PerPage { get; set; }

        [JsonPropertyName("nextPage")]
        public int? NextPage { get; set; }

        [JsonPropertyName("prevPage")]
        public int? PrevPage { get; set; }

        [JsonPropertyName("from")]
        public int From { get; set; }

        [JsonPropertyName("to")]
        public int To { get; set; }
    }
}