using Microsoft.Extensions.DependencyInjection;

namespace ProductsApiClient.DependencyInjection;

public static class ServicesExtensions
{
    public static IHttpClientBuilder AddProductsClient(
        this IServiceCollection services,
        Uri baseAddress)
    {
        if (services is null)
        {
            throw new ArgumentNullException(
                nameof(services),
                "Failed to configure services: IServiceCollection is null.");
        }

        return services.AddHttpClient<IProductsClient>()
            .ConfigureHttpClient(client => client.BaseAddress = baseAddress);
    }

    public static IHttpClientBuilder AddCachedProductsClient(
        this IServiceCollection services,
        Uri baseAddress)
    {
        if (services is null)
        {
            throw new ArgumentNullException(
                nameof(services),
                "Failed to configure services: IServiceCollection is null.");
        }

        var httpClientBuilder = services.AddHttpClient<IProductsClient>()
            .ConfigureHttpClient(client => client.BaseAddress = baseAddress);

        services.Decorate<IProductsClient, CachedProductsClient>();
        services.AddMemoryCache();

        return httpClientBuilder;
    }
}
