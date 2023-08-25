namespace ProductsApiClient.Models;

public record Product(
    Guid Id,
    string Name,
    string Supplier,
    double UnitPrice);