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

        actual[0].ShouldBe("1320");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 15);

        var solution = new Day15Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual[0].ShouldBe("509784");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day15Solution(input, _ => {});

        var actual = solution.SecondSolution("20").ToList();

        actual.Single().ShouldBe("145");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 15);

        var solution = new Day15Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("230197");
    }
}