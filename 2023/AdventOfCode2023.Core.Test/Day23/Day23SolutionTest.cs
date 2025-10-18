using AdventOfCode2023.Core.Day23;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day23;

public class Day23SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day23Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("94");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFile("day23");

        var solution = new Day23Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("2182");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day23Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("154");
    }

    [Fact(Skip = "unoptimized")]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFile("day23");

        var solution = new Day23Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("6670");
    }
}