using AdventOfCode2023.Core.Day05;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day05;

public class Day05SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day05Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("35");

        var map = MappingTable.FromInputLines(input.ToList());
        var mapped = map.MapToEnd(82);
        var reversed = map.MapToStart(46, "location");
        mapped.ShouldBe(46uL);
        reversed.ShouldBe(82uL);
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day05");

        var solution = new Day05Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("650599855");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day05Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("46");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day05");

        var solution = new Day05Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        // 63651375 Too high
        // 2464306 Too high
        // 12077938 Too high
        // 11291003 wrong
        // 1240035
        actual.Single().ShouldBe("1240035");
    }
}