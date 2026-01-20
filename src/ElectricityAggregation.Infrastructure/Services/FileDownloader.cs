using ElectricityAggregation.Core.Services;

namespace ElectricityAggregation.Infrastructure.Services;

public sealed class FileDownloader : IFileDownloader
{
    private readonly HttpClient _httpClient;

    public FileDownloader(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Stream> DownloadAsync(Uri url, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }
}
