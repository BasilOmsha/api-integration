using api_integration.Application.src.Interfaces;
using api_integration.Application.src.Interfaces.IConnConfig;
using api_integration.Presenter.API.src.ConnConfig;
using api_integration.Presenter.API.src.Services;
using Microsoft.Extensions.Options;

namespace api_integration.Presenter.API.src
{
    public static class DependencyInjections
    {
        public static IServiceCollection DI(this IServiceCollection services, IConfiguration configuration)
        {
            AddConfiguration(services, configuration);
            AddServices(services, configuration);

            return services;
        }

        //Fingrid configurations
        private static void AddConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FingridApiConfigurations>(options =>
            {
                // Use class defaults, then bind from appsettings.json if section exists
                configuration.GetSection(FingridApiConfigurations.SectionName).Bind(options);

                var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";

                if (environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
                {
                    // Override from User Secrets
                    // options.ApiKey = configuration["ConnStr:fingrid"];
                    options.ApiKey = configuration.GetConnectionString("fingrid");
                }
                else
                {
                    // Override from Azure Key Vault
                     options.ApiKey = configuration.GetConnectionString("fingrid");
                }
            });

             services.AddOptions<FingridApiConfigurations>()
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton<IFingridApiConfiguration>(provider =>
                provider.GetRequiredService<IOptions<FingridApiConfigurations>>().Value);
        }
        public static void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IFingridService, FingridService>((serviceProvider, client) =>
            {
                var config = serviceProvider.GetRequiredService<IFingridApiConfiguration>();
                if (string.IsNullOrWhiteSpace(config.BaseUrl))
                {
                    throw new InvalidOperationException("FingridApi BaseUrl is not configured. Check your appsettings.json and ensure the BaseUrl is set.");
                }
                client.BaseAddress = new Uri(config.BaseUrl);
                client.DefaultRequestHeaders.Add("X-API-Key", config.ApiKey);
                client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
            });
        }
    }
}