using AdventOfCode2021.Core.Day03;
using Shouldly;

namespace AdventOfCode2021.Core.Test.Day03;

public class Day03SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day03Solution(input);

        var actual = solution.FirstSolution().ToList();
        
        actual.Single().ShouldBe("198");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day03Solution(input);

        var actual = solution.FirstSolution().ToList();
        
        actual.Single().ShouldBe("1131506");
    }
    
    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day03Solution(input);

        var actual = solution.SecondSolution().ToList();
        
        actual.Single().ShouldBe("230");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day03Solution(input);

        var actual = solution.SecondSolution().ToList();
        
        actual.Single().ShouldBe("7863147");
    }
}