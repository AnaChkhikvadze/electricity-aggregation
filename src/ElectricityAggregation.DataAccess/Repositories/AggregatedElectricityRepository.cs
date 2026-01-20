namespace ElectricityAggregation.DataAccess.Repositories;

using System.Text;
using Dapper;
using ElectricityAggregation.Core.Models;
using ElectricityAggregation.Core.Repositories;
using Npgsql;

public sealed class AggregatedElectricityRepository : IAggregatedElectricityRepository
{
    private readonly string _connectionString;

    public AggregatedElectricityRepository(string connectionString)
    {
        _connectionString = connectionString; 
    }

    public async Task UpsertAsync(
        IEnumerable<AggregatedElectricityUsage> aggregations,
        CancellationToken cancellationToken)
    {
        if (!aggregations.Any())
        {
            return;
        }

        const string sql = """
            INSERT INTO aggregated_electricity (
                region,
                month,
                total_power_consumed,
                total_power_produced,
                record_count,
                processed_at_utc
            )
            VALUES (
                @Region,
                @Month,
                @TotalPowerConsumed,
                @TotalPowerProduced,
                @RecordCount,
                @ProcessedAtUtc
            )
            ON CONFLICT (region, month)
            DO UPDATE SET
                total_power_consumed = EXCLUDED.total_power_consumed,
                total_power_produced = EXCLUDED.total_power_produced,
                record_count = EXCLUDED.record_count,
                processed_at_utc = EXCLUDED.processed_at_utc
            """;

        var parameters = aggregations.Select(a => new
        {
            Region = a.Region,
            Month = a.Month.ToDateTime(),
            TotalPowerConsumed = a.TotalPowerConsumed,
            TotalPowerProduced = a.TotalPowerProduced,
            RecordCount = a.RecordCount,
            ProcessedAtUtc = a.ProcessedAtUtc
        });

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                parameters,
                cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<AggregatedElectricityUsage>> GetAsync(
        string? region,
        YearMonth? month,
        YearMonth? startMonth,
        YearMonth? endMonth,
        CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder();
        sqlBuilder.AppendLine("""
            SELECT
                region AS Region,
                month AS Month,
                total_power_consumed AS TotalPowerConsumed,
                total_power_produced AS TotalPowerProduced,
                record_count AS RecordCount,
                processed_at_utc AS ProcessedAtUtc
            FROM aggregated_electricity
            WHERE 1 = 1
            """);

        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(region))
        {
            sqlBuilder.AppendLine("AND LOWER(region) = LOWER(@Region)");
            parameters.Add("Region", region);
        }

        if (month.HasValue)
        {
            sqlBuilder.AppendLine("AND month = @Month");
            parameters.Add("Month", month.Value.ToDateTime());
        }

        if (startMonth.HasValue)
        {
            sqlBuilder.AppendLine("AND month >= @StartMonth");
            parameters.Add("StartMonth", startMonth.Value.ToDateTime());
        }

        if (endMonth.HasValue)
        {
            sqlBuilder.AppendLine("AND month <= @EndMonth");
            parameters.Add("EndMonth", endMonth.Value.ToDateTime());
        }

        sqlBuilder.AppendLine("ORDER BY month DESC, region");

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var rows = await connection.QueryAsync<DatabaseRow>(
            new CommandDefinition(
                sqlBuilder.ToString(),
                parameters,
                cancellationToken: cancellationToken));

        return rows.Select(row => new AggregatedElectricityUsage
        {
            Region = row.Region,
            Month = YearMonth.FromDateTime(row.Month),
            TotalPowerConsumed = row.TotalPowerConsumed,
            TotalPowerProduced = row.TotalPowerProduced,
            RecordCount = row.RecordCount,
            ProcessedAtUtc = row.ProcessedAtUtc
        }).ToList();
    }

    private sealed class DatabaseRow
    {
        public string Region { get; init; } = string.Empty;
        public DateTime Month { get; init; }
        public decimal TotalPowerConsumed { get; init; }
        public decimal TotalPowerProduced { get; init; }
        public int RecordCount { get; init; }
        public DateTime ProcessedAtUtc { get; init; }
    }
}
