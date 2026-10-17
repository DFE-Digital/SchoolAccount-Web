using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FeatureManagement;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.TestCommon.Stubs;

namespace SchoolAccount.IntegrationTests.Common.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Replaces the real query handler with <paramref name="handler"/>. The type arguments are
    /// inferred from it, so the closed generic handler type stays out of the test.
    /// </summary>
    public static IServiceCollection StubQueryHandler<TQuery, TResponse>(
        this IServiceCollection services,
        StubQueryHandler<TQuery, TResponse> handler
    )
        where TQuery : IQuery<TResponse>
    {
        return services.AddScoped<IQueryHandler<TQuery, TResponse>>(_ => handler);
    }

    /// <summary>
    /// Turns <paramref name="featureName"/> off for the test, whatever the appsettings say. Other
    /// features keep their configured state, and calls can be chained to turn off several.
    /// </summary>
    public static IServiceCollection DisableFeature(
        this IServiceCollection services,
        string featureName
    )
    {
        var descriptor = services.Last(d => d.ServiceType == typeof(IFeatureDefinitionProvider));
        services.Remove(descriptor);

        return services.AddSingleton<IFeatureDefinitionProvider>(
            serviceProvider => new DisabledFeatureDefinitionProvider(
                (IFeatureDefinitionProvider)CreateService(serviceProvider, descriptor),
                featureName
            )
        );
    }

    private static object CreateService(
        IServiceProvider serviceProvider,
        ServiceDescriptor descriptor
    ) =>
        descriptor.ImplementationInstance
        ?? descriptor.ImplementationFactory?.Invoke(serviceProvider)
        ?? ActivatorUtilities.CreateInstance(serviceProvider, descriptor.ImplementationType!);
}
