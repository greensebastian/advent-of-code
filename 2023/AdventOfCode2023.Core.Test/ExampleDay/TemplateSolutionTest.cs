using AdventOfCode2023.Core.ExampleDay;
using Shouldly;

namespace AdventOfCode2023.Core.Test.ExampleDay;

public class TemplateSolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new TemplateSolution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("0");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("input");

        var solution = new TemplateSolution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("0");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new TemplateSolution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("input");

        var solution = new TemplateSolution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }
}