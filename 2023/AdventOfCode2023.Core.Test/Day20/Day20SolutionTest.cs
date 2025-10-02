using AdventOfCode2023.Core.Day20;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day20;

public class Day20SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day20Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("32000000");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFile("day20");

        var solution = new Day20Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("806332748");
        // 17793 Too high
    }

    [Fact(Skip="Not relevant")]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day20Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("1623178306");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFile("day20");

        var solution = new Day20Solution(input, _ => {});

        var actual = solution.SecondSolution("811589153", "10").ToList();

        actual.Single().ShouldBe("228060006554227");
    }
}