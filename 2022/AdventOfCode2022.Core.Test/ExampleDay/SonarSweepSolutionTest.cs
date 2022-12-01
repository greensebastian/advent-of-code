using AdventOfCode2022.Core.ExampleDay;
using Shouldly;

namespace AdventOfCode2022.Core.Test.ExampleDay;

public class SonarSweepSolutionTest
{
    [Fact]
    public void Solve_SingleMeasurement_Zero()
    {
        var input = Util.ReadFromFile("input");

        var solution = new SonarSweepSolution(input);

        var actual = solution.FirstSolution();
        actual.Single().ShouldBe("0");
    }

    [Fact]
    public void Solve_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new SonarSweepSolution(input);

        var actual = solution.FirstSolution();
        actual.Single().ShouldBe("1374");
    }
}