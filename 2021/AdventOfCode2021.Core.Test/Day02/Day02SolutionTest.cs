using AdventOfCode2021.Core.Day02;
using AdventOfCode2021.Core.ExampleDay;
using Shouldly;

namespace AdventOfCode2021.Core.Test.Day02;

public class Day02SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day02Solution(input);

        var actual = solution.FirstSolution().ToList();
        
        actual.Single().ShouldBe("150");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day02Solution(input);

        var actual = solution.FirstSolution().ToList();
        
        actual.Single().ShouldBe("2272262");
    }
    
    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day02Solution(input);

        var actual = solution.SecondSolution().ToList();
        
        actual.Single().ShouldBe("900");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day02Solution(input);

        var actual = solution.SecondSolution().ToList();
        
        actual.Single().ShouldBe("2134882034");
    }
}