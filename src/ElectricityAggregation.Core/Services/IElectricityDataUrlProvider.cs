using ElectricityAggregation.Core.Models;

namespace ElectricityAggregation.Core.Services;

public interface IElectricityDataUrlProvider
{
    Uri GetCsvUrl(YearMonth month);
}
