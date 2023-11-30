using AdventOfCode2023.Core.Day15;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day15;

public class Day15SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day15Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual[0].ShouldBe("26");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day15Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual[1].ShouldBe("5108096");
        // 4945034 too low
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day15Solution(input, _ => {});

        var actual = solution.SecondSolution("20").ToList();

        actual.Single().ShouldBe("56000011");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day15Solution(input, _ => {});

        var actual = solution.SecondSolution("4000000").ToList();

        actual.Single().ShouldBe("10553942650264");
    }
}