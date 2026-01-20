using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using ElectricityAggregation.Core.Models;
using ElectricityAggregation.Core.Services;

namespace ElectricityAggregation.Infrastructure.Services;

public sealed class CsvParser : ICsvParser
{
    public IEnumerable<ElectricityRecord> Parse(Stream csvStream)
    {
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            BadDataFound = null,
            MissingFieldFound = null
        });

        csv.Context.RegisterClassMap<ElectricityRecordMap>();

        while (csv.Read())
        {
            ElectricityRecord? record = null;
            try
            {
                record = csv.GetRecord<CsvRow>()?.ToElectricityRecord();
            }
            catch
            {
                // skip malformed rows
            }

            if (record is not null)
            {
                yield return record;
            }
        }
    }

    private sealed class CsvRow
    {
        public string Region { get; set; } = string.Empty;
        public string BuildingType { get; set; } = string.Empty;
        public decimal PPlus { get; set; }
        public decimal PMinus { get; set; }
        public string Month { get; set; } = string.Empty;

        public ElectricityRecord? ToElectricityRecord()
        {
            if (string.IsNullOrWhiteSpace(Region) ||
                string.IsNullOrWhiteSpace(BuildingType) ||
                string.IsNullOrWhiteSpace(Month))
            {
                return null;
            }

            if (!TryParseYearMonth(Month, out var yearMonth))
            {
                return null;
            }

            return new ElectricityRecord
            {
                Region = Region,
                BuildingType = BuildingType,
                PowerConsumed = PPlus,
                PowerProduced = PMinus,
                RecordMonth = yearMonth
            };
        }

        private static bool TryParseYearMonth(string value, out YearMonth result)
        {
            result = default;
            var parts = value.Split('-');
            if (parts.Length != 2)
            {
                return false;
            }

            if (!int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var year) ||
                !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var month))
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
    }

    private sealed class ElectricityRecordMap : ClassMap<CsvRow>
    {
        public ElectricityRecordMap()
        {
            Map(m => m.Region).Name("Region");
            Map(m => m.BuildingType).Name("BuildingType");
            Map(m => m.PPlus).Name("P+");
            Map(m => m.PMinus).Name("P-");
            Map(m => m.Month).Name("Month");
        }
    }
}
