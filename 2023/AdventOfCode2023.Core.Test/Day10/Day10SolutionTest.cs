using System.Text;
using AdventOfCode2023.Core.Day10;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day10;

public class Day10SolutionTest
{
    [Fact]
    public void Memory_Example_AllCyclesOk()
    {
        var memory = new Memory();
        memory.CyclesStarted.ShouldBe(0);
        memory.X.ShouldBe(1);

        var expected = new Dictionary<long, long>()
        {
            { 1, 1 },
            { 2, 1 },
            { 3, 1 },
            { 4, 4 },
            { 5, 4 }
        };

        var cyclesSeen = new List<long>();
        
        foreach (var cycle in memory.Do(Operation.From("noop")))
        {
            cyclesSeen.Add(cycle);
            memory.X.ShouldBe(expected[cycle]);
        }

        foreach (var cycle in memory.Do(Operation.From("addx 3")))
        {
            cyclesSeen.Add(cycle);
            memory.X.ShouldBe(expected[cycle]);
        }
        
        foreach (var cycle in memory.Do(Operation.From("addx -5")))
        {
            cyclesSeen.Add(cycle);
            memory.X.ShouldBe(expected[cycle]);
        }
        
        cyclesSeen.Count.ShouldBe(5);
        memory.X.ShouldBe(-1);
    }
    
    [Fact]
    public void FirstSolution_SmallExample_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day10Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.ShouldHaveSingleItem();
    }
    
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day10Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("13140");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day10Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("11720");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day10Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();
        
        var answer = Util.ReadFromFile("answer");
        
        string.Join("|", actual).ShouldBe(string.Join("|", answer));
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day10Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        Console.OutputEncoding = Encoding.Unicode;
        foreach (var line in actual)
        {
            var lineInUnicode = string.Join("", line.Select(c => c == '#' ? "⬜" : "⬛"));
            solution.Log.Invoke(lineInUnicode);
        }
        
        var answer = Util.ReadFromFile("answer");
        
        string.Join("|", actual).ShouldBe(string.Join("|", answer));
    }
}