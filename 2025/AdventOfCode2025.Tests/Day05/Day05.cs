using Shouldly;

namespace AdventOfCode2025.Tests.Day05;

public class Day05
{
    private const string Example = """
                                   3-5
                                   10-14
                                   16-20
                                   12-18
                                   
                                   1
                                   5
                                   8
                                   11
                                   17
                                   32
                                   """;
    
    [Fact]
    public void Example_1()
    {
        var lines = Util.ReadRaw(Example);
        var wh = new Warehouse(lines);
        wh.FreshCount().ShouldBe(3);
    }
    
    [Fact]
    public void Real_1()
    {
        var lines = Util.ReadFile("day05");
        var wh = new Warehouse(lines);
        wh.FreshCount().ShouldBe(640);
    }
    
    [Fact]
    public void Example_2()
    {
        var lines = Util.ReadRaw(Example);
        var wh = new Warehouse(lines);
        wh.FreshRangeCount().ShouldBe(14);
    }
    
    [Fact]
    public void Real_2()
    {
        var lines = Util.ReadFile("day05");
        var wh = new Warehouse(lines);
        wh.FreshRangeCount().ShouldBe(365804144481581L);
    }
}

public class Warehouse(IReadOnlyList<string> input)
{
    private List<Range> Ranges { get; } =
        input.TakeWhile(l => !string.IsNullOrWhiteSpace(l)).Select(Range.FromLine).ToList();

    public int FreshCount()
    {
        var c = 0;
        foreach (var line in input.SkipWhile(l => !string.IsNullOrWhiteSpace(l)).Skip(1))
        {
            var val = long.Parse(line);
            if (Ranges.Any(r => r.Contains(val))) c++;
        }

        return c;
    }

    public long FreshRangeCount()
    {
        var sortedRanges = Ranges.OrderBy(r => r.Start).ToList();
        var count = 0L;
        var alreadyCounted = 0L;
        foreach (var range in sortedRanges)
        {
            var min = range.Start;
            if (alreadyCounted >= min)
            {
                min = alreadyCounted + 1;
            }
            if (alreadyCounted >= range.End) continue;
            count += range.End - min + 1;
            alreadyCounted = range.End;
        }

        return count;
    }
}

public record Range(long Start, long End)
{
    public bool Contains(long n) => n >= Start && n <= End;

    public static Range FromLine(string l)
    {
        var left = long.Parse(l.Split('-')[0]);
        var right = long.Parse(l.Split('-')[1]);

        var low = left < right ? left : right;
        var high = low == left ? right : left;
        return new Range(low, high);
    }
}