using System.ComponentModel.DataAnnotations;
using api_integration.Application.src.Interfaces.IConnConfig;

namespace api_integration.Presenter.API.src.ConnConfig
{

    // made this DTO for validation on runtime to check that the required settings for connection are set. Check the appSettings
    public class FingridApiConfigurations : IFingridApiConfiguration
    {
        public const string SectionName = "FingridApi"; // used to identify section in appSettings

        [Required]
        [Url]
        public string? BaseUrl { get; set; }

        [Required]
        public string? ApiKey { get; set; }

        [Range(1, 300)]
        public int TimeoutSeconds { get; set; } = 30;

        [Range(0, 10)]
        public int MaxRetryAttempts { get; set; } = 2;

        [Range(100, 10000)]
        public int RetryDelayMs { get; set; } = 1000;
    }
}