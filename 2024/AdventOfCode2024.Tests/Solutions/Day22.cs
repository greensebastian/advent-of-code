using FluentAssertions;
// ReSharper disable InvalidXmlDocComment

namespace AdventOfCode2024.Tests.Solutions;

public class Day22 : ISolution
{
    private const string Example = """
                                   1
                                   10
                                   100
                                   2024
                                   """;

    private const string ShortExample = """
                                        123
                                        """;

    private const string SecondExample = """
                                         1
                                         2
                                         3
                                         2024
                                         """;

    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day22");

        var sum = new MonkeyExchange(input).HashSum();
        sum.Should().Be(14082561342L);
    }

    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw(SecondExample);
        var input = Util.ReadFile("day22");

        var sum = new MonkeyExchange(input).GetBestBananaPurchases();
        sum.Should().Be(1568L);
    }

    private class MonkeyExchange(IReadOnlyList<string> input)
    {
        public long HashSum()
        {
            var sum = 0L;
            foreach (var line in input)
            {
                var initial = long.Parse(line);
                var secret = initial;
                for (var i = 0; i < 2000; i++)
                {
                    secret = Hash(secret);
                }

                sum += secret;
            }

            return sum;
        }

        private record ChangeSequence(short A, short B, short C, short D)
        {
            public static ChangeSequence FromEnumerable(IEnumerable<long> source)
            {
                var s = source.ToArray();
                return new ChangeSequence((short)(s[1] - s[0]), (short)(s[2] - s[1]), (short)(s[3] - s[2]), (short)(s[4] - s[3]));
            }
        }
        
        public long GetBestBananaPurchases()
        {
            var pointsByChangeSequence = new Dictionary<long, Dictionary<ChangeSequence, int>>();
            foreach (var line in input)
            {
                var initial = long.Parse(line);
                var monkeyResult = new Dictionary<ChangeSequence, int>();
                pointsByChangeSequence[initial] = monkeyResult;
                var secret = initial;
                var pricesTail = new LinkedList<long>();
                pricesTail.AddLast(secret % 10);
                for (var i = 0; i < 2000; i++)
                {
                    secret = Hash(secret);
                    var price = secret % 10;
                    pricesTail.AddLast(price);
                    if (pricesTail.Count == 5)
                    {
                        var changes = ChangeSequence.FromEnumerable(pricesTail);
                        monkeyResult.TryAdd(changes, (int)price);
                        pricesTail.RemoveFirst();
                    }
                }
            }

            var seenSequences = pointsByChangeSequence.SelectMany(kv => kv.Value.Keys).Distinct().ToList();
            var best = 0;
            foreach (var sequence in seenSequences)
            {
                if (sequence.D < 0) continue;
                var sum = pointsByChangeSequence.Select(m => m.Value.GetValueOrDefault(sequence, 0)).Sum();
                if (sum > best) best = sum;
            }

            return best;
        }

        private long Hash(long value)
        {
            const long prune = 16777216;
            var result = value;
            result ^= result << 6;
            result %= prune;
            result ^= result >> 5;
            result %= prune;
            result ^= result << 11;
            result %= prune;
            return result;
        }
    }
}