using System.Text.RegularExpressions;

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
        var set = SequenceSet.FromInput(Input.ToArray());
        yield return set.FocusingPower().ToString();
    }
}

public record SequenceSet(Sequence[] Sequences)
{
    public int HashSum() => Sequences.Select(s => s.Hash()).Sum();

    public long FocusingPower()
    {
        var map = new HashMap();
        foreach (var se in Sequences)
        {
            map.Process(se.Value);
        }

        return map.FocusingPower();
    }
    
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

public record HashMap()
{
    private static Regex _digitRegex = new Regex("^[a-zA-Z]+", RegexOptions.Compiled);
    private Dictionary<int, List<string>> _map = new ();
    
    public void Process(string op)
    {
        var label = _digitRegex.Match(op).Value;
        var hash = Hash(label);
        if (!_map.ContainsKey(hash)) _map[hash] = new List<string>();

        var lenses = _map[hash];

        var action = op[label.Length];
        if (action == '-')
        {
            lenses.RemoveAll(l => l.StartsWith(label));
        }

        if (action == '=')
        {
            var toAdd = $"{label} {op[(label.Length + 1)..]}";
            var i = lenses.FindIndex(l => l.StartsWith(label));
            if (i >= 0) lenses[i] = toAdd;
            else lenses.Add(toAdd);
        }
    }

    public long FocusingPower()
    {
        var p = 0L;
        foreach (var (key, lenses) in _map)
        {
            for (var i = 0; i < lenses.Count; i++)
            {
                p += (1 + key) * (1 + i) * lenses[i].Ints().Single();
            }
        }

        return p;
    }
    
    private static int Hash(string s)
    {
        var hash = 0;
        foreach (var c in s)
        {
            hash = ((hash + c) * 17) % 256;
        }

        return hash;
    }
}