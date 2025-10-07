using AdventOfCode2023.Core.Day22;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day22;

public class Day22SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day22Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();
        
        actual.Single().ShouldBe("5");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFile("day22");

        var solution = new Day22Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("465");
        // 475 too high
        // 470 too high
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day22Solution(input, _ => {});

        var actual = solution.SecondSolution("example").ToList();

        actual.Single().ShouldBe("5031");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day22");

        var solution = new Day22Solution(input, _ => {});

        var actual = solution.SecondSolution("real").ToList();

        actual.Single().ShouldBe("134076");
    }
}