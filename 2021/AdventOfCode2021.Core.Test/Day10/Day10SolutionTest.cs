using AdventOfCode2021.Core.Day10;
using Shouldly;

namespace AdventOfCode2021.Core.Test.Day10;

public class Day10SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day10Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("26397");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day10");

        var solution = new Day10Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("240123");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day10Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("288957");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day10");

        var solution = new Day10Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("3260812321");
    }
}