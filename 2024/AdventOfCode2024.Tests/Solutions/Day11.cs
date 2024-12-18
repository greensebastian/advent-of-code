using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day11 : ISolution
{
    private const string Example = """
                                   125 17
                                   """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day11");

        var stones = Stone.Parse(input.Single()).ToArray();
        //Print(stones);
        var sum = stones.Sum(stone => StoneCount(stone.Value, 25));
        sum.Should().Be(198075);
    }

    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day11");

        var stones = Stone.Parse(input.Single()).ToArray();
        //Print(stones);
        var sum = stones.Sum(stone => StoneCount(stone.Value, 75));
        sum.Should().Be(235571309320764L);
    }

    private void Print(IEnumerable<Stone> stones) => Console.WriteLine(string.Join(" ", stones.Select(s => s.Value)));

    private Dictionary<(long, int), long> _countCache = new();
    
    private long StoneCount(long stoneValue, int depth)
    {
        if (_countCache.TryGetValue((stoneValue, depth), out var cachedValue)) return cachedValue;
        long returnValue;
        if (depth == 0) returnValue = 1;
        else if (stoneValue == 0) returnValue = StoneCount(1, depth - 1);
        else
        {
            var toString = stoneValue.ToString();
            if (toString.Length % 2 == 0)
            {
                returnValue = StoneCount(long.Parse(toString[..(toString.Length / 2)]), depth - 1) +
                       StoneCount(long.Parse(toString[(toString.Length / 2)..]), depth - 1);
            }
            else
            {
                returnValue = StoneCount(stoneValue * 2024, depth - 1);
            }
        }

        _countCache[(stoneValue, depth)] = returnValue;
        return returnValue;
    }
    
    private record Stone(long Value)
    {
        public static IEnumerable<Stone> Parse(string input)
        {
            return input.Split(" ").Select(str => new Stone(long.Parse(str)));
        }
    }
}