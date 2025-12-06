using Shouldly;

namespace AdventOfCode2025.Tests.Day06;

public class Day06
{
    private const string Example = """
                                   123 328  51 64 
                                    45 64  387 23 
                                     6 98  215 314
                                   *   +   *   +  
                                   """;
    
    [Fact]
    public void Example_1()
    {
        var lines = Util.ReadRaw(Example);
        var ws = new Worksheet(lines);
        ws.ProblemResult().ShouldBe(4277556);
    }
    
    [Fact]
    public void Real_1()
    {
        var lines = Util.ReadFile("day06");
        var ws = new Worksheet(lines);
        ws.ProblemResult().ShouldBe(6171290547579L);
        
    }
    
    [Fact]
    public void Example_2()
    {
        var lines = Util.ReadRaw(Example, false);
        var ws = new CephWorksheet(lines);
        ws.ProblemResult().ShouldBe(3263827);
    }
    
    [Fact]
    public void Real_2()
    {
        var lines = Util.ReadFile("day06", false);
        var ws = new CephWorksheet(lines);
        ws.ProblemResult().ShouldBe(8811937976367L);
    }
}

public class Worksheet(IReadOnlyList<string> input)
{
    public IReadOnlyList<Column> Columns { get; } = GetColumns(input).ToList();

    private static IEnumerable<Column> GetColumns(IReadOnlyList<string> input)
    {
        var numLength = input.Count - 1;
        var nums = input.SkipLast(1).Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()).ToArray();
        var isMult = input[numLength].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => s == "*").ToArray();

        for (var i = 0; i < isMult.Length; i++)
        {
            yield return new Column(nums.Select(ns => ns[i]).ToArray(), isMult[i]);
        }
    }

    public long ProblemResult() => Columns.Select(c => c.Result()).Sum();
}

public record Column(int[] Nums, bool IsMult)
{
    public long Result() => IsMult ? Nums.Aggregate(1L, (a, b) => a * b) : Nums.Aggregate(0L, (a, b) => a + b);
}

public class CephWorksheet(IReadOnlyList<string> input)
{
    public long ProblemResult()
    {
        var sum = 0L;
        var res = 0L;
        var isMult = false;
        for (var col = 0; col < input[0].Length; col++)
        {
            var inLine = input.Select(l => l[col]).ToArray();
            if (inLine.All(c => c == ' '))
            {
                sum += res;
                res = 0;
                continue;
            }

            if (inLine.Last() == '*')
            {
                isMult = true;
                res = 1;
            }
            else if (inLine.Last() == '+')
            {
                isMult = false;
                res = 0;
            }

            var val = string.Join("", inLine.SkipLast(1).Where(c => c != ' '));
            var multText = isMult ? "*" : "+";
            Console.WriteLine($"Res: {res} {multText} {val}");
            if (!string.IsNullOrWhiteSpace(val)) res = isMult ? res * int.Parse(val) : res + int.Parse(val);
        }

        if (res != 0) sum += res;

        return sum;
    }
}