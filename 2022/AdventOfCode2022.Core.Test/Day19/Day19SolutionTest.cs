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

        var actual = solution.FirstSolution("24", "200000").ToList();

        actual.Single().ShouldBe("0");
        // 1228 too low
        // 1205 too low
        // 1091 too low
        // 821 too low
        // 568 too low
        // 513 too low
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