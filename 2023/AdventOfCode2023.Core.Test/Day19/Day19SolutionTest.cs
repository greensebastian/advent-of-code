using AdventOfCode2023.Core.Day19;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day19;

public class Day19SolutionTest
{
    [Fact(Skip = "Very slow")]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day19Solution(input, _ => {});

        var actual = solution.FirstSolution("24", "10000000").ToList();

        actual.Single().ShouldBe("33");
        //actual.Single().ShouldBe("24");
        //actual.Single().ShouldBe("9");
    }

    [Fact(Skip = "Very slow")]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day19Solution(input, _ => {});

        var actual = solution.FirstSolution("24", "10000000").ToList();

        actual.Single().ShouldBe("1382");
        // 1382 CORRECT!!!
        // 1370 ?
        // 1356 wrong
        // 1341 wrong
        // 1228 wrong
        // 1205 wrong
        // 1174 ?
        // 1091 too low
        // 821 too low
        // 568 too low
        // 513 too low
    }

    [Fact(Skip = "Very slow")]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day19Solution(input, _ => {});

        var actual = solution.SecondSolution("32").ToList();

        actual.Single().ShouldBe((56 * 62).ToString());
    }

    [Fact(Skip = "Very slow")]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day19Solution(input, _ => {});

        var actual = solution.SecondSolution("32").ToList();

        actual.Single().ShouldBe("31740");
        // 31740 CORRECT!!!
        // 31280 too low
        // 28152
        // 21896 too low
    }
}