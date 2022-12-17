using AdventOfCode2022.Core.Day17;
using Shouldly;

namespace AdventOfCode2022.Core.Test.Day17;

public class Day17SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day17Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("3068");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day17Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("3200");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day17Solution(input);

        var actual = solution.SecondSolution("1000000000").ToList();

        actual.Single().ShouldBe("1514285714288");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day17Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }
}