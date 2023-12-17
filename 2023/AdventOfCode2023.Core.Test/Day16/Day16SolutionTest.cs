using AdventOfCode2023.Core.Day16;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day16;

public class Day16SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day16Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("46");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 16);

        var solution = new Day16Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("2359");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day16Solution(input, _ => {});

        var actual = solution.SecondSolution("100", "100000").ToList();

        actual.Single().ShouldBe("1706");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day16Solution(input, _ => {});

        var actual = solution.SecondSolution("11", "1000").ToList();

        actual.Single().ShouldBe("2999");
        // 2656 too low
        // 2839 too low
    }
    
    [Fact(Skip = "Bonus")]
    public void FirstSolution_Bonus1_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day16Solution(input, _ => {});

        var actual = solution.FirstSolution("11", "1000000").ToList();

        actual.Single().ShouldBe("2640");
    }
    
    [Fact(Skip = "Bonus")]
    public void FirstSolution_Bonus2_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day16Solution(input, _ => {});

        var actual = solution.FirstSolution("11", "10000").ToList();

        actual.Single().ShouldBe("13468");
    }
    
    [Fact(Skip = "Bonus")]
    public void FirstSolution_Bonus3_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day16Solution(input, _ => {});

        var actual = solution.FirstSolution("11", "10000").ToList();

        actual.Single().ShouldBe("1288");
    }
    
    [Fact(Skip = "Bonus")]
    public void FirstSolution_Bonus4_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day16Solution(input, _ => {});

        var actual = solution.FirstSolution("11", "10000").ToList();

        actual.Single().ShouldBe("2400");
    }
    
    [Fact(Skip = "Bonus")]
    public void SecondSolution_Bonus1_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day16Solution(input, _ => {});

        var actual = solution.SecondSolution("10", "100000").ToList();

        actual.Single().ShouldBe("2670");
    }
    
    [Fact(Skip = "Bonus")]
    public void SecondSolution_Bonus2_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day16Solution(input, _ => {});

        var actual = solution.SecondSolution("11", "1000").ToList();

        actual.Single().ShouldBe("12887");
    }
    
    [Fact(Skip = "Bonus")]
    public void SecondSolution_Bonus3_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day16Solution(input, _ => {});

        var actual = solution.SecondSolution("11", "1000").ToList();

        actual.Single().ShouldBe("1484");
    }
    
    [Fact(Skip = "Bonus")]
    public void SecondSolution_Bonus4_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day16Solution(input, _ => {});

        var actual = solution.SecondSolution("11", "1000").ToList();

        actual.Single().ShouldBe("3680");
    }
}