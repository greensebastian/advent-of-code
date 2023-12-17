using System.Text;

namespace AdventOfCode2023.Core.Day16;

public record Day16Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var map = LaserMap.FromInput(Input.ToArray());
        yield return map.EnergizedPositions().ToString();
    }

    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var map = LaserMap.FromInput(Input.ToArray());
        yield return map.HighestEnergizedPositionsCount().ToString();
    }
}

public record LaserMap(char[][] Mirrors)
{
    public string ToString(IReadOnlySet<Laser> seen)
    {
        var sb = new StringBuilder();
        for (var row = 0; row < Mirrors.Length; row++)
        {
            for (var col = 0; col < Mirrors[row].Length; col++)
            {
                var p = new Point(row, col);
                var seenCount = seen.Count(s => s.Origin == p);
                var c = seenCount == 0 ? Mirrors[row][col] : seenCount > 9 ? 'X' : seenCount.ToString()[0];
                sb.Append(c);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
    
    private Point Min { get; } = new Point(0, 0);
    private Point Max { get; } = new Point(Mirrors.Length - 1, Mirrors[0].Length - 1);

    public int HighestEnergizedPositionsCount()
    {
        var max = 0;
        foreach (var start in StartPositions())
        {
            var count = EnergizedPositions(start);
            max = Math.Max(max, count);
        }

        return max;
    }

    private IEnumerable<Laser> StartPositions()
    {
        for (var col = Min.Col - 1; col <= Max.Col + 1; col++)
        {
            yield return new Laser(new Point(Min.Row - 1, col), Direction.South);
            yield return new Laser(new Point(Max.Row + 1, col), Direction.North);
        }

        for (var row = Min.Row - 1; row < Max.Row + 1; row++)
        {
            yield return new Laser(new Point(row, Min.Col - 1), Direction.East);
            yield return new Laser(new Point(row, Max.Col + 1), Direction.West);
        }
    }
    
    public int EnergizedPositions(Laser? start = null)
    {
        var seen = new HashSet<Laser>();
        var laser = start ?? new Laser(new Point(0, -1), Direction.East);
        Check(laser, seen);
        return seen.Select(l => l.Origin).Distinct().Count();
    }

    private void Check(Laser laser, ISet<Laser> seen)
    {
        if (seen.Contains(laser)) return;
        if (InGrid(laser.Origin))
        {
            seen.Add(laser);
        }
        var nextPos = laser.Dir switch
        {
            Direction.North => laser.Origin.North,
            Direction.East => laser.Origin.East,
            Direction.South => laser.Origin.South,
            Direction.West => laser.Origin.West
        };
        if (!InGrid(nextPos)) return;

        void Go(Direction dir)
        {
            Check(new Laser(nextPos!, dir), seen);
        }
        
        var mirror = Mirrors[nextPos.Row][nextPos.Col];
        switch (laser.Dir, mirror)
        {
            case (Direction.North, '/'):
                Go(Direction.East);
                break;
            case (Direction.North, '\\'):
                Go(Direction.West);
                break;
            case (Direction.North, '-'):
                Go(Direction.West);
                Go(Direction.East);
                break;
            case (Direction.East, '/'):
                Go(Direction.North);
                break;
            case (Direction.East, '\\'):
                Go(Direction.South);
                break;
            case (Direction.East, '|'):
                Go(Direction.North);
                Go(Direction.South);
                break;
            case (Direction.South, '/'):
                Go(Direction.West);
                break;
            case (Direction.South, '\\'):
                Go(Direction.East);
                break;
            case (Direction.South, '-'):
                Go(Direction.West);
                Go(Direction.East);
                break;
            case (Direction.West, '/'):
                Go(Direction.South);
                break;
            case (Direction.West, '\\'):
                Go(Direction.North);
                break;
            case (Direction.West, '|'):
                Go(Direction.North);
                Go(Direction.South);
                break;
            default: 
                Go(laser.Dir);
                break;
        }
    }

    private bool InGrid(Point p)
    {
        return Min.Col <= p.Col && p.Col <= Max.Col && Min.Row <= p.Row && p.Row <= Max.Row;
    }
    
    public static LaserMap FromInput(IList<string> lines)
    {
        var mirrors = new char[lines.Count][];
        for (var row = 0; row < lines.Count; row++)
        {
            mirrors[row] = new char[lines[row].Length];
            for (var col = 0; col < lines[row].Length; col++)
            {
                mirrors[row][col] = lines[row][col];
            }
        }

        return new LaserMap(mirrors);
    }
}

public enum Direction
{
    North,
    East,
    South,
    West
}

public record Laser(Point Origin, Direction Dir);

public record Point(int Row, int Col)
{
    public Point South => this with { Row = Row + 1 };
    public Point North => this with { Row = Row - 1 };
    public Point West => this with { Col = Col - 1 };
    public Point East => this with { Col = Col + 1 };
    public Point Add(Point other) => new(Row + other.Row, Col + other.Col);
    public Point Sub(Point other) => new(Row - other.Row, Col - other.Col);
}