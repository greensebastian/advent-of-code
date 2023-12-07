using AdventOfCode2023.Core.Day01;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day01;

public class Day01SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day01Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();
        actual.First().ShouldBe("142");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 1);

        var solution = new Day01Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.First().ShouldBe("54561");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day01Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();
        actual.First().ShouldBe("281");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day01");

        var solution = new Day01Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.First().ShouldBe("54076");
    }
}