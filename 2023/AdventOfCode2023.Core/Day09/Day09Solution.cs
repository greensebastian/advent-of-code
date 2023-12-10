namespace AdventOfCode2023.Core.Day09;

public record Day09Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var sequences = HistorySequences.FromInput(Input);
        yield return sequences.SumNexts().ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var sequences = HistorySequences.FromInput(Input);
        yield return sequences.SumPrevs().ToString();
    }
}

public record HistorySequences(IList<HistorySequence> Sequences)
{
    public long SumNexts()
    {
        var sum = 0L;
        foreach (var sequence in Sequences)
        {
            var partialSum = sequence.GetNext();
            sum += partialSum;
        }

        return sum;
    }
    
    public long SumPrevs()
    {
        var sum = 0L;
        foreach (var sequence in Sequences)
        {
            var partialSum = sequence.GetPrevious();
            sum += partialSum;
        }

        return sum;
    }
    
    public static HistorySequences FromInput(IEnumerable<string> lines) =>
        new (lines.Select(HistorySequence.FromInput).ToArray());
}

public record HistorySequenceSet(IList<HistorySequence> Sequences);

public record HistorySequence(IList<long> InitialRange)
{
    public override string ToString() => string.Join(", ", InitialRange);

    private long Last { get; } = InitialRange[^1];

    public long GetNext()
    {
        var expanded = ExpandInitialRange();

        for (var i = expanded.Sequences.Count - 2; i >= 0; i--)
        {
            var toExtend = expanded.Sequences[i];
            var below = expanded.Sequences[i + 1];
            toExtend.InitialRange.Add(toExtend.InitialRange[^1] + below.InitialRange[^1]);
        }

        var toReturn = expanded.Sequences[0].InitialRange[^1];
        return toReturn;
    }
    
    public long GetPrevious()
    {
        var expanded = ExpandInitialRange();

        for (var i = expanded.Sequences.Count - 2; i >= 0; i--)
        {
            var toExtend = expanded.Sequences[i];
            var below = expanded.Sequences[i + 1];
            var toInsert = toExtend.InitialRange[0] - below.InitialRange[0];
            toExtend.InitialRange.Insert(0, toInsert);
        }

        var toReturn = expanded.Sequences[0].InitialRange[0];
        return toReturn;
    }

    private HistorySequenceSet ExpandInitialRange()
    {
        var expanded = new HistorySequenceSet(new List<HistorySequence>());
        var range = InitialRange.ToList();
        while (true)
        {
            expanded.Sequences.Add(new HistorySequence(range));

            if (range.All(n => n == 0)) break;

            range = GetDiffs(range).ToList();
        }

        return expanded;
    }

    private IEnumerable<long> GetDiffs(IList<long> sequence)
    {
        for (var i = 1; i < sequence.Count; i++)
        {
            yield return sequence[i] - sequence[i - 1];
        }
    }
    
    public static HistorySequence FromInput(string line) => new(line.Longs().ToArray());
}