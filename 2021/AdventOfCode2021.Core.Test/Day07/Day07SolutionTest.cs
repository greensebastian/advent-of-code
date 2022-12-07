using AdventOfCode2021.Core.Day07;
using Shouldly;

namespace AdventOfCode2021.Core.Test.Day07;

public class Day07SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day07Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("37");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day07Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("344535");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day07Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("168");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day07Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("95581659");
    }
}