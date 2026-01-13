using System.Text.Json;

internal static class HttpClientExt
{
    public static async Task<JsonDocument> ReadJsonDocumentAsync(this HttpClient client, string requestUri)
    {
        using var response = await client.GetAsync(requestUri);
        //throw new Exception(response.Content);
        //response.EnsureSuccessStatusCode();
        //return await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        throw new Exception(await response.Content.ReadAsStringAsync());

    }
}