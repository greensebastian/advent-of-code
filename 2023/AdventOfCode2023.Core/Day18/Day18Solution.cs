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
        yield return "0";
    }
}

public record DigSite(Instruction[] Instructions)
{
    public int GetL1TrenchSize()
    {
        var zero = new Point(0, 0);
        var digLine = new List<Point> { zero };
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
        }
        return CoveredSquares(digLine[..^1]);
    }

    private int CoveredSquares(IList<Point> path)
    {
        var min = new Point(path.Min(p => p.Row), path.Min(p => p.Col));
        var max = new Point(path.Max(p => p.Row), path.Max(p => p.Col));
        var c = 0;

        var output = new List<List<bool>>();
        
        for (var row = min.Row; row <= max.Row; row++)
        {
            var walls = 0;
            var l = new List<bool>();
            output.Add(l);
            for (var col = min.Col; col <= max.Col; col++)
            {
                var curr = new Point(row, col);
                var posInPath = path.IndexOf(curr);
                if (posInPath >= 0)
                {
                    var prevI = posInPath == 0 ? path.Count - 1 : posInPath - 1;
                    var prev = path[prevI];
                    var dirToCurr = curr.Sub(prev).GetDir();
                    var nextI = (posInPath + 1) % path.Count;
                    var next = path[nextI];
                    var dirFromCurr = curr.Sub(next).GetDir();
                    if (dirToCurr == Direction.Up || dirFromCurr == Direction.Up) walls++;
                }

                if (walls % 2 == 1 || posInPath >= 0)
                {
                    l.Add(true);
                    c++;
                }
                else
                {
                    l.Add(false);
                }
            }
        }
        
        Print(output);

        return c;
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
    
    public static DigSite FromInstruction(IEnumerable<string> lines)
    {
        return new DigSite(lines.Select(Instruction.FromInput).ToArray());
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public record Instruction(Direction Dir, int Len, string Rgb)
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
        var rgb = line[^8..^1];
        return new Instruction(dir, len, rgb);
    }
}


public record Point(int Row, int Col)
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
        (1, 0) => Direction.Down,
        (-1, 0) => Direction.Up,
        (0, 1) => Direction.Right,
        (0, -1) => Direction.Left,
        _ => throw new ArgumentOutOfRangeException()
    };
}