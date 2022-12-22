using AdventOfCode2022.Core.Day22;
using Shouldly;

namespace AdventOfCode2022.Core.Test.Day22;

public class Day22SolutionTest
{
    private string ExpectedScore(int row, int col, int dirScore) => (1000 * row + 4 * col + dirScore).ToString();
    
    [Fact]
    public void FirstSolution_WrapToWallAtRight_Solves()
    {
        var input = new[]
        {
            "..",
            "#.",
            "",
            "1R1L1"
        };

        var solution = new Day22Solution(input, _ => { });
        
        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe(ExpectedScore(2, 2, 0));
    }
    
    [Fact]
    public void FirstSolution_WrapToWallAtLeft_Solves()
    {
        var input = new[]
        {
            "..",
            ".#",
            "",
            "R1R1"
        };

        var solution = new Day22Solution(input, _ => { });
        
        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe(ExpectedScore(2, 1, 2));
    }
    
    [Fact]
    public void FirstSolution_WrapToWallAtTop_Solves()
    {
        var input = new[]
        {
            ".",
            "#",
            "",
            "L1"
        };

        var solution = new Day22Solution(input, _ => { });
        
        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe(ExpectedScore(1, 1, 3));
    }
    
    [Fact]
    public void FirstSolution_WrapToWallAtBot_Solves()
    {
        var input = new[]
        {
            ".#",
            "..",
            "",
            "R1L1R1"
        };

        var solution = new Day22Solution(input, _ => { });
        
        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe(ExpectedScore(2, 2, 1));
    }
    
    [Fact]
    public void FirstSolution_BumpAllWalls_Solves()
    {
        var input = new[]
        {
            "..#",
            " ..",
            " #.#",
            "  #",
            "",
            "1R1L1R1L2R3R4R70"
        };

        var solution = new Day22Solution(input, _ => { });
        
        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe(ExpectedScore(2, 3, 3));
    }
    
    [Fact]
    public void FirstSolution_WrapAllDirections_Solves()
    {
        var input = new[]
        {
            "...",
            "  .",
            "  .",
            "",
            "3RR2LRRR1L5RR2"
        };

        var solution = new Day22Solution(input, _ => { });
        
        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe(ExpectedScore(1, 3, 1));
    }
    
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day22Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("6032");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day22");

        var solution = new Day22Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("122082");
        // 12534 too low
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day22Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("5031");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day22");

        var solution = new Day22Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }
}