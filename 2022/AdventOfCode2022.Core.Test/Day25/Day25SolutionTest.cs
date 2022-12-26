using AdventOfCode2022.Core.Day25;
using Shouldly;

namespace AdventOfCode2022.Core.Test.Day25;

public class Day25SolutionTest
{
    [Theory]
    [InlineData(10, "20")]
    [InlineData(11, "21")]
    [InlineData(12, "22")]
    [InlineData(13, "1==")]
    [InlineData(-12, "==")]
    [InlineData(-13, "-22")]
    public void Convert_To_Snafu(long value, string expectedSnafu)
    {
        var actual = SnafuConverter.Convert(value);
        actual.ShouldBe(expectedSnafu);
    }
    
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day25Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("2=-1=0");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFile("day25");

        var solution = new Day25Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("0");
        // 27766274000 wrong
        // 6291437520 Wrong
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day25Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day25Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }
}