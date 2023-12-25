using AdventOfCode2023.Core.Day18;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day18;

public class Day18SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day18Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("62");
    }
    
    [Theory]
    [InlineData("""
                D 2
                U 2
                """, "3")]
    [InlineData("""
                D 2
                R 2
                L 4
                R 2
                U 2
                """, "7")]
    [InlineData("""
                D 4
                U 2
                R 2
                L 4
                R 2
                U 2
                """, "9")]
    public void FirstSolution_VertSpike_Solves(string input, string expected)
    {
        var solution = new Day18Solution(input.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries), _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe(expected);
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 18);

        var solution = new Day18Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("53844");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day18Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("952408144115");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day18Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("2428");
        // 2408 too low
        // 4242 wrong
    }
    
    [Fact]
    public void SecondSolution_Ring_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day18Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("62");
    }
    
    [Fact]
    public void SecondSolution_Pyramid_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day18Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("38");
    }
    
    [Fact]
    public void SecondSolution_Egg_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day18Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("50");
    }
    
    [Fact]
    public void SecondSolution_TwoCompartments_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day18Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("70");
    }
    
    [Fact]
    public void SecondSolution_SnakingExit_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day18Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("82");
    }
}