using System.Globalization;

namespace AdventOfCode2022.Core.Day21;

public record Day21Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var monkeys = new Dictionary<string, Monkey>();
        var dependencies = new HashSet<string>();
        foreach (var line in Input)
        {
            var m = new Monkey(line);
            monkeys.Add(m.Name, m);
            if (m.Left is not null && m.Right is not null)
            {
                var leftUnique = dependencies.Add(m.Left);
                var rightUnique = dependencies.Add(m.Right);
                if (!leftUnique || !rightUnique)
                    throw new ArgumentException($"Many monkey depend on {m.Left} or {m.Right}");
            }
        }

        var rootMonkey = new TreeMonkey(monkeys["root"], monkeys);
        var rootValue = rootMonkey.GetValue();

        yield return rootValue.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return "0";
    }
}

public class Monkey
{
    public Monkey(string input)
    {
        Name = input[..4];
        if (char.IsNumber(input[6]))
        {
            Value = long.Parse(input[6..]);
        }
        else
        {
            Operation = input[11];
            Left = input[6..10];
            Right = input[13..];
        }
    }
    public string? Left { get; }
    public string? Right { get; }
    public string Name { get; }
    public long? Value { get; }
    public char? Operation { get; }
}

public class TreeMonkey
{
    private TreeMonkey? Left { get; }
    private TreeMonkey? Right { get; }
    private string Name { get; }
    private long? Value { get; set; }
    private char? Operation { get; }
    
    public TreeMonkey(Monkey m, Dictionary<string, Monkey> monkeys)
    {
        Name = m.Name;
        Value = m.Value;
        Operation = m.Operation;
        if (m.Left is not null)
            Left = new TreeMonkey(monkeys[m.Left], monkeys);
        if (m.Right is not null)
            Right = new TreeMonkey(monkeys[m.Right], monkeys);
    }

    public long GetValue()
    {
        if (Value.HasValue)
            return Value.Value;

        var value = Operation switch
        {
            '+' => Left!.GetValue() + Right!.GetValue(),
            '-' => Left!.GetValue() - Right!.GetValue(),
            '*' => Left!.GetValue() * Right!.GetValue(),
            '/' => Left!.GetValue() / Right!.GetValue(),
            '%' => Left!.GetValue() % Right!.GetValue(),
            _ => throw new ArgumentOutOfRangeException()
        };

        Value = value;
        return value;
    }
}