using System.Numerics;

namespace AdventOfCode2022.Core.Day25;

public record Day25Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var sum = BigInteger.Zero;
        foreach (var line in Input)
        {
            sum += SnafuConverter.Convert(line);
        }

        var snafu = SnafuConverter.Convert(sum);

        yield return snafu;
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

    public static BigInteger Convert(string snafu)
    {
        var m = BigInteger.One;
        var sum = BigInteger.Zero;
        foreach (var c in snafu.Reverse())
        {
            sum += Snafu2Decimal[c] * m;
            m *= 5;
        }

        return sum;
    }

    public static string Convert(BigInteger value)
    {
        var digits = 0;
        var offset = 0L;
        var foundEnd = false;
        while (!foundEnd)
        {
            digits++;
            var factor = (long)Math.Pow(5, digits - 1);
            offset -= factor * 2;
            var reachable = offset + factor * 5 - 1;
            if (value >= offset && value <= reachable)
            {
                foundEnd = true;
            }
        }

        var sum = new BigInteger(offset);
        var snafu = "";
        for (var place = 0; place < digits; place++)
        {
            var factor = BigInteger.Pow(5, digits - place - 1);
            for (var multiple = 0; multiple < 5; multiple++)
            {
                var remainder = value - (sum + multiple * factor);
                if (BigInteger.Abs(remainder) < factor)
                {
                    snafu += "=-012"[multiple];
                    sum += multiple * factor;
                    break;
                }
            }
        }

        return snafu;
    }
}