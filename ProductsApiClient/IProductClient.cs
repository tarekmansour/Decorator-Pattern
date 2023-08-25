using ProductsApiClient.Models;

namespace ProductsApiClient;
public interface IProductClient
{
    Task<Product?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default);
}
