using AdventOfCode2023.Core.Day19;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day19;

public class Day19SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day19Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("19114");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 19);

        var solution = new Day19Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("418498");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day19Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe(167409079868000L.ToString());
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFile("day19");

        var solution = new Day19Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe(123331556462603L.ToString());
    }
}