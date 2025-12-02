using Shouldly;

namespace AdventOfCode2025.Tests.Day02;

public class Day02
{
    private const string Example = """
                                   11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124
                                   """;
    
    [Fact]
    public void Example_1()
    {
        var lines = Util.ReadRaw(Example);
        var ranges = new IdRanges(lines.Single());
        ranges.SumOfInvalid().ShouldBe(1227775554);
    }
    
    [Fact]
    public void Real_1()
    {
        var lines = Util.ReadFile("day02");
        var ranges = new IdRanges(lines.Single());
        ranges.SumOfInvalid().ShouldBe(31839939622L);
    }
    
    [Fact]
    public void Example_2()
    {
        var lines = Util.ReadRaw(Example);
        var ranges = new IdRanges(lines.Single());
        ranges.SumOfInvalidAnyLength().ShouldBe(4174379265);
    }
    
    [Fact]
    public void Real_2()
    {
        var lines = Util.ReadFile("day02");
        var ranges = new IdRanges(lines.Single());
        ranges.SumOfInvalidAnyLength().ShouldBe(41662374059L);
    }
}

public class IdRanges(string input)
{
    public long SumOfInvalid()
    {
        var ranges = input.Split(',');
        var sum = 0L;
        foreach (var range in ranges)
        {
            var left = long.Parse(range.Split('-')[0]);
            var right = long.Parse(range.Split('-')[1]);
            for (var i = left; i <= right; i++)
            {
                var s = i.ToString();
                if (s.Length % 2 == 1) continue;
                if (s[..(s.Length >> 1)] == s[(s.Length >> 1)..]) sum += i;
            }
        }

        return sum;
    }
    
    public long SumOfInvalidAnyLength()
    {
        var ranges = input.Split(',');
        var sum = 0L;
        foreach (var range in ranges)
        {
            var left = long.Parse(range.Split('-')[0]);
            var right = long.Parse(range.Split('-')[1]);
            for (var i = left; i <= right; i++)
            {
                if (Repeats(i)) sum += i;
            }
        }

        return sum;
    }

    private bool Repeats(long n)
    {
        var s = n.ToString();
        for (var repLen = 1; repLen < s.Length; repLen++)
        {
            var p = s[..repLen];
            if (s.Split(p, StringSplitOptions.RemoveEmptyEntries).Length == 0) return true;
        }

        return false;
    }
}