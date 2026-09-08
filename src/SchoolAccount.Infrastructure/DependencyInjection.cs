using Dfe.TramsDataApi.Client.Extensions;
using GovUK.Dfe.AcademiesApi.Client;
using GovUK.Dfe.AcademiesApi.Client.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Infrastructure.Clients.Academies;
using SchoolAccount.Infrastructure.Clients.Collect;
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
        services.AddAcademiesApiClient(configuration);

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

    private static void AddAcademiesApiClient(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddAcademiesApiClient<IEstablishmentsV4Client, EstablishmentsV4Client>(
            configuration
        );

        services.AddAcademiesApiClient<ITrustsV4Client, TrustsV4Client>(configuration);
    }
}
