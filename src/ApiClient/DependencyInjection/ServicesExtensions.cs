using Microsoft.Extensions.DependencyInjection;

namespace Api.Client.DependencyInjection;

public static class ServicesExtensions
{
    public static IHttpClientBuilder AddDataClient(
        this IServiceCollection services,
        Uri baseAddress)
    {
        if (services is null)
        {
            throw new ArgumentNullException(
                nameof(services),
                $"Failed to configure services: IServiceCollection is null. Parameter name: {nameof(services)}");
        }

        return services.AddHttpClient<IDataClient>()
            .ConfigureHttpClient(client => client.BaseAddress = baseAddress);
    }

    public static IHttpClientBuilder AddCachedDataClient(
        this IServiceCollection services,
        Uri baseAddress)
    {
        if (services is null)
        {
            throw new ArgumentNullException(
                nameof(services),
                $"Failed to configure services: IServiceCollection is null. Parameter name: {nameof(services)}");
        }

        var httpClientBuilder = services.AddHttpClient<IDataClient>()
            .ConfigureHttpClient(client => client.BaseAddress = baseAddress);

        services.Decorate<IDataClient, CachedDataClient>();
        services.AddMemoryCache();

        return httpClientBuilder;
    }
}
