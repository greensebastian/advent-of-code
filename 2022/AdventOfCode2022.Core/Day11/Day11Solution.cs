namespace AdventOfCode2022.Core.Day11;

public record Day11Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var monkeys = new MonkeyGroup(Input);

        for (int i = 0; i < 20; i++)
        {
            monkeys.DoRound();
        }
        
        yield return monkeys.Business.ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        yield return "0";
    }
}

public class MonkeyGroup
{
    private List<Monkey> Monkeys { get; } = new();
    public long Business => Monkeys
        .Select(monkey => monkey.Inspections)
        .OrderByDescending(inspections => inspections)
        .Take(2)
        .Aggregate(1L, (agg, curr) => agg * curr);
    
    public MonkeyGroup(IEnumerable<string> input)
    {
        foreach (var monkeyInput in input.Chunk(7))
        {
            Monkeys.Add(new Monkey(monkeyInput));
        }
    }

    public void DoRound()
    {
        var business = new List<long>();
        foreach (var monkey in Monkeys)
        {
            while (monkey.Items.TryPeek(out _))
            {
                var worry = monkey.Items.Dequeue();
                var newWorry = monkey.Inspect(worry) / 3;
                monkey.Inspections++;
                var target = monkey.Test(newWorry) ? monkey.TargetIfTrue : monkey.TargetIfFalse;
                Monkeys[target].Items.Enqueue(newWorry);
            }
        }
    }
}

public class Monkey
{
    public Queue<long> Items { get; } = new();
    public Func<long, long> Inspect { get; }
    public Func<long, bool> Test { get; }
    public int TargetIfTrue { get; }
    public int TargetIfFalse { get; }
    public int Inspections { get; set; }
    
    public Monkey(string[] lines)
    {
        // Starting items
        foreach (var worry in lines[1].Ints())
        {
            Items.Enqueue(worry);
        }

        // Operation
        var opIndex = lines[2].IndexOf("old", StringComparison.InvariantCultureIgnoreCase) + 4;
        var targetIndex = opIndex + 2;
        long? opAmount = null;
        var amountString = lines[2][targetIndex..];
        if (amountString != "old")
        {
            opAmount = long.Parse(amountString);
        }
        var op = lines[2][opIndex];
        switch (op)
        {
            case '+':
                Inspect = worry => worry + (opAmount ?? worry);
                break;
            case '*':
                Inspect = worry => worry * (opAmount ?? worry);
                break;
            default:
                throw new ArgumentException("Impossible operation");
        }
        
        // Test
        Test = worry => worry % lines[3].Ints().Single() == 0;
        
        // Targets
        TargetIfTrue = lines[4].Ints().Single();
        TargetIfFalse = lines[5].Ints().Single();
    }
}

internal static class EnumerableExtensions
{
    public static IEnumerable<int> Ints(this IEnumerable<char> source)
    {
        var currentNumber = string.Empty;
        foreach (var c in source)
        {
            if (char.IsNumber(c))
            {
                currentNumber += c;
            }
            else
            {
                if (string.IsNullOrEmpty(currentNumber)) continue;
                
                yield return int.Parse(currentNumber);
                currentNumber = string.Empty;
            }
        }

        if (currentNumber.Length > 0) yield return int.Parse(currentNumber);
    }
}