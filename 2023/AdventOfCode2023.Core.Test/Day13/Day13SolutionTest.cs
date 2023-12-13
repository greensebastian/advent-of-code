using AdventOfCode2023.Core.Day13;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day13;

public class Day13SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day13Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("405");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 13);

        var solution = new Day13Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("32723");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day13Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("140");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 13);

        var solution = new Day13Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("22713");
    }
}