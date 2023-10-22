namespace AdventOfCode2021.Core.Day10;

public record Day10Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var collection = new ChunkSequenceCollection(Input);

        yield return collection.GetIllegalCharScore().ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        var collection = new ChunkSequenceCollection(Input);

        yield return collection.GetCompletionScore().ToString();
    }
}

public class ChunkSequenceCollection
{
    public List<ChunkSequence> Sequences { get; } = new();
    public ChunkSequenceCollection(IEnumerable<string> input)
    {
        Sequences.AddRange(input.Select(line => new ChunkSequence(line)));
    }

    public int GetIllegalCharScore() => Sequences.Sum(seq => seq.GetIllegalCharScore());

    public long GetCompletionScore()
    {
        var scores = Sequences
            .Where(seq => seq.IllegalCharPos == null)
            .Select(seq => seq.GetCompletionScore())
            .OrderDescending()
            .ToList();
        return scores[scores.Count / 2];
    }
}

public class ChunkSequence
{
    private List<char> Chars { get; }

    private static IReadOnlyDictionary<char, char> OpenToClose { get; } = new Dictionary<char, char>
    {
        { '(', ')' },
        { '[', ']' },
        { '{', '}' },
        { '<', '>' }
    };

    public static IReadOnlyDictionary<char, int> InvalidCharScoring { get; } = new Dictionary<char, int>
    {
        { ')', 3 },
        { ']', 57 },
        { '}', 1197 },
        { '>', 25137 }
    };

    public static IReadOnlyDictionary<char, int> AutoCompleteScoring { get; } = new Dictionary<char, int>
    {
        { ')', 1 },
        { ']', 2 },
        { '}', 3 },
        { '>', 4 }
    };

    public ChunkSequence(string inputChars)
    {
        Chars = inputChars.ToList();
    }

    public int? IllegalCharPos
    {
        get
        {
            var complementStack = new Stack<char>();
            for(var i = 0; i < Chars.Count; i++)
            {
                var c = Chars[i];
                if (OpenToClose.ContainsKey(c))
                {
                    complementStack.Push(OpenToClose[c]);
                }
                else if (complementStack.Peek() == c)
                {
                    complementStack.Pop();
                }
                else
                {
                    return i;
                }
            }

            return null;
        }
    }

    public long GetCompletionScore()
    {
        var complementStack = new Stack<char>();
        foreach (var c in Chars)
        {
            if (OpenToClose.ContainsKey(c))
            {
                complementStack.Push(OpenToClose[c]);
            }
            else if (complementStack.Peek() == c)
            {
                complementStack.Pop();
            }
        }

        return complementStack.Aggregate(0L, NextScore);
    }

    private static long NextScore(long score, char nextChar)
    {
        return score * 5 + AutoCompleteScoring[nextChar];
    }

    public int GetIllegalCharScore()
    {
        var pos = IllegalCharPos;
        return pos.HasValue ? InvalidCharScoring[Chars[pos.Value]] : 0;
    }
}