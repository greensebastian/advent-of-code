using AdventOfCode2022.Core.Day23;
using Shouldly;

namespace AdventOfCode2022.Core.Test.Day23;

public class Day23SolutionTest
{
    [Fact]
    public void FirstSolution_SmallExample_Solves()
    {
        var input = new []
        {
            ".....",
            "..##.",
            "..#..",
            ".....",
            "..##.",
            "....."
        };

        var solution = new Day23Solution(input, _ => {});

        var actual = solution.FirstSolution("10").ToList();

        actual.Single().ShouldBe("25");
    }
    
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day23Solution(input, _ => {});

        var actual = solution.FirstSolution("10").ToList();

        actual.Single().ShouldBe("110");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day23");

        var solution = new Day23Solution(input, _ => {});

        var actual = solution.FirstSolution("10").ToList();

        actual.Single().ShouldBe("3871");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day23Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("20");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day23");

        var solution = new Day23Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("925");
    }
}