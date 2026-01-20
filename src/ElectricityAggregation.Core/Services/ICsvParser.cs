using ElectricityAggregation.Core.Models;

namespace ElectricityAggregation.Core.Services;

public interface ICsvParser
{
    IEnumerable<ElectricityRecord> Parse(Stream csvStream);
}
