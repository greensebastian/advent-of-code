using System.Globalization;

namespace AdventOfCode2021.Core.Day10;

public record Day10Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var collection = new ChunkSequenceCollection(Input);

        yield return collection.GetScore().ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        yield return "0";
    }
}

public class ChunkSequenceCollection
{
    public List<ChunkSequence> Sequences { get; } = new();
    public ChunkSequenceCollection(IEnumerable<string> input)
    {
        Sequences.AddRange(input.Select(line => new ChunkSequence(line)));
    }

    public int GetScore() => Sequences.Sum(seq => seq.GetIllegalCharScore());
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

    public static IReadOnlyDictionary<char, int> Scoring { get; } = new Dictionary<char, int>
    {
        { ')', 3 },
        { ']', 57 },
        { '}', 1197 },
        { '>', 25137 }
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

    public int GetIllegalCharScore()
    {
        var pos = IllegalCharPos;
        return pos.HasValue ? Scoring[Chars[pos.Value]] : 0;
    }
}