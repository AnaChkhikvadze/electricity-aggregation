namespace ElectricityAggregation.Core.Services;

public interface IFileDownloader
{
    Task<Stream> DownloadAsync(Uri url, CancellationToken cancellationToken);
}
