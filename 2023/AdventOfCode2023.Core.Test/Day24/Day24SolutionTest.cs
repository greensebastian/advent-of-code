using AdventOfCode2023.Core.Day24;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day24;

public class Day24SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day24Solution(input, Console.WriteLine);

        var actual = solution.FirstSolution("7", "27").ToList();

        actual.Single().ShouldBe("2");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFile("day24");

        var solution = new Day24Solution(input, _ => {});

        var actual = solution.FirstSolution("200000000000000", "400000000000000").ToList();

        actual.Single().ShouldBe("17906");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day24Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("571093786416929");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFile("day24");

        var solution = new Day24Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("571093786416929");
    }
}