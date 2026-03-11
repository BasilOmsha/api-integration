using api_integration.Application.src.Interfaces;
using api_integration.Application.src.Interfaces.IConnConfig;
using api_integration.Domain.src.Interfaces.Repositories;
using api_integration.Infrastructure.src.Data;
using api_integration.Infrastructure.src.Repositories;
using api_integration.Presenter.API.src.ConnConfig;
using api_integration.Presenter.API.src.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Polly;

namespace api_integration.Presenter.API.src
{
    public static class DependencyInjections
    {
        public static IServiceCollection DI(this IServiceCollection services, IConfiguration configuration)
        {
            AddConfiguration(services, configuration);
            AddServices(services, configuration);
            AddDatabase(services, configuration);
            AddRepositories(services);

            return services;
        }

        //Fingrid configurations
        private static void AddConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FingridApiConfigurations>(options =>
            {
                // Use class defaults, then bind from appsettings.json if section exists
                configuration.GetSection(FingridApiConfigurations.SectionName).Bind(options);

                // Override from User Secrets
                options.ApiKey = configuration.GetConnectionString("fingrid");

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
                // Let Polly handle timeouts (inner per-attempt + outer overall).
                // HttpClient.Timeout would cancel the entire retry chain prematurely.
                client.Timeout = Timeout.InfiniteTimeSpan;
            })
            // Outer policy: overall timeout for the entire operation (all retries + waits)
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(
                TimeSpan.FromMinutes(3)))
            // Middle policy: retry on 429 / transient errors
            .AddTransientHttpErrorPolicy(policy =>
            {
                return policy.OrResult(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    .WaitAndRetryAsync(
                        retryCount: 3,
                        sleepDurationProvider: _ => TimeSpan.FromSeconds(13)
                    );
            })
            // Inner policy: per-attempt timeout (each individual HTTP request)
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(
                TimeSpan.FromSeconds(30)));

            services.AddScoped<IFingridDataService, FingridDataService>();
            services.AddScoped<IFingridMetaDataService, FingridMetaDataService>();

            services.AddHostedService<CacheCleanupBackgroundService>();
        }
        private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("postgres")));
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IMetaDataRepository, MetaDataRepository>();
            services.AddScoped<ICachedDataPointRepository, CachedDataPointRepository>();
        }
    }
}