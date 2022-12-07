using AdventOfCode2021.Core.Day21;
using Shouldly;

namespace AdventOfCode2021.Core.Test.Day21;

public class Day21SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day21Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("0");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day21Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("0");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day21Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day21Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }
}