using AdventOfCode2021.Core.Day01;
using Shouldly;

namespace AdventOfCode2021.Core.Test.Day01;

public class Day01SolutionTest
{
    [Fact]
    public void FirstSolution_SingleMeasurement_Zero()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day01Solution(input);

        var actual = solution.FirstSolution();
        actual.Single().ShouldBe("0");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day01Solution(input);

        var actual = solution.FirstSolution();
        actual.Single().ShouldBe("1374");
    }
    
    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day01Solution(input);

        var actual = solution.SecondSolution();
        actual.Single().ShouldBe("1418");
    }
}