using AdventOfCode2021.Core.Day06;
using Shouldly;

namespace AdventOfCode2021.Core.Test.Day06;

public class Day06SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day06Solution(input);

        var actual = solution.FirstSolution().ToList();
        
        actual.Single().ShouldBe("5934");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day06Solution(input);

        var actual = solution.FirstSolution().ToList();
        
        actual.Single().ShouldBe("373378");
    }
    
    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day06Solution(input);

        var actual = solution.SecondSolution().ToList();
        
        actual.Single().ShouldBe("26984457539");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day06Solution(input);

        var actual = solution.SecondSolution().ToList();
        
        actual.Single().ShouldBe("1682576647495");
    }
}