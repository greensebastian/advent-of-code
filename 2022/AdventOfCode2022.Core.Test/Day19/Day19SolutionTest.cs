using AdventOfCode2022.Core.Day19;
using Shouldly;

namespace AdventOfCode2022.Core.Test.Day19;

public class Day19SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day19Solution(input);

        var actual = solution.FirstSolution("24", "1000").ToList();

        actual.Single().ShouldBe("33");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day19Solution(input);

        var actual = solution.FirstSolution("24").ToList();

        actual.Single().ShouldBe("0");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day19Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day19Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }
}