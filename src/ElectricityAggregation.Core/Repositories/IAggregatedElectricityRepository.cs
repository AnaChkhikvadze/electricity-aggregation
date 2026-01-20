namespace ElectricityAggregation.Core.Repositories;

using ElectricityAggregation.Core.Models;

public interface IAggregatedElectricityRepository
{
    Task UpsertAsync(
        IEnumerable<AggregatedElectricityUsage> aggregations,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AggregatedElectricityUsage>> GetAsync(
        string? region,
        YearMonth? month,
        YearMonth? startMonth,
        YearMonth? endMonth,
        CancellationToken cancellationToken);
}