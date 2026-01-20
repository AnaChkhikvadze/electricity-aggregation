using ElectricityAggregation.Api.Contracts;
using ElectricityAggregation.Core.Models;
using ElectricityAggregation.Core.Repositories;
using ElectricityAggregation.DataAccess;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PostgreSQL")
    ?? throw new InvalidOperationException("PostgreSQL connection string is required");

builder.Services.AddDataAccess(connectionString);

var app = builder.Build();

app.MapGet("/api/electricity/aggregated", async (
    string? region,
    string? month,
    string? startMonth,
    string? endMonth,
    IAggregatedElectricityRepository repository,
    CancellationToken cancellationToken) =>
{
    if (month is not null && (startMonth is not null || endMonth is not null))
    {
        return Results.BadRequest(new { error = "Cannot combine 'month' with 'startMonth' or 'endMonth'" });
    }

    YearMonth? parsedMonth = null;
    YearMonth? parsedStartMonth = null;
    YearMonth? parsedEndMonth = null;

    if (month is not null)
    {
        if (!TryParseYearMonth(month, out var m))
        {
            return Results.BadRequest(new { error = "Invalid 'month' format. Expected YYYY-MM" });
        }
        parsedMonth = m;
    }

    if (startMonth is not null)
    {
        if (!TryParseYearMonth(startMonth, out var sm))
        {
            return Results.BadRequest(new { error = "Invalid 'startMonth' format. Expected YYYY-MM" });
        }
        parsedStartMonth = sm;
    }

    if (endMonth is not null)
    {
        if (!TryParseYearMonth(endMonth, out var em))
        {
            return Results.BadRequest(new { error = "Invalid 'endMonth' format. Expected YYYY-MM" });
        }
        parsedEndMonth = em;
    }

    if (parsedStartMonth.HasValue && parsedEndMonth.HasValue &&
        parsedStartMonth.Value.CompareTo(parsedEndMonth.Value) > 0)
    {
        return Results.BadRequest(new { error = "'startMonth' must be less than or equal to 'endMonth'" });
    }

    var data = await repository.GetAsync(
        region,
        parsedMonth,
        parsedStartMonth,
        parsedEndMonth,
        cancellationToken);

    var response = data.Select(ToResponse).ToList();

    return Results.Ok(response);
});

app.Run();

static bool TryParseYearMonth(string value, out YearMonth result)
{
    result = default;
    var parts = value.Split('-');
    if (parts.Length != 2)
    {
        return false;
    }

    if (!int.TryParse(parts[0], out var year) || !int.TryParse(parts[1], out var month))
    {
        return false;
    }

    if (month < 1 || month > 12)
    {
        return false;
    }

    result = new YearMonth(year, month);
    return true;
}

static AggregatedElectricityResponse ToResponse(AggregatedElectricityUsage usage) => new(
    usage.Region,
    usage.Month.ToString(),
    usage.TotalPowerConsumed,
    usage.TotalPowerProduced,
    usage.RecordCount,
    usage.ProcessedAtUtc);
