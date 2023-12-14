namespace AdventOfCode2023.Core.Day14;

public record Day14Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var tilt = new Point(0, 0).North;
        var platform = Platform.FromInput(Input.ToArray());
        platform.Tilt(tilt);
        var score = platform.Load(tilt);
        yield return score.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return "0";
    }
}

public record Platform(IReadOnlySet<Point> Solid, Point[] Moving)
{
    public void Tilt(Point dir)
    {
        for (var i = 0; i < Moving.Length; i++)
        {
            var moving = Moving[i];
            
            var newPos = moving.Add(dir);
            while (!Solid.Contains(newPos))
            {
                newPos = newPos.Add(dir);
            }
            newPos = newPos.Sub(dir);

            var otherMoving = Moving.Where(p => p != moving).ToHashSet();
            while (otherMoving.Contains(newPos))
            {
                newPos = newPos.Sub(dir);
            }

            Moving[i] = newPos;
        }
    }

    public int Load(Point dir)
    {
        if (dir == new Point(0, 0).North)
        {
            var max = Solid.Select(p => p.Row).Max();
            return Moving.Select(p => max - p.Row).Sum();
        }

        return 0;
    }
    
    public static Platform FromInput(IList<string> lines)
    {
        var solid = new List<Point>();
        var moving = new List<Point>();
        
        // Boundary
        for (var row = -1; row <= lines.Count; row++)
        {
            solid.Add(new Point(row, -1));
            solid.Add(new Point(row, lines[0].Length));
        }
        for (var col = -1; col <= lines[0].Length; col++)
        {
            solid.Add(new Point(-1, col));
            solid.Add(new Point(lines.Count, col));
        }
        
        // Parse
        for (var row = 0; row < lines.Count; row++)
        {
            for (var col = 0; col < lines[0].Length; col++)
            {
                if (lines[row][col] == '#') solid.Add(new Point(row, col));
                if (lines[row][col] == 'O') moving.Add(new Point(row, col));
            }
        }

        return new Platform(solid.ToHashSet(), moving.Distinct().ToArray());
    }
}

public record Point(int Row, int Col)
{
    public Point South => this with { Row = Row + 1 };
    public Point North => this with { Row = Row - 1 };
    public Point West => this with { Col = Col - 1 };
    public Point East => this with { Col = Col + 1 };
    public Point Add(Point other) => new(Row + other.Row, Col + other.Col);
    public Point Sub(Point other) => new(Row - other.Row, Col - other.Col);
}