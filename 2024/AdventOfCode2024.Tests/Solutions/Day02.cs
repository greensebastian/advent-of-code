using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day02 : ISolution
{
    [Fact]
    public void Solution1()
    {
        /*var input = Util.ReadRaw("""
                                 7 6 4 2 1
                                 1 2 7 8 9
                                 9 7 6 2 1
                                 1 3 2 4 5
                                 8 6 4 4 1
                                 1 3 6 7 9
                                 """);*/

        var input = Util.ReadFile("day02");

        var successful = input.Select(l => l.Split(" ").Select(int.Parse).ToArray()).Select(levels =>
        {
            var dir = levels[1] - levels[0];
            for (var i = 1; i < levels.Length; i++)
            {
                var diff = levels[i] - levels[i - 1];
                if (Math.Abs(diff) > 3 || Math.Abs(diff) < 1) return false;
                if (dir > 0 && diff < 0) return false;
                if (dir < 0 && diff > 0) return false;
            }

            return true;
        }).Count(s => s);

        successful.Should().Be(383);
    }

    private (bool safe, int? failedIndex) LevelsSafe(int[] levels)
    {
        var dir = levels[1] - levels[0];
        for (var i = 1; i < levels.Length; i++)
        {
            var diff = levels[i] - levels[i - 1];
            if (Math.Abs(diff) > 3 || Math.Abs(diff) < 1) return (false, i);
            if (dir > 0 && diff < 0) return (false, i);
            if (dir < 0 && diff > 0) return (false, i);
        }
        return (true, null);
    }

    [Fact]
    public void Solution2()
    {
        /*var input = Util.ReadRaw("""
                                 7 6 4 2 1
                                 1 2 7 8 9
                                 9 7 6 2 1
                                 1 3 2 4 5
                                 8 6 4 4 1
                                 1 3 6 7 9
                                 """);*/
        
        var input = Util.ReadFile("day02");

        var successful = input.Select(l => l.Split(" ").Select(int.Parse).ToArray()).Select(levels =>
        {
            var (safe, failedIndex) = LevelsSafe(levels);
            if (safe) return true;

            for (int i = 0; i < levels.Length; i++)
            {
                var newLevels = levels.Where((l, place) => place != i).ToArray();
                if (LevelsSafe(newLevels).safe) return true;
            }

            return false;
        }).Count(s => s);

        successful.Should().Be(436);
        // not 437
        // not 513
        // not 421
        // not 421
        // not 434
    }
}