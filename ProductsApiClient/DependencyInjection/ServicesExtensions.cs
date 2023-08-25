using Microsoft.Extensions.DependencyInjection;

namespace ProductsApiClient.DependencyInjection;

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

        return services.AddHttpClient<IProductClient>()
            .ConfigureHttpClient(client => client.BaseAddress = baseAddress);
    }
}
