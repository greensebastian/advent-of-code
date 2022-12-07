using AdventOfCode2021.Core.Day08;
using Shouldly;

namespace AdventOfCode2021.Core.Test.Day08;

public class Day08SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day08Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("26");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day08Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("349");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day08Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("61229");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day08Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("1070957");
    }
}