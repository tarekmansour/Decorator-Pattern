using Microsoft.Extensions.Caching.Memory;
using ProductsApiClient.Models;

namespace ProductsApiClient;

public class CachedProductsClient : IProductsClient
{
    private readonly IProductsClient _decorated;
    private readonly IMemoryCache _memoryCache;

    public CachedProductsClient(IProductsClient decorated, IMemoryCache memoryCache)
    {
        _decorated = decorated;
        _memoryCache = memoryCache;
    }

    public async Task<Product?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"products-{Id}";

        return await _memoryCache.GetOrCreateAsync(
            cacheKey,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                return _decorated.GetByIdAsync(Id, cancellationToken);
            });
    }
}
