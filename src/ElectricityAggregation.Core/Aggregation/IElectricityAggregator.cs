using ElectricityAggregation.Core.Models;

namespace ElectricityAggregation.Core.Aggregation;

public interface IElectricityAggregator
{
    IReadOnlyList<AggregatedElectricityUsage> Aggregate(
        IEnumerable<ElectricityRecord> records,
        DateTime processedAtUtc);
}
