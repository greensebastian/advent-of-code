using AdventOfCode2023.Core.Day03;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day03;

public class Day03SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day03Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("4361");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day03");

        var solution = new Day03Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        // 516889 wrong
        actual.Single().ShouldBe("517021");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day03Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("467835");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day03");

        var solution = new Day03Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("81296995");
    }
}