using AdventOfCode2023.Core.Day21;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day21;

public class Day21SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day21Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("152");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFileBySignature("input");

        var solution = new Day21Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("168502451381566");
    }
    
    [Fact]
    public void SecondSolution_Operations_Solves()
    {
        var input = Util.ReadFromFile("input");

        var monkeys = new MonkeyGroup(input.ToArray());
        var me = monkeys.Root.Find("me__")!;

        var actual = me.GetOperationsTo("root");

        var operations = string.Join(", ", actual);
        
        operations.ShouldBe("x / 5, x * 4, x - 3, x + 2");

        var res = monkeys.GetValueForEqualityAt("me__");
        res.ShouldBe(2.5m);
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day21Solution(input, _ => {});

        var actual = solution.SecondSolution("humn").ToList();

        actual.Single().ShouldBe("301");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.ReadFromCachedFileBySignature("input");

        var solution = new Day21Solution(input, _ => {});

        var actual = solution.SecondSolution("humn").ToList();

        actual.Single().ShouldBe("3343167719435");
    }
}