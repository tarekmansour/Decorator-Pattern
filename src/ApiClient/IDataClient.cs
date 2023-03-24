namespace Api.Client;

public interface IDataClient
{
    Task<IDictionary<string, IEnumerable<string>>> CheckDataAsync(
        IDictionary<string, string[]> myData,
        CancellationToken cancellationToken = default);
}
