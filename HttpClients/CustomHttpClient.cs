using System.Text.Json;

namespace LoggingAuto.HttpClients;

public class CustomHttpClient
{
    private readonly HttpClient _httpClient;

    public CustomHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
    public async Task<TResult?> Get<TResult>(string action, string? parametes = null)
    {
        var request = CreateRequest();
        using HttpResponseMessage result = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        using Stream contentStream = await result.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<TResult?>(contentStream, options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    private static HttpRequestMessage CreateRequest(HttpMethod? method = null)
    {
        method ??= HttpMethod.Get;
        return new HttpRequestMessage(method, "path");
    }
}