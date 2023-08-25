using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using ProductsApiClient.Models;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductsApiClient.Tests;

[ExcludeFromCodeCoverage]
public class ProductsClientTests
{
    [Fact(DisplayName = "New Products client with null httpClient")]
    public void New_With_Null_HttpClient_Throws_ArgumentNullException()
    {
        //Arrange
        var constructor = () => new ProductsClient(null!);

        //Act & Assert
        constructor.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("httpClient");
    }

    [Fact(DisplayName = "Get product by id with invalid GUID")]
    public async Task GetByIdAsync_InvalidId_ThrowsException()
    {
        // Arrange
        var httpClient = Substitute.For<HttpClient>();
        var productsClient = new ProductsClient(httpClient);
        string invalidId = null;

        // Act
        Func<Task> act = async () => await productsClient.GetByIdAsync(new Guid(invalidId));

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "Get Prodcut by Id from API")]
    public async Task GetByIdAsync_ValidId_ReturnsProduct()
    {
        // Arrange
        var productId = new Guid("65c3da65-debb-47e3-b4dd-46bfd171a2d8");
        var expectedProduct = new Product(productId, "ProductName", "ProductSupplier", 5.10m);
        var httpClient = Substitute.For<HttpClient>();
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expectedProduct)
        };

        httpClient.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(httpResponse);
        var productsClient = new ProductsClient(httpClient);

        // Act
        var result = await productsClient.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeEquivalentTo(expectedProduct);
    }

    [Fact(DisplayName = "Get Prodcut by Id from Cache")]
    public async Task GetByIdAsync_Should_Return_Cached_Item_If_Available()
    {
        // Arrange
        var productId = new Guid("65c3da65-debb-47e3-b4dd-46bfd171a2d8");
        var cancellationToken = CancellationToken.None;
        var decoratedSubstitute = Substitute.For<IProductsClient>();
        var memoryCacheSubstitute = Substitute.For<IMemoryCache>();

        decoratedSubstitute.GetByIdAsync(productId, cancellationToken)
            .Returns(new Product(productId, "ProductName", "ProductSupplier", 5.10m));

        memoryCacheSubstitute.TryGetValue($"products-{productId}", out Arg.Any<Product?>())
            .Returns(false);

        var cachedClient = new CachedProductsClient(decoratedSubstitute, memoryCacheSubstitute);

        // Act
        var result = await cachedClient.GetByIdAsync(productId, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        await decoratedSubstitute.Received(1)
            .GetByIdAsync(productId, cancellationToken);
    }
}