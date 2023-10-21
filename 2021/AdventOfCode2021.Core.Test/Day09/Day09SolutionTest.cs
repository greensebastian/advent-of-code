using AdventOfCode2021.Core.Day09;
using Shouldly;

namespace AdventOfCode2021.Core.Test.Day09;

public class Day09SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day09Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("15");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day9");

        var solution = new Day09Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("491");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day09Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("1134");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day09Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }
}