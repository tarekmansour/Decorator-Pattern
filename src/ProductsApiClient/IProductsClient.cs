using ProductsApiClient.Models;

namespace ProductsApiClient;
public interface IProductsClient
{
    Task<Product?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default);
}
