using AdventOfCode2023.Core.Day11;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day11;

public class Day11SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day11Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("374");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 11);

        var solution = new Day11Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("9565386");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day11Solution(input, _ => {});

        var actual10X = solution.SecondSolution("10").ToList();
        var actual100X = solution.SecondSolution("100").ToList();

        actual10X.Single().ShouldBe("1030");
        actual100X.Single().ShouldBe("8410");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 11);

        var solution = new Day11Solution(input, _ => {});

        var actual = solution.SecondSolution("1000000").ToList();

        actual.Single().ShouldBe("857986849428");
    }
}