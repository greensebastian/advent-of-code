using AdventOfCode2023.Core.Day06;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day06;

public class Day06SolutionTest
{
    [Theory]
    [InlineData("mjqjpqmgbljsphdztnvjfqwrcgsmlb", "7")]
    [InlineData("bvwbjplbgvbhsrlpgdmjqwftvncz", "5")]
    [InlineData("nppdvjthqldpwncqszvftbrmjlhg", "6")]
    [InlineData("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", "10")]
    [InlineData("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", "11")]
    public void FirstSolution_Example_Solves(string input, string expectedCharactersBeforeMarker)
    {
        var inputLines = new[] { input };

        var solution = new Day06Solution(inputLines, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe(expectedCharactersBeforeMarker);
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day06Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("1892");
    }

    [Theory]
    [InlineData("mjqjpqmgbljsphdztnvjfqwrcgsmlb", "19")]
    [InlineData("bvwbjplbgvbhsrlpgdmjqwftvncz", "23")]
    [InlineData("nppdvjthqldpwncqszvftbrmjlhg", "23")]
    [InlineData("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", "29")]
    [InlineData("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", "26")]
    public void SecondSolution_Example_Solves(string input, string expectedCharactersBeforeMarker)
    {
        var inputLines = new[] { input };

        var solution = new Day06Solution(inputLines, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe(expectedCharactersBeforeMarker);
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day06Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("2313");
    }
}