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

    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day22");

        var sum = new MonkeyExchange(input).HashSum(2000);
        sum.Should().Be(37327623);
    }

    [Fact]
    public void Solution2()
    {
        var input = Util.ReadFile("day21");

        false.Should().Be(false);
    }

    private class MonkeyExchange(IReadOnlyList<string> input)
    {
        public long HashSum(int iteration)
        {
            var sum = 0L;
            foreach (var line in input)
            {
                var initial = long.Parse(line);
                var secret = initial;
                for (var i = 0; i < iteration; i++)
                {
                    secret = Hash(secret);
                }

                sum += secret;
            }

            return sum;
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