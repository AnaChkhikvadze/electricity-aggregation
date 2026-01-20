using ElectricityAggregation.Core.Models;
using ElectricityAggregation.Core.Services;
using Microsoft.Extensions.Options;

namespace ElectricityAggregation.Infrastructure.Services;

public sealed class ElectricityDataUrlProvider : IElectricityDataUrlProvider
{
    private readonly ElectricityDataOptions _options;

    public ElectricityDataUrlProvider(IOptions<ElectricityDataOptions> options)
    {
        _options = options.Value;
    }

    public Uri GetCsvUrl(YearMonth month)
    {
        var baseUrl = _options.BaseUrl.TrimEnd('/');
        var path = $"{month.Year:D4}/{month.Month:D2}/electricity.csv";
        return new Uri($"{baseUrl}/{path}");
    }
}
