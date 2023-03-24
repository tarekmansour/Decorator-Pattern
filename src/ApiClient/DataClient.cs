using Api.Client.ErrorManagement;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Api.Client;

public class DataClient : IDataClient
{
    private const string ApiVersion = "1.0";
    private readonly HttpClient _httpClient;

    public DataClient(HttpClient httpClient)
    {
        if (httpClient is null)
        {
            throw new ArgumentNullException(nameof(httpClient), "Failed to perform HTTP request: HttpClient instance is null.");
        }
        _httpClient = httpClient;
    }
    public virtual async Task<IDictionary<string, IEnumerable<string>>> CheckDataAsync(
        IDictionary<string, string[]> myData,
        CancellationToken cancellationToken = default)
    {
        if (!myData.Any())
        {
            throw new ArgumentNullException(nameof(myData), $"Failed to check data: input data is null. Parameter name: {nameof(myData)}");
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"controller/checking?api-version={ApiVersion}")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(myData),
                Encoding.UTF8,
                "application/json")
        };

        var response = await _httpClient.SendAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponseAsync(response, cancellationToken)
                .ConfigureAwait(false);
        }

        return (await response.Content
            .ReadFromJsonAsync<IDictionary<string, IEnumerable<string>>>(cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    private static async Task HandleErrorResponseAsync(
        HttpResponseMessage responseMessage,
        CancellationToken cancellationToken = default)
    {
        try
        {
            switch (responseMessage.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    break;
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.InternalServerError:
                    var problem = await responseMessage.Content
                        .ReadFromJsonAsync<ProblemDetails>(cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                    throw new DataException(problem!);
                default:
                    var message = await responseMessage.Content
                        .ReadAsStringAsync(cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    if (string.IsNullOrEmpty(message))
                    {
                        message = responseMessage.ReasonPhrase;
                    }

                    throw new DataException(message!, (int)responseMessage.StatusCode);
            }
        }
        catch (JsonException ex)
        {
            var message = await responseMessage.Content
                .ReadAsStringAsync (cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (string.IsNullOrEmpty(message))
            {
                message = responseMessage.ReasonPhrase;
            }

            throw new DataException(message!, (int)responseMessage.StatusCode, ex);
        }
    }
}
