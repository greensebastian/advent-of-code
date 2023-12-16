namespace AdventOfCode2023.Core.Day15;

public record Day15Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var set = SequenceSet.FromInput(Input.ToArray());
        yield return set.HashSum().ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return 0.ToString();
    }
}

public record SequenceSet(Sequence[] Sequences)
{
    public int HashSum() => Sequences.Select(s => s.Hash()).Sum();
    
    public static SequenceSet FromInput(IEnumerable<string> lines)
    {
        return new SequenceSet(lines.SelectMany(l => l.Split(',', StringSplitOptions.TrimEntries).Select(op => new Sequence(op))).ToArray());
    }
}

public record Sequence(string Value)
{
    public int Hash()
    {
        var hash = 0;
        foreach (var c in Value)
        {
            hash = ((hash + c) * 17) % 256;
        }

        return hash;
    }
}