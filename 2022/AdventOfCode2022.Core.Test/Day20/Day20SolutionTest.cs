using AdventOfCode2022.Core.Day20;
using Shouldly;

namespace AdventOfCode2022.Core.Test.Day20;

public class Day20SolutionTest
{
    [Fact]
    public void MixerList_TwoEntries_Works()
    {
        var list = new MixerList(new[] { "1", "2" });
        list.ToString().ShouldBe("(2):\t[1, 2]");
    }
    
    [Fact]
    public void MixerList_ThreeEntries_Works()
    {
        var list = new MixerList(new[] { "1", "2", "3" });
        list.ToString().ShouldBe("(3):\t[1, 2, 3]");
    }
    
    [Fact]
    public void MixerList_MoveToStart_Works()
    {
        var list = new MixerList(new[] { "1", "2", "3" });
        list.ToString().ShouldBe("(3):\t[1, 2, 3]");

        var three = list.First().Next.Next;
        three.Value.ShouldBe(3);
        
        three.MoveUp(-2);
        list.ToString().ShouldBe("(3):\t[1, 2, 3]");
        
        three.MoveUp(-3);
        list.ToString().ShouldBe("(3):\t[1, 3, 2]");
    }
    
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day20Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("3");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day20Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("13967");
        // 17793 Too high
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day20Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day20Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }
}