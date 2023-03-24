using Microsoft.Extensions.Caching.Memory;

namespace Api.Client;

public class CachedDataClient : IDataClient
{
    private readonly IDataClient _decorated;
    private readonly IMemoryCache _memoryCache;
    private string _cacheKey = string.Empty;

    public CachedDataClient(
        IDataClient dataClient,
        IMemoryCache memoryCache)
    {
        _decorated = dataClient;
        _memoryCache = memoryCache;
    }
    public virtual async Task<IDictionary<string, IEnumerable<string>>> CheckDataAsync(
        IDictionary<string, string[]> myData,
        CancellationToken cancellationToken = default)
    {
        _cacheKey = "DataKey";

        (var cachedValues, var existingValues, var newValues) = FigureAlreadyExistingData(myData);

        if (!newValues.Keys.Any())
        {
            return existingValues;
        }

        var clientRespose = await _decorated
            .CheckDataAsync(newValues, cancellationToken)
            .ConfigureAwait(false);

        if (clientRespose.Count != 0)
        {
            _memoryCache.Set(_cacheKey,
                GetUnion(cachedValues, clientRespose),
                TimeSpan.FromHours(1));
        }

        return GetUnion(existingValues, clientRespose);
    }

    private (
        IDictionary<string,IEnumerable<string>>,
        IDictionary<string, IEnumerable<string>>,
        IDictionary<string, string[]>)
        FigureAlreadyExistingData(IDictionary<string, string[]> myData)
    {
        var alreadyExistingData = _memoryCache.Get<IDictionary<string, IEnumerable<string>>>(_cacheKey);

        if (alreadyExistingData is null)
        {
            return (new Dictionary<string, IEnumerable<string>>(), new Dictionary<string, IEnumerable<string>>(), myData);
        }

        var intersection = alreadyExistingData.Keys
            .Intersect(myData.Keys)
            .ToDictionary(x => x, x => alreadyExistingData[x].Where(m => myData[x].Any(mv => mv == m)));

        var incoherents = alreadyExistingData.Keys
            .Intersect(myData.Keys)
            .ToDictionary(x => x, x => myData[x].Except(alreadyExistingData[x].Select(x => x)).ToArray())
            .Where(x => x.Value.Any())
            .ToDictionary(x => x.Key, x => x.Value);

        return (alreadyExistingData, intersection, incoherents);
    }

    //TODO: to refactor
    private IDictionary<string, IEnumerable<string>> GetUnion(
        IDictionary<string, IEnumerable<string>> oldValues,
        IDictionary<string, IEnumerable<string>> newValues)
        => oldValues.Keys
            .Union(newValues.Keys)
            .Distinct()
            .ToDictionary(x => x,
                x => Resolve(oldValues, x).Union(Resolve(newValues, x))
                        .GroupBy(x => x)
                        .Select(x => x.First()));

    private IEnumerable<string> Resolve(
        IDictionary<string, IEnumerable<string>> keyValuePairs,
        string key)
        => keyValuePairs.ContainsKey(key)
            ? keyValuePairs[key]
            : new List<string>();
}
