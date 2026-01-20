using ElectricityAggregation.Core.Models;
using ElectricityAggregation.Core.Services;

namespace ElectricityAggregation.Worker;

public sealed class Worker : BackgroundService
{
    private readonly IElectricityIngestionService _ingestionService;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<Worker> _logger;

    public Worker(
        IElectricityIngestionService ingestionService,
        TimeProvider timeProvider,
        ILogger<Worker> logger)
    {
        _ingestionService = ingestionService;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var months = GetMonthsToProcess();

        _logger.LogInformation("Starting ingestion for {Count} months", months.Count);

        try
        {
            await _ingestionService.IngestAsync(months, stoppingToken);
            _logger.LogInformation("Ingestion completed successfully");
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Ingestion cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ingestion failed");
            throw;
        }
    }

    private List<YearMonth> GetMonthsToProcess()
    {
        var now = _timeProvider.GetUtcNow();
        var currentMonth = YearMonth.FromDateTime(now.DateTime);

        return
        [
            currentMonth.AddMonths(-2),
            currentMonth.AddMonths(-1),
            currentMonth
        ];
    }
}
