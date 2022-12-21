namespace AdventOfCode2022.Core.Day21;

public record Day21Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var monkeys = new MonkeyGroup(Input.ToArray());
        
        var rootValue = monkeys.Root.GetValue();

        yield return rootValue.ToString();
    }

    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var monkeys = new MonkeyGroup(Input.ToArray());

        var myValue = monkeys.GetValueForEqualityAt(args[0]);

        yield return myValue.ToString();
    }
}

public record Operation(decimal? Left, char Symbol, decimal? Right)
{
    public override string ToString() => $"{Left?.ToString() ?? "x"} {Symbol} {Right?.ToString() ?? "x"}";
}

public class MonkeyGroup
{
    public TreeMonkey Root { get; }
    
    public MonkeyGroup(string[] input)
    {
        var monkeys = ParseMonkeys(input);
        Root = new TreeMonkey(monkeys["root"], monkeys, null);
    }
    
    private Dictionary<string, Monkey> ParseMonkeys(string[] input)
    {
        var monkeys = new Dictionary<string, Monkey>();
        var dependencies = new HashSet<string>();
        foreach (var line in input)
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

        return monkeys;
    }
    
    public decimal GetValueForEqualityAt(string myName)
    {
        var me = Root.Find(myName)!;

        var operations = me.GetOperationsTo("root").ToList();
        var invertedOperations = Invert(operations);
        var operationsToDo = Do(invertedOperations);

        var otherRoot = Root.Left!.Find(myName) is not null
            ? Root.Right!
            : Root.Left!;
        var sumToMatch = otherRoot.GetValue();

        var myValue = sumToMatch;
        foreach (var op in operationsToDo)
        {
            myValue = op(myValue);
        }

        return myValue;
    }
    
    private IEnumerable<Operation> Invert(IList<Operation> operations)
    {
        foreach (var op in operations.Reverse())
        {
            if (op.Left is not null)
            {
                var value = op.Left.Value;
                yield return op.Symbol switch
                {
                    '+' => new Operation(null, '-',value),
                    '-' => new Operation(value, '-', null),
                    '*' => new Operation(null, '/', value),
                    '/' => new Operation(value, '/', null),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            else if (op.Right is not null)
            {
                var value = op.Right!.Value;
                yield return op.Symbol switch
                {
                    '+' => new Operation(null, '-', value),
                    '-' => new Operation(null, '+', value),
                    '*' => new Operation(null, '/', value),
                    '/' => new Operation(null, '*', value),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }

    private IEnumerable<Func<decimal, decimal>> Do(IEnumerable<Operation> operations)
    {
        foreach (var op in operations)
        {
            if (op.Left is not null)
            {
                var left = op.Left.Value;
                yield return op.Symbol switch
                {
                    '+' => old => left + old,
                    '-' => old => left - old,
                    '*' => old => left * old,
                    '/' => old => left / old,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            else if (op.Right is not null)
            {
                var right = op.Right!.Value;
                yield return op.Symbol switch
                {
                    '+' => old => old + right,
                    '-' => old => old - right,
                    '*' => old => old * right,
                    '/' => old => old / right,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}

public class Monkey
{
    public Monkey(string input)
    {
        Name = input[..4];
        if (char.IsNumber(input[6]))
        {
            Value = decimal.Parse(input[6..]);
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
    public decimal? Value { get; }
    public char? Operation { get; }
}

public class TreeMonkey
{
    private TreeMonkey? Parent { get; }
    public TreeMonkey? Left { get; }
    public TreeMonkey? Right { get; }
    public string Name { get; }
    private decimal? Value { get; set; }
    private char? Operation { get; }
    
    public TreeMonkey(Monkey m, Dictionary<string, Monkey> monkeys, TreeMonkey? parent)
    {
        Parent = parent;
        Name = m.Name;
        Value = m.Value;
        Operation = m.Operation;
        if (m.Left is not null)
            Left = new TreeMonkey(monkeys[m.Left], monkeys, this);
        if (m.Right is not null)
            Right = new TreeMonkey(monkeys[m.Right], monkeys, this);
    }

    public IEnumerable<Operation> GetOperationsTo(string name)
    {
        var current = Parent;
        var prev = this;
        while (current is not null && current.Name != name)
        {
            var left = prev == current.Left ? (decimal?)null : current.Left!.GetValue();
            var right = prev == current.Right ? (decimal?)null : current.Right!.GetValue();
            yield return new Operation(left, current.Operation!.Value, right);
            prev = current;
            current = current.Parent;
        }
    }
    
    public TreeMonkey? Find(string name)
    {
        if (Name == name) return this;
        if (Left is not null)
        {
            var fromLeft = Left.Find(name);
            if (fromLeft is not null) return fromLeft;
        }

        if (Right is not null)
        {
            var fromRight = Right.Find(name);
            if (fromRight is not null) return fromRight;
        }

        return null;
    }

    public decimal GetValue(bool recompute = false)
    {
        if (Value.HasValue && !recompute)
            return Value.Value;

        var value = Operation switch
        {
            '+' => Left!.GetValue(recompute) + Right!.GetValue(recompute),
            '-' => Left!.GetValue(recompute) - Right!.GetValue(recompute),
            '*' => Left!.GetValue(recompute) * Right!.GetValue(recompute),
            '/' => Left!.GetValue(recompute) / Right!.GetValue(recompute),
            '%' => Left!.GetValue(recompute) % Right!.GetValue(recompute),
            _ => throw new ArgumentOutOfRangeException()
        };

        Value = value;
        return value;
    }

    public override string ToString()
    {
        var left = Left?.Name ?? string.Empty;
        var right = Right?.Name ?? string.Empty;
        var op = Operation?.ToString() ?? string.Empty;
        var value = GetValue();
        return $"{Name}: {value} [{left} {op} {right}]";
    }
}