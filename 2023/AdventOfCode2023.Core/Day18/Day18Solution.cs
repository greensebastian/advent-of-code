using System.Globalization;
using System.Text;

namespace AdventOfCode2023.Core.Day18;

public record Day18Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var site = DigSite.FromInstruction(Input);
        yield return site.GetL1TrenchSize().ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var site = DigSite.FromInstruction(Input, true);
        yield return site.GetL1TrenchSize().ToString();
    }
}

public record DigSite(Instruction[] Instructions)
{
    public long GetL1TrenchSize()
    {
        var zero = new Point(0, 0);
        var digLine = new List<Point> { zero };
        var turnPoints = new List<Point> { zero };
        foreach (var instruction in Instructions)
        {
            var step = instruction.Dir switch
            {
                Direction.Up => zero.North,
                Direction.Down => zero.South,
                Direction.Left => zero.West,
                Direction.Right => zero.East,
                _ => throw new ArgumentOutOfRangeException()
            };
            for (var i = 0; i < instruction.Len; i++)
            {
                digLine.Add(digLine[^1].Add(step));
            }
            turnPoints.Add(digLine[^1]);
        }
        return CoveredSquares(digLine[..^1], turnPoints);
    }

    private long CoveredSquares(IList<Point> path, IList<Point> turnPoints)
    {
        var rows = turnPoints.Select(p => p.Row).Order().Distinct().ToList();
        var cols = turnPoints.Select(p => p.Col).Order().Distinct().ToList();
        var c = 0L;
        var points = path.ToHashSet();

        var prevRow = 0L;
        for (var rowInd = 0; rowInd < rows.Count; rowInd++)
        {
            var row = rows[rowInd];
            var inPrevRow = CoveredInRow(row - 1);
            var inRow = CoveredInRow(row);

            var rowsCovered = row - prevRow;
            var delta = rowsCovered * inPrevRow + inRow;
            c += delta;
            prevRow = row + 1;
        }

        return c;

        long CoveredInRow(long row)
        {
            var covered = 0L;
            var seen = "";
            var walls = 0;
            for (var colInd = 0; colInd < cols.Count; colInd++)
            {
                var col = cols[colInd];

                if (walls % 2 == 1 && colInd > 0)
                {
                    covered += cols[colInd] - cols[colInd - 1] - 1;
                }
                
                var curr = new Point(row, col);
                var onPath = points.Contains(curr);
                if (onPath)
                {
                    var timesOnPath = path.Select((point, i) => new { p = point, ind = i }).Where(p => p.p == curr).ToArray();
                    foreach (var time in timesOnPath)
                    {
                        var posInPath = time.ind;
                        var prevI = posInPath == 0 ? path.Count - 1 : posInPath - 1;
                        var prev = path[prevI];
                        var toCurrFromPrev = curr.Sub(prev).GetDir();
                        var nextI = (posInPath + 1) % path.Count;
                        var next = path[nextI];
                        var toCurrFromNext = curr.Sub(next).GetDir();
                        if (toCurrFromPrev == Direction.Up || toCurrFromNext == Direction.Up)
                        {
                            if (toCurrFromNext == toCurrFromPrev) continue;
                            walls++;
                        }

                        if (toCurrFromNext == toCurrFromPrev) walls++;
                    }
                }

                if (onPath)
                {
                    covered++;
                }
            }

            return covered;
        }
    }

    private static void Print(List<List<bool>> items)
    {
        var sb = new StringBuilder();
        foreach (var row in items)
        {
            foreach (var item in row)
            {
                sb.Append(item ? "#" : ".");
            }

            sb.AppendLine();
        }
        Console.WriteLine(sb.ToString());
    }
    
    public static DigSite FromInstruction(IEnumerable<string> lines, bool useHex = false)
    {
        return new DigSite(lines.Select(l => useHex ? Instruction.FromHexInput(l) : Instruction.FromInput(l)).ToArray());
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public record Instruction(Direction Dir, int Len)
{
    public static Instruction FromInput(string line)
    {
        var dir = line[0] switch
        {
            'U' => Direction.Up,
            'R' => Direction.Right,
            'D' => Direction.Down,
            'L' => Direction.Left,
            _ => throw new ArgumentOutOfRangeException()
        };
        var len = line.Ints().First();
        return new Instruction(dir, len);
    }
    
    public static Instruction FromHexInput(string line)
    {
        var rgb = line[^7..^1];
        var dir = rgb[^1] switch
        {
            '3' => Direction.Up,
            '0' => Direction.Right,
            '1' => Direction.Down,
            '2' => Direction.Left,
            _ => throw new ArgumentOutOfRangeException()
        };
        var len = int.Parse(rgb[..^1], NumberStyles.HexNumber);
        return new Instruction(dir, len);
    }
}


public record struct Point(long Row, long Col)
{
    public override string ToString() => $"{Row}, {Col}";

    public Point South => this with { Row = Row + 1 };
    public Point North => this with { Row = Row - 1 };
    public Point West => this with { Col = Col - 1 };
    public Point East => this with { Col = Col + 1 };
    public Point Add(Point other) => new(Row + other.Row, Col + other.Col);
    public Point Sub(Point other) => new(Row - other.Row, Col - other.Col);

    public Direction GetDir() => (Row, Col) switch
    {
        (>0, 0) => Direction.Down,
        (<0, 0) => Direction.Up,
        (0, >0) => Direction.Right,
        (0, <0) => Direction.Left,
        _ => throw new ArgumentOutOfRangeException()
    };
}