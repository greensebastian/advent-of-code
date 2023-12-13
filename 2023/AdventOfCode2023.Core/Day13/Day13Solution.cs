namespace AdventOfCode2023.Core.Day13;

public record Day13Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var boards = Board.FromWholeInput(Input).ToArray();
        var pivots = boards.Select(b => b.LinesBeforeReflection()!.Value).ToArray();
        yield return pivots.Sum().ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var boards = Board.FromWholeInputPermutations(Input).ToArray();
        var pivots = boards.Select(b => (b.board.LinesBeforeReflection(), b.board.LinesBeforeReflectionMutated(b.permutations))).ToArray();
        yield return pivots.Select(p => p.Item2).Sum().ToString();
    }
}

public record Board(ulong[] Rows, ulong[] Cols)
{
    public override string ToString() => $"[{string.Join(", ", Rows)}], [{string.Join(", ", Cols)}]";

    public int LinesBeforeReflectionMutated(IEnumerable<Board> permutations)
    {
        var bench = LinesBeforeReflection();
        foreach (var board in permutations)
        {
            var other = board.LinesBeforeReflection(bench);
            if (other.HasValue && other != bench) return other.Value;
        }

        return bench!.Value;
    }
    
    public int? LinesBeforeReflection(int? toIgnore = null)
    {
        var colPivot = FindPivot(Cols, toIgnore);
        return colPivot ?? FindPivot(Rows, toIgnore / 100) * 100;
    }

    private static int? FindPivot(ulong[] input, int? toIgnore = null)
    {
        for (var i = 1; i < input.Length; i++)
        {
            if (input[i] != input[i - 1]) continue;

            if (IsMirror(input, i - 1, i))
            {
                if (toIgnore != i) return i;
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

    private static (Board, IEnumerable<Board>) FromInputPermutation(char[][] lines)
    {
        var toChange = new List<(int row, int col)>();
        for (var row = 0; row < lines.Length; row++)
        {
            for (var col = 0; col < lines[row].Length; col++)
            {
                toChange.Add((row, col));
            }
        }

        var ordinary = FromInput(lines);
        var permutations = new List<Board>();
        foreach (var change in toChange)
        {
            var origin = lines[change.row][change.col];
            lines[change.row][change.col] = origin == '#' ? '.' : '#';
            permutations.Add(FromInput(lines));
            lines[change.row][change.col] = origin == '.' ? '.' : '#';
        }

        return (ordinary, permutations);
    }
    
    private static Board FromInput(char[][] lines)
    {
        var rows = new List<ulong>();
        for (var row = 0; row < lines.Length; row++)
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
            for (var row = 0; row < lines.Length; row++)
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
                yield return FromInput(batch.Select(l => l.ToCharArray()).ToArray());
                batch.Clear();
            }
            else
            {
                batch.Add(line);
            }
        }

        yield return FromInput(batch.Select(l => l.ToCharArray()).ToArray());
    }
    
    public static IEnumerable<(Board board, IEnumerable<Board> permutations)> FromWholeInputPermutations(IEnumerable<string> lines)
    {
        var batch = new List<string>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                yield return FromInputPermutation(batch.Select(l => l.ToCharArray()).ToArray());
                batch.Clear();
            }
            else
            {
                batch.Add(line);
            }
        }

        yield return FromInputPermutation(batch.Select(l => l.ToCharArray()).ToArray());
    }
}