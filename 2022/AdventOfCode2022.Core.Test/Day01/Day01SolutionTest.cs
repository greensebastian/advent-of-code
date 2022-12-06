using AdventOfCode2022.Core.Day01;
using Shouldly;

namespace AdventOfCode2022.Core.Test.Day01;

public class Day01SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day01Solution(input);

        var actual = solution.FirstSolution().ToList();
        actual.First().ShouldBe("4");
        actual.Last().ShouldBe("24000");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day01Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.First().ShouldBe("238");
        actual.Last().ShouldBe("68442");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day01Solution(input);

        var actual = solution.SecondSolution().ToList();
        actual.First().ShouldBe("4, 3, 5");
        actual.Last().ShouldBe("45000");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day01Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.First().ShouldBe("238, 240, 27");
        actual.Last().ShouldBe("204837");
    }
}