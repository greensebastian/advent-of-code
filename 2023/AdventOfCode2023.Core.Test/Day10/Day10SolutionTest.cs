using AdventOfCode2023.Core.Day10;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day10;

public class Day10SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day10Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("8");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 10);

        var solution = new Day10Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("6842");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day10Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();
        
        actual.Single().ShouldBe("10");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 10);

        var solution = new Day10Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        // 2210 Too high
        // 500 Too high
        actual.Single().ShouldBe("393");
    }
}