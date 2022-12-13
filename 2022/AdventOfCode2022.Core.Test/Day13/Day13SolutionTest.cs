using AdventOfCode2022.Core.Day13;
using Shouldly;

namespace AdventOfCode2022.Core.Test.Day13;

public class Day13SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day13Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("13");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day13Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("5808");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day13Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("140");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day13Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("22713");
    }
}