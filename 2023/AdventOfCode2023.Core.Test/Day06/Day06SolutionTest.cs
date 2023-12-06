using AdventOfCode2023.Core.Day06;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day06;

public class Day06SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var inputLines = Util.ReadFromFile("input");

        var solution = new Day06Solution(inputLines, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("288");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day06");

        var solution = new Day06Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("1892");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var inputLines = Util.ReadFromFile("input");

        var solution = new Day06Solution(inputLines, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("abc");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day06");

        var solution = new Day06Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("2313");
    }
}