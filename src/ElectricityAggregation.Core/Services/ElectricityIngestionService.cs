using ElectricityAggregation.Core.Aggregation;
using ElectricityAggregation.Core.Models;
using ElectricityAggregation.Core.Repositories;

namespace ElectricityAggregation.Core.Services;

public sealed class ElectricityIngestionService : IElectricityIngestionService
{
    private readonly IElectricityDataUrlProvider _urlProvider;
    private readonly IFileDownloader _fileDownloader;
    private readonly ICsvParser _csvParser;
    private readonly IElectricityAggregator _aggregator;
    private readonly IAggregatedElectricityRepository _repository;
    private readonly TimeProvider _timeProvider;

    public ElectricityIngestionService(
        IElectricityDataUrlProvider urlProvider,
        IFileDownloader fileDownloader,
        ICsvParser csvParser,
        IElectricityAggregator aggregator,
        IAggregatedElectricityRepository repository,
        TimeProvider timeProvider)
    {
        _urlProvider = urlProvider;
        _fileDownloader = fileDownloader;
        _csvParser = csvParser;
        _aggregator = aggregator;
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task IngestAsync(IEnumerable<YearMonth> months, CancellationToken cancellationToken)
    {
        foreach (var month in months)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await IngestMonthAsync(month, cancellationToken);
        }
    }

    private async Task IngestMonthAsync(YearMonth month, CancellationToken cancellationToken)
    {
        var url = _urlProvider.GetCsvUrl(month);

        await using var csvStream = await _fileDownloader.DownloadAsync(url, cancellationToken);

        var records = _csvParser.Parse(csvStream);

        var aggregated = _aggregator.Aggregate(records, _timeProvider.GetUtcNow().UtcDateTime);

        if (aggregated.Count > 0)
        {
            await _repository.UpsertAsync(aggregated, cancellationToken);
        }
    }
}
