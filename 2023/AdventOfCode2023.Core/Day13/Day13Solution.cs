namespace AdventOfCode2023.Core.Day13;

public record Day13Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var boards = Board.FromWholeInput(Input).ToArray();
        var pivots = boards.Select(b => b.LinesBeforeReflection()).ToArray();
        yield return pivots.Sum().ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return 0.ToString();
    }
}

public record Board(ulong[] Rows, ulong[] Cols)
{
    public override string ToString() => $"[{string.Join(", ", Rows)}], [{string.Join(", ", Cols)}]";

    public int LinesBeforeReflection()
    {
        var colPivot = FindPivot(Cols);
        return colPivot ?? FindPivot(Rows)!.Value * 100;
    }

    private static int? FindPivot(ulong[] input)
    {
        for (var i = 1; i < input.Length; i++)
        {
            if (input[i] != input[i - 1]) continue;

            if (IsMirror(input, i - 1, i))
            {
                return i;
            }
        }
        
        return null;
    }

    private static bool IsMirror(ulong[] input, int left, int right)
    {
        while (left >= 0 && right < input.Length)
        {
            if (input[left] != input[right]) return false;
            left--;
            right++;
        }

        return true;
    }
    
    public static Board FromInput(IList<string> lines)
    {
        var rows = new List<ulong>();
        for (var row = 0; row < lines.Count; row++)
        {
            ulong c = 0;
            for (var col = 0; col < lines[row].Length; col++)
            {
                c = c << 1 | (lines[row][col] == '#' ? 1uL : 0);
            }
            rows.Add(c);
        }

        var cols = new List<ulong>();
        for (var col = 0; col < lines[0].Length; col++)
        {
            ulong c = 0;
            for (var row = 0; row < lines.Count; row++)
            {
                c = c << 1 | (lines[row][col] == '#' ? 1uL : 0);
            }
            cols.Add(c);
        }

        return new Board(rows.ToArray(), cols.ToArray());
    }

    public static IEnumerable<Board> FromWholeInput(IEnumerable<string> lines)
    {
        var batch = new List<string>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                yield return FromInput(batch);
                batch.Clear();
            }
            else
            {
                batch.Add(line);
            }
        }

        yield return FromInput(batch);
    }
}