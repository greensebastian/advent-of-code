using AdventOfCode2023.Core.Day14;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day14;

public class Day14SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day14Solution(input, Console.WriteLine);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("136");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 14);

        var solution = new Day14Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("113424");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day14Solution(input, Console.WriteLine);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("64");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 14);

        var solution = new Day14Solution(input, Console.WriteLine);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("96003");
    }
}