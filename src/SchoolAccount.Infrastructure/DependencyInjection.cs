using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Infrastructure.Collect.CensusStatuses;
using SchoolAccount.Infrastructure.Config;
using SchoolAccount.Infrastructure.Time;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddServices().AddHealthChecks();
        services.AddCollectApiClient(configuration);

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }

    private static void AddCollectApiClient(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddOptions<CommonApiConfig>()
            .Bind(configuration.GetSection(CommonApiConfig.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient<ICollectApiClient, CollectApiClient>(
            (serviceProvider, client) =>
            {
                var config = serviceProvider.GetRequiredService<IOptions<CommonApiConfig>>().Value;
                client.BaseAddress = new Uri(config.CollectApiUrl);
            }
        );
    }
}
