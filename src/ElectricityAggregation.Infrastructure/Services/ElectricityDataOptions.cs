namespace ElectricityAggregation.Infrastructure.Services;

public sealed class ElectricityDataOptions
{
    public const string SectionName = "ElectricityData";

    public required string BaseUrl { get; set; }
}
