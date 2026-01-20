namespace ElectricityAggregation.UnitTests.Aggregation;

using ElectricityAggregation.Core.Aggregation;
using ElectricityAggregation.Core.Models;
using FluentAssertions;
using Xunit;

public class ElectricityAggregatorTests
{
    private readonly ElectricityAggregator _aggregator = new();
    private readonly DateTime _processedAt =
        new(2026, 1, 20, 10, 30, 0, DateTimeKind.Utc);

    [Fact]
    public void Aggregate_SingleRegionSingleMonth_AggregatesCorrectly()
    {
        var records = new[]
        {
            new ElectricityRecord
            {
                Region = "Vilnius",
                BuildingType = "Butas",
                PowerConsumed = 1000.50m,
                PowerProduced = 50.25m,
                RecordMonth = new YearMonth(2025, 10)
            },
            new ElectricityRecord
            {
                Region = "Vilnius",
                BuildingType = "Butas",
                PowerConsumed = 2000.75m,
                PowerProduced = 75.50m,
                RecordMonth = new YearMonth(2025, 10)
            }
        };

        var result = _aggregator.Aggregate(records, _processedAt);

        result.Should().HaveCount(1);
        result[0].Region.Should().Be("Vilnius");
        result[0].Month.Should().Be(new YearMonth(2025, 10));
        result[0].TotalPowerConsumed.Should().Be(3001.25m);
        result[0].TotalPowerProduced.Should().Be(125.75m);
        result[0].RecordCount.Should().Be(2);
        result[0].ProcessedAtUtc.Should().Be(_processedAt);
    }

    [Fact]
    public void Aggregate_MultipleRegionsSameMonth_CreatesSeparateAggregations()
    {
        var records = new[]
        {
            new ElectricityRecord
            {
                Region = "Vilnius",
                BuildingType = "Butas",
                PowerConsumed = 1000m,
                PowerProduced = 50m,
                RecordMonth = new YearMonth(2025, 10)
            },
            new ElectricityRecord
            {
                Region = "Kaunas",
                BuildingType = "Butas",
                PowerConsumed = 2000m,
                PowerProduced = 100m,
                RecordMonth = new YearMonth(2025, 10)
            },
            new ElectricityRecord
            {
                Region = "Klaipėda",
                BuildingType = "Butas",
                PowerConsumed = 1500m,
                PowerProduced = 75m,
                RecordMonth = new YearMonth(2025, 10)
            }
        };

        var result = _aggregator.Aggregate(records, _processedAt);

        result.Should().HaveCount(3);
        result.Should().Contain(r => r.Region == "Vilnius" && r.TotalPowerConsumed == 1000m);
        result.Should().Contain(r => r.Region == "Kaunas" && r.TotalPowerConsumed == 2000m);
        result.Should().Contain(r => r.Region == "Klaipėda" && r.TotalPowerConsumed == 1500m);
    }

    [Fact]
    public void Aggregate_MultipleMonthsSameRegion_CreatesSeparateAggregations()
    {
        var records = new[]
        {
            new ElectricityRecord
            {
                Region = "Vilnius",
                BuildingType = "Butas",
                PowerConsumed = 1000m,
                PowerProduced = 50m,
                RecordMonth = new YearMonth(2025, 10)
            },
            new ElectricityRecord
            {
                Region = "Vilnius",
                BuildingType = "Butas",
                PowerConsumed = 1100m,
                PowerProduced = 55m,
                RecordMonth = new YearMonth(2025, 11)
            },
            new ElectricityRecord
            {
                Region = "Vilnius",
                BuildingType = "Butas",
                PowerConsumed = 1200m,
                PowerProduced = 60m,
                RecordMonth = new YearMonth(2025, 12)
            }
        };

        var result = _aggregator.Aggregate(records, _processedAt);

        result.Should().HaveCount(3);
        result.Should().Contain(r => r.Month == new YearMonth(2025, 10) && r.TotalPowerConsumed == 1000m);
        result.Should().Contain(r => r.Month == new YearMonth(2025, 11) && r.TotalPowerConsumed == 1100m);
        result.Should().Contain(r => r.Month == new YearMonth(2025, 12) && r.TotalPowerConsumed == 1200m);
    }

    [Fact]
    public void Aggregate_NonApartmentRecords_AreIgnored()
    {
        var records = new[]
        {
            new ElectricityRecord
            {
                Region = "Vilnius",
                BuildingType = "Butas",
                PowerConsumed = 1000m,
                PowerProduced = 50m,
                RecordMonth = new YearMonth(2025, 10)
            },
            new ElectricityRecord
            {
                Region = "Vilnius",
                BuildingType = "Namas",
                PowerConsumed = 2000m,
                PowerProduced = 100m,
                RecordMonth = new YearMonth(2025, 10)
            }
        };

        var result = _aggregator.Aggregate(records, _processedAt);

        result.Should().HaveCount(1);
        result[0].TotalPowerConsumed.Should().Be(1000m);
        result[0].RecordCount.Should().Be(1);
    }

    [Fact]
    public void Aggregate_EmptyInput_ReturnsEmptyList()
    {
        var result = _aggregator.Aggregate(Array.Empty<ElectricityRecord>(), _processedAt);

        result.Should().BeEmpty();
    }
}
