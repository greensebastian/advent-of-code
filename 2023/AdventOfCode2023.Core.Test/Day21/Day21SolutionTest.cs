using AdventOfCode2023.Core.Day21;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day21;

public class Day21SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day21Solution(input, _ => {});

        var actual = solution.FirstSolution("6").ToList();

        actual.Single().ShouldBe("16");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFile("day21");

        var solution = new Day21Solution(input, _ => {});

        var actual = solution.FirstSolution("64").ToList();

        actual.Single().ShouldBe("3788");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day21Solution(input, _ => {});

        var actual = solution.SecondSolution("humn").ToList();

        actual.Single().ShouldBe("301");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFileBySignature("input");

        var solution = new Day21Solution(input, _ => {});

        var actual = solution.SecondSolution("humn").ToList();

        actual.Single().ShouldBe("3343167719435");
    }
}