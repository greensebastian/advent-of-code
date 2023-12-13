using AdventOfCode2023.Core.Day12;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day12;

public class Day12SolutionTest
{
    [Theory]
    [InlineData("?.# 1,1", 1)]
    [InlineData("???.### 1,1,3", 1)]
    [InlineData("????.#...#...?????.#...#...?????.#...#...?????.#...#...?????.#...#... 4,1,1,4,1,1,4,1,1,4,1,1,4,1,1", 16)]
    public void FirstSolution_Specific_Solves(string input, int expected)
    {
        var solution = new Day12Solution(new []{input}, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe(expected.ToString());
    }
    
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day12Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("21");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 12);

        var solution = new Day12Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("6935");
    }
    
    [Theory]
    [InlineData("???.### 1,1,3", 1)]
    [InlineData(".??..??...?##. 1,1,3", 16384)]
    [InlineData("?#?#?#?#?#?#?#? 1,3,1,6", 1)]
    [InlineData("????.#...#... 4,1,1", 16)]
    [InlineData("????.######..#####. 1,6,5", 2500)]
    [InlineData("?###???????? 3,2,1", 506250)]
    [InlineData("?????.??.???. 1,1,1", 211315169336)]
    public void SecondSolution_Specific_Solves(string input, long expected)
    {

        var solution = new Day12Solution(new []{input}, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe(expected.ToString());
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day12Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("525152");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 12);

        var solution = new Day12Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        // 211315169336 Too low
        actual.Single().ShouldBe("3920437278260");
    }
}