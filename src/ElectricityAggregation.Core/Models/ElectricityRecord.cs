namespace ElectricityAggregation.Core.Models;

public record ElectricityRecord
{
    public required string Region { get; init; }
    public required string BuildingType { get; init; }
    public required decimal PowerConsumed { get; init; }
    public required decimal PowerProduced { get; init; }
    public required YearMonth RecordMonth { get; init; }
}