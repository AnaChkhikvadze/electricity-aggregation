namespace ElectricityAggregation.Api.Contracts;

public record AggregatedElectricityResponse(
    string Region,
    string Month,
    decimal TotalConsumed,
    decimal TotalProduced,
    int ApartmentCount,
    DateTime LastUpdatedUtc);
