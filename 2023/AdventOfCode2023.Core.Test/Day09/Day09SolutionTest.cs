using AdventOfCode2023.Core.Day09;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day09;

public class Day09SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day09Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("13");
    }

    [Theory]
    [InlineData("U 1", "R 2")]
    [InlineData("U 1", "L 2")]
    [InlineData("R 1", "D 2")]
    [InlineData("R 1", "U 2")]
    [InlineData("D 1", "L 2")]
    [InlineData("D 1", "R 2")]
    [InlineData("L 1", "D 2")]
    [InlineData("L 1", "U 2")]
    public void FirstSolution_MovesDiagonal_Follows(params string[] input)
    {
        var solution = new Day09Solution(input, _ => {});
        solution.FirstSolution().Single().ShouldBe("2");
    }
    
    [Theory]
    [InlineData("U 2", "R 1")]
    [InlineData("U 2", "L 1")]
    [InlineData("R 2", "D 1")]
    [InlineData("R 2", "U 1")]
    [InlineData("D 2", "L 1")]
    [InlineData("D 2", "R 1")]
    [InlineData("L 2", "D 1")]
    [InlineData("L 2", "U 1")]
    public void FirstSolution_MovesKnight_Follows(params string[] input)
    {
        var solution = new Day09Solution(input, _ => {});
        solution.FirstSolution().Single().ShouldBe("2");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day09Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("6311");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day09Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("1");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day09Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("2482");
    }
}