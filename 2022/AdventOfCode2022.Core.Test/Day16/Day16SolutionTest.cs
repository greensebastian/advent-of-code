using AdventOfCode2022.Core.Day16;
using Shouldly;

namespace AdventOfCode2022.Core.Test.Day16;

public class Day16SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day16Solution(input);

        var actual = solution.FirstSolution("5").ToList();

        actual.Single().ShouldBe("1651");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day16Solution(input);

        var actual = solution.FirstSolution("11").ToList();

        actual.Single().ShouldBe("2359");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day16Solution(input);

        var actual = solution.SecondSolution("10", "10000").ToList();

        actual.Single().ShouldBe("1706");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day16Solution(input);

        var actual = solution.SecondSolution("11", "1000").ToList();

        actual.Single().ShouldBe("2999");
        // 2656 too low
        // 2839 too low
    }
}