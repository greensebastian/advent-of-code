using AdventOfCode2022.Core.Day24;
using Shouldly;

namespace AdventOfCode2022.Core.Test.Day24;

public class Day24SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day24Solution(input, Console.WriteLine);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("18");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day24");

        var solution = new Day24Solution(input, Console.WriteLine);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("274");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day24Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("54");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day24");

        var solution = new Day24Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }
}