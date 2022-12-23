using System.Globalization;

namespace AdventOfCode2022.Core.Day23;

public record Day23Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var rounds = int.Parse(args[0]);
        var map = new Map(Input.ToArray());
        for (var i = 0; i < rounds; i++)
        {
            foreach (var line in map.GetPrintLines())
            {
                Log(line);
            }
            map.DoRound();
        }
        
        yield return map.GetScore().ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var map = new Map(Input.ToArray());
        var rounds = 0;
        while (map.DoRound())
        {
            rounds++;
        }
        yield return (rounds + 1).ToString();
    }
}

public class Map
{
    public HashSet<Vector> Elves { get; } = new();
    public Vector GlobalMin { get; private set; }
    public Vector GlobalMax { get; private set; }

    public Map(string[] input)
    {
        for (var rowIndex = 0; rowIndex < input.Length; rowIndex++)
        {
            for (var colIndex = 0; colIndex < input[rowIndex].Length; colIndex++)
            {
                if (input[rowIndex][colIndex] == '#')
                    Elves.Add(new Vector(colIndex, rowIndex));
            }
        }
        SetMaxMin();
    }

    public int GetScore()
    {
        SetMaxMin();
        var empty = 0;
        for (var row = GlobalMin.Row; row <= GlobalMax.Row; row++)
        {
            for (var col = GlobalMin.Col; col <= GlobalMax.Col; col++)
            {
                if (!Elves.Contains(new Vector(col, row)))
                    empty++;
            }
        }

        return empty;
    }

    private bool HasNeighbour(Vector elf)
    {
        var neighbour = elf.East;
        if (Elves.Contains(neighbour)) return true;
        neighbour = neighbour.South;
        if (Elves.Contains(neighbour)) return true;
        neighbour = neighbour.West;
        if (Elves.Contains(neighbour)) return true;
        neighbour = neighbour.West;
        if (Elves.Contains(neighbour)) return true;
        neighbour = neighbour.North;
        if (Elves.Contains(neighbour)) return true;
        neighbour = neighbour.North;
        if (Elves.Contains(neighbour)) return true;
        neighbour = neighbour.East;
        if (Elves.Contains(neighbour)) return true;
        neighbour = neighbour.East;
        if (Elves.Contains(neighbour)) return true;
        return false;
    }

    public bool DoRound()
    {
        var proposed = new Dictionary<Vector, List<Vector>>();
        var propositionDirections = GetPropositionOrder().ToArray();
        foreach (var elf in Elves)
        {
            if (!HasNeighbour(elf))
                continue;
            
            foreach (var dir in propositionDirections)
            {
                var (firstDir, secondDir) = dir.Col == 0 ? (dir.West, dir.East) : (dir.North, dir.South);
                var middle = elf.Move(dir);
                var first = elf.Move(firstDir);
                var second = elf.Move(secondDir);
                if (Elves.Contains(first) || Elves.Contains(middle) || Elves.Contains(second))
                    continue;
                
                if (!proposed.ContainsKey(middle))
                    proposed[middle] = new List<Vector>();
                
                proposed[middle].Add(elf);
                break;
            }
        }

        var validMoves = proposed
            .Where(kv => kv.Value.Count == 1)
            .Select(kv => (Current: kv.Value.Single(), Target: kv.Key));

        var moved = false;
        foreach (var (current, target) in validMoves)
        {
            Elves.Remove(current);
            Elves.Add(target);
            moved = true;
        }

        return moved;
    }

    public IEnumerable<string> GetPrintLines()
    {
        SetMaxMin();
        var margin = 1;
        for (var row = GlobalMin.Row - margin; row <= GlobalMax.Row + margin; row++)
        {
            var line = $"{row}:\t";
            for (var col = GlobalMin.Col - margin; col <= GlobalMax.Col + margin; col++)
            {
                line += Elves.Contains(new Vector(col, row)) ? "#" : ".";
            }

            yield return line;
        }

        yield return string.Empty;
    }

    private int PropositionIndex { get; set; }
    private Vector[] PropositionOrder { get; } = {
        Vector.DirNorth,
        Vector.DirSouth,
        Vector.DirWest,
        Vector.DirEast
    };
    public IEnumerable<Vector> GetPropositionOrder()
    {
        // N - S - W - E
        for (var i = 0; i < 4; i++)
        {
            yield return PropositionOrder[(PropositionIndex + i) % 4];
        }

        PropositionIndex = (PropositionIndex + 1) % 4;
    }

    private void SetMaxMin()
    {
        var minCol = Elves.Min(e => e.Col);
        var maxCol = Elves.Max(e => e.Col);
        var minRow = Elves.Min(e => e.Row);
        var maxRow = Elves.Max(e => e.Row);
        GlobalMin = new Vector(minCol, minRow);
        GlobalMax = new Vector(maxCol, maxRow);
    }
}

public record Vector(long Col, long Row)
{
    public Vector South => this with { Row = Row + 1 };
    public Vector North => this with { Row = Row - 1 };
    public Vector West => this with { Col = Col - 1 };
    public Vector East => this with { Col = Col + 1 };
    public Vector To(Vector other) => new(other.Col - Col, other.Row - Row);
    public Vector Move(Vector other) => new(Col + other.Col, Row + other.Row);
    public Vector Move(long col, long row) => new(Col + col, Row + row);
    public Vector Inverse => new(-Col, -Row);

    public static readonly Vector DirEast = new(1, 0);
    public static readonly Vector DirSouth = new(0, 1);
    public static readonly Vector DirWest = new(-1, 0);
    public static readonly Vector DirNorth = new(0, -1);

    public override string ToString() => $"Row: {Row}, Col: {Col}";
}