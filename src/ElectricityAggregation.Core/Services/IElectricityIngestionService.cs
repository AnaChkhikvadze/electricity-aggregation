using ElectricityAggregation.Core.Models;

namespace ElectricityAggregation.Core.Services;

public interface IElectricityIngestionService
{
    Task IngestAsync(IEnumerable<YearMonth> months, CancellationToken cancellationToken);
}
