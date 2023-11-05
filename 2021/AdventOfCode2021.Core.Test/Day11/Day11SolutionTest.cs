using AdventOfCode2021.Core.Day11;
using Shouldly;

namespace AdventOfCode2021.Core.Test.Day11;

public class Day11SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day11Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("1656");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day11");

        var solution = new Day11Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("1603");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day11Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day11Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }
}