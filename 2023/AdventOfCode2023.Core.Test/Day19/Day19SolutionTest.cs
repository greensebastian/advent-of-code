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

    [Fact(Skip = "Very slow")]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day19Solution(input, _ => {});

        var actual = solution.SecondSolution("32").ToList();

        actual.Single().ShouldBe((56 * 62).ToString());
    }

    [Fact(Skip = "Very slow")]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day19Solution(input, _ => {});

        var actual = solution.SecondSolution("32").ToList();

        actual.Single().ShouldBe("31740");
        // 31740 CORRECT!!!
        // 31280 too low
        // 28152
        // 21896 too low
    }
}