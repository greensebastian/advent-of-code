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
        actual.Single().ShouldBe("MHQTLJRLB");
    }
}