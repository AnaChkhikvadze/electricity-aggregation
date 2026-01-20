namespace ElectricityAggregation.Core.Aggregation;

using ElectricityAggregation.Core.Models;

public class ElectricityAggregator
{
    public IReadOnlyList<AggregatedElectricityUsage> Aggregate(
        IEnumerable<ElectricityRecord> records,
        DateTime processedAtUtc)
    {
        return records
            .Where(r => !string.IsNullOrWhiteSpace(r.Region))
            .Where(r =>
                string.Equals(
                    r.BuildingType?.Trim(),
                    "Butas",
                    StringComparison.OrdinalIgnoreCase))
            .GroupBy(r => new
            {
                Region = r.Region.Trim(),
                r.RecordMonth
            })
            .Select(group => new AggregatedElectricityUsage
            {
                Region = group.Key.Region,
                Month = group.Key.RecordMonth,
                TotalPowerConsumed = group.Sum(r => r.PowerConsumed),
                TotalPowerProduced = group.Sum(r => r.PowerProduced),
                RecordCount = group.Count(),
                ProcessedAtUtc = processedAtUtc
            })
            .ToList();
    }
}