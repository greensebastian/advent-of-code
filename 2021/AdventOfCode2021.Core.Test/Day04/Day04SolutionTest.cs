using AdventOfCode2021.Core.Day04;
using Shouldly;

namespace AdventOfCode2021.Core.Test.Day04;

public class Day04SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day04Solution(input);

        var actual = solution.FirstSolution().ToList();
        
        actual.Single().ShouldBe("4512");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day04Solution(input);

        var actual = solution.FirstSolution().ToList();
        
        actual.Single().ShouldBe("5685");
    }
    
    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day04Solution(input);

        var actual = solution.SecondSolution().ToList();
        
        actual.Single().ShouldBe("1924");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day04Solution(input);

        var actual = solution.SecondSolution().ToList();
        
        actual.Single().ShouldBe("21070");
    }
}