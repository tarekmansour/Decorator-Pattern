using ProductsApiClient.Models;
using System.Net.Http.Json;

namespace ProductsApiClient;

public class ProductClient : IProductClient
{
    private const string ApiVersion = "1.0";
    private readonly HttpClient _httpClient;

    public ProductClient(HttpClient httpClient)
    {
        if (httpClient is null)
        {
            throw new ArgumentNullException(nameof(httpClient), "Failed to perform HTTP request: HttpClient instance is null.");
        }
        _httpClient = httpClient;
    }

    public async Task<Product?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(Id.ToString(), out var _))
        {
            throw new ArgumentNullException(nameof(Id), "Product Id is not valid. Invalid GUID format");
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"products/{Id}?api-version={ApiVersion}");

        var response = await _httpClient.SendAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Request failed with status code: " + response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<Product?>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
