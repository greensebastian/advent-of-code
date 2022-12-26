using System.Collections.ObjectModel;
using System.Globalization;

namespace AdventOfCode2022.Core.Day25;

public record Day25Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var sum = 0L;
        foreach (var line in Input)
        {
            sum += SnafuConverter.Convert(line);
        }

        yield return sum.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return "0";
    }
}

public static class SnafuConverter
{
    private static IReadOnlyDictionary<char, int> Snafu2Decimal { get; } = new Dictionary<char, int>
    {
        { '2', 2 },
        { '1', 1 },
        { '0', 0 },
        { '-', -1 },
        { '=', -2 }
    };

    public static long Convert(string snafu)
    {
        var m = 1;
        var sum = 0L;
        foreach (var c in snafu.Reverse())
        {
            sum += Snafu2Decimal[c] * m;
            m *= 5;
        }

        return sum;
    }

    public static string Convert(long value)
    {
        var abs = Math.Abs(value);
        var maxMultiple = 1;
        var reachable = 2;
        while (reachable < abs)
        {
            maxMultiple *= 5;
            reachable += 2 * maxMultiple;
        }

        var remainder = value;
        var nextMultiple = maxMultiple / 5;
        var nextReachable = reachable - 1 - 2 * nextMultiple;

        return "";
    }
}