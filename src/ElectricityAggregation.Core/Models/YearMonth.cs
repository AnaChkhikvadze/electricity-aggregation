namespace ElectricityAggregation.Core.Models;

public readonly record struct YearMonth : IComparable<YearMonth>
{
    public int Year { get; }
    public int Month { get; }

    public YearMonth(int year, int month)
    {
        Year = year;
        Month = month;
    }

    public DateTime ToDateTime() => new(Year, Month, 1);

    public static YearMonth FromDateTime(DateTime dateTime) =>
        new(dateTime.Year, dateTime.Month);

    public YearMonth AddMonths(int months)
    {
        var date = ToDateTime().AddMonths(months);
        return new YearMonth(date.Year, date.Month);
    }

    public int CompareTo(YearMonth other)
    {
        var yearComparison = Year.CompareTo(other.Year);
        return yearComparison != 0
            ? yearComparison
            : Month.CompareTo(other.Month);
    }

    public override string ToString() => $"{Year:D4}-{Month:D2}";
}