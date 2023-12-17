using AdventOfCode2023.Core.Day17;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day17;

public class Day17SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day17Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("102");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 17);

        var solution = new Day17Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("3200");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day17Solution(input, _ => {});

        var actual = solution.SecondSolution("1000000000000").ToList();

        actual.Single().ShouldBe("1514285714288");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day17Solution(input, _ => {});

        var actual = solution.SecondSolution("1000000000000").ToList();

        actual.Single().ShouldBe("1584927536247");
    }
}