using AdventOfCode2023.Core.Day25;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day25;

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

    [Theory]
    [InlineData(10, "20")]
    [InlineData(11, "21")]
    [InlineData(12, "22")]
    [InlineData(13, "1==")]
    [InlineData(-12, "==")]
    [InlineData(-13, "-22")]
    [InlineData(27766274000, "10=--=12=2==2000")]
    public void Convert_TwoWays_Succeeds(long dec, string snafu)
    {
        SnafuConverter.Convert(dec).ShouldBe(snafu);
        SnafuConverter.Convert(snafu).ShouldBe(dec);
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

        actual.Single().ShouldBe("2-121-=10=200==2==21");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day25Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }

    [Fact(Skip = "No 25th part 2")]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day25Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }
}