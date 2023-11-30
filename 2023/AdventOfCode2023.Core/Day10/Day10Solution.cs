namespace AdventOfCode2023.Core.Day10;

public record Day10Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var sum = 0L;
        var memory = new Memory();
        foreach (var line in Input)
        {
            var op = Operation.From(line);
            foreach (var cycle in memory.Do(op))
            {
                if ((cycle - 20) % 40 == 0)
                {
                    sum += cycle * memory.X;
                }
            }
        }

        yield return sum.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var memory = new Memory();
        var outputLine = string.Empty;
        foreach (var line in Input)
        {
            var op = Operation.From(line);
            foreach (var cycle in memory.Do(op))
            {
                var xPos = (cycle-1) % 40;
                var diff = Math.Abs(xPos - memory.X);
                outputLine += diff < 2 ? "#" : ".";
                if (xPos == 39)
                {
                    yield return outputLine;
                    outputLine = string.Empty;
                }
            }
        }
    }
}

public record Operation(int Cycles, Action<Memory>? StartOfOperation = null, Action<Memory>? EndOfOperation = null)
{
    public static Operation From(string input)
    {
        var opName = input[..4];
        switch (opName)
        {
            case "noop":
                return new Operation(1);
            case "addx":
                var opArg = long.Parse(input[5..]);
                return new Operation(2, null, memory => memory.Add(opArg));
            default:
                throw new ArgumentException($"Invalid operation '{opName}'");
        }
    }
}

public class Memory
{
    public long X { get; private set; } = 1;
    
    public long CyclesStarted { get; private set; } = 0;
    
    public void Add(long delta) => X += delta;

    public IEnumerable<long> Do(Operation op)
    {
        op.StartOfOperation?.Invoke(this);
        for (var cycle = 0; cycle < op.Cycles; cycle++)
        {
            CyclesStarted += 1;
            yield return CyclesStarted;
        }
        op.EndOfOperation?.Invoke(this);
    }
}