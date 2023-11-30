namespace AdventOfCode2023.Core.Day24;

public record Day24Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var map = new BlizzardMap(Input.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray(), Log);

        var ans = map.SolveMaze();

        yield return ans.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var map = new BlizzardMap(Input.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray(), Log);

        var ans = map.SolveRoundTrips();

        yield return ans.ToString();
    }
}

public record PosDir(Vector Position, Vector Direction);

public record PosRound(Vector Position, int MapIndex)
{
    public required int MinutesPassed { get; init; }
}

public class BlizzardMap
{
    private readonly Action<string> _log;
    private readonly int _lcm;
    private List<Dictionary<Vector, char>> BlizzardsPerRound { get; } = new();
    private Dictionary<Vector, (Vector, char)> DirByBlizzardStartPos { get; } = new();
    private int Width { get; }
    private int Height { get; }
    private Vector Min { get; }
    private Vector Max { get; }
    private Vector Start { get; }
    private Vector End { get; }

    public BlizzardMap(string[] input, Action<string> log)
    {
        _log = log;
        Height = input.Length - 2;
        Width = input[0].Length - 2;
        Min = new Vector(1, 1);
        Max = Min.Move(Width - 1, Height - 1);
        Start = Min.North;
        End = Max.South;
        for (var row = 1; row <= Height; row++)
        {
            for (var col = 1; col <= Width; col++)
            {
                var c = input[row][col];
                if (c == '.') continue;
                var pos = new Vector(col, row);
                var dir = c switch
                {
                    '^' => Vector.DirNorth,
                    '>' => Vector.DirEast,
                    'v' => Vector.DirSouth,
                    '<' => Vector.DirWest,
                    _ => throw new ArgumentOutOfRangeException()
                };
                DirByBlizzardStartPos[pos] = (dir, c);
            }
        }

        _lcm = (int)Util.LowestCommonMultiple(Width, Height);
        for (var i = 0; i < _lcm; i++)
        {
            var blizzardsOnRound = GenerateBlizzardPositionsOnRound(i);
            BlizzardsPerRound.Add(blizzardsOnRound);
        }
    }

    public int SolveMaze()
    {
        var start = new PosRound(Start, 0) { MinutesPassed = 0 };
        return Travel(start, End).MinutesPassed;
    }

    public int SolveRoundTrips()
    {
        var start = new PosRound(Start, 0) { MinutesPassed = 0 };
        var first = Travel(start, End);
        var second = Travel(first, Start);
        var third = Travel(second, End);

        return third.MinutesPassed;
    }

    private PosRound Travel(PosRound start, Vector end)
    {
        var unexplored = new Queue<PosRound>();
        unexplored.Enqueue(start);
        var explored = new HashSet<PosRound> { unexplored.Peek() };
        var highestSeen = 0;
        while (unexplored.Count > 0)
        {
            var current = unexplored.Dequeue();
            if (current.MinutesPassed > highestSeen)
            {
                highestSeen = current.MinutesPassed;
                _log($"At {highestSeen}, {unexplored.Count} unexplored, {explored.Count} explored");
                Print(current);
            }

            // Print(current);
            if (current.Position == end)
                return current;

            var nextMinutesPassed = current.MinutesPassed + 1;
            var nextMapIndex = nextMinutesPassed % _lcm;
            var toCheck = current.Position.Neighbours.Concat(new[] { current.Position })
                .Where(n => 
                    n == end 
                    || !BlizzardsPerRound[nextMapIndex].ContainsKey(n) 
                    || (n == current.Position && !BlizzardsPerRound[nextMapIndex].ContainsKey(n)))
                .Where(n => InBounds(n, end, current.Position));
            foreach (var neighbour in toCheck)
            {
                var nextPosRound = new PosRound(neighbour, nextMapIndex) { MinutesPassed = nextMinutesPassed };
                if (explored.Contains(nextPosRound)) continue;

                explored.Add(nextPosRound);
                unexplored.Enqueue(nextPosRound);
            }
        }

        throw new ArgumentException("Cant find solution");
    }

    private bool InBounds(Vector v, Vector end, Vector current)
    {
        if (v == end) return true;
        if (v == current) return true;
        return v.Col >= Min.Col 
               && v.Col <= Max.Col 
               && v.Row >= Min.Row 
               && v.Row <= Max.Row;
    }

    public void Print(PosRound curr)
    {
        var margin = 1;
        for (var row = Min.Row - margin; row <= Max.Row + margin; row++)
        {
            var line = $"{row}\t";
            for (var col = Min.Col - margin; col <= Max.Col + margin; col++)
            {
                var pos = new Vector(col, row);
                if (pos == curr.Position)
                {
                    line += "P";
                }
                else if (pos == Start || pos == End)
                {
                    line += "~";
                }
                else if (BlizzardsPerRound[curr.MapIndex].TryGetValue(pos, out var c))
                {
                    line += c;
                }
                else if (col == Max.Col + 1 || col == Min.Col - 1 || row == Max.Row + 1 || row == Min.Row - 1)
                {
                    line += "#";
                }
                else
                {
                    line += ".";
                }
            }

            _log(line);
        }
    }

    private Dictionary<Vector, char> GenerateBlizzardPositionsOnRound(int i)
    {
        var blizzardsOnRound = new Dictionary<Vector, char>();
        foreach (var (startPos, (dir, c)) in DirByBlizzardStartPos)
        {
            var posOnRound = GenerateBlizzardPosOnRound(startPos, dir, i);
            if (blizzardsOnRound.TryGetValue(posOnRound, out var curC))
            {
                blizzardsOnRound[posOnRound] = char.IsNumber(curC) ? (int.Parse(curC.ToString()) + 1).ToString()[0] : '2';
            }
            else
            {
                blizzardsOnRound[posOnRound] = c;
            }
        }

        return blizzardsOnRound;
    }

    private Vector GenerateBlizzardPosOnRound(Vector originalPos, Vector dir, int round)
    {
        var newPos = (dir * round) + originalPos; // [0,1] * 2 + [3,3] => [0,2] + [3,3] => [3,5]
        var newCol = Util.Mod((int)(newPos.Col - Min.Col), Width) + Min.Col; // (3-1) % 5 + 1 => 3
        var newRow = Util.Mod((int)(newPos.Row - Min.Row), Height) + Min.Row; // (5-1) % 10 + 1 => 5
        return new Vector(newCol, newRow);
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

    public static Vector operator *(Vector vec, int multiple) =>
        new(vec.Col * multiple, vec.Row * multiple);
    
    public static Vector operator +(Vector vec, Vector toAdd) => vec.Move(toAdd);

    public static readonly Vector DirEast = new(1, 0);
    public static readonly Vector DirSouth = new(0, 1);
    public static readonly Vector DirWest = new(-1, 0);
    public static readonly Vector DirNorth = new(0, -1);

    public (Vector Min, Vector Max) GetMinMax(IEnumerable<Vector> vectors)
    {
        var vectorArray = vectors.ToArray();
        var minCol = vectorArray.Min(e => e.Col);
        var maxCol = vectorArray.Max(e => e.Col);
        var minRow = vectorArray.Min(e => e.Row);
        var maxRow = vectorArray.Max(e => e.Row);
        var min = new Vector(minCol, minRow);
        var max = new Vector(maxCol, maxRow);
        return (min, max);
    }

    public IEnumerable<Vector> Neighbours
    {
        get
        {
            yield return East;
            yield return South;
            yield return West;
            yield return North;
        }
    }

    public override string ToString() => $"Row: {Row}, Col: {Col}";
}