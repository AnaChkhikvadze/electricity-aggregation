namespace ElectricityAggregation.Core.Models;

public record AggregatedElectricityUsage
{
    public required string Region { get; init; }
    public required YearMonth Month { get; init; }
    public required decimal TotalPowerConsumed { get; init; }
    public required decimal TotalPowerProduced { get; init; }
    public required int RecordCount { get; init; }
    public required DateTime ProcessedAtUtc { get; init; }
}