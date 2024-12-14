namespace AdventOfCode2024.Tests;

public static class Util
{
    public static string[] ReadFile(string name)
    {
        var filename = Directory.GetFiles(Environment.CurrentDirectory, $"{name}.input.txt", SearchOption.AllDirectories).Single();
        var lines = File.ReadAllLines(filename);
        return CleanInput(lines);
    }

    public static string[] CleanInput(string[] lines)
    {
        if (string.IsNullOrWhiteSpace(lines[0])) lines = lines[1..];
        if (string.IsNullOrWhiteSpace(lines[^1])) lines = lines[..^1];
        return lines;
    }

    public static string[] ReadRaw(string lines) => CleanInput(lines.Split("\n"));
}

public readonly record struct Point(int Row, int Col)
{
    public Point Up => this with { Row = Row - 1 };
    public Point Right => this with { Col = Col + 1 };
    public Point Down => this with { Row = Row + 1 };
    public Point Left => this with { Col = Col - 1 };

    public IEnumerable<Point> ClockwiseNeighboursWithDiagonal =>
        [Up, Up.Right, Right, Right.Down, Down, Down.Left, Left, Left.Up];

    public static Point operator +(Point a, Point b) => new(Row: a.Row + b.Row, Col: a.Col + b.Col);
    public static Point operator -(Point a, Point b) => new(Row: a.Row - b.Row, Col: a.Col - b.Col);
    
    public Point DirTo(Point other)
    {
        var diff = other - this;
        if (diff.Row == 0) return new Point(0, Math.Clamp(diff.Col, -1, 1));
        if (diff.Col == 0) return new Point(Math.Clamp(diff.Row, -1, 1), 0);
        if (Math.Abs(diff.Row) != Math.Abs(diff.Col)) throw new ArgumentException($"Difference not orthogonal or diagonal, {diff}", nameof(other));
        return (diff.Col > 0, diff.Row > 0) switch
        {
            (true, true) => new Point(1, 1),
            (true, false) => new Point(1, -1),
            (false, true) => new Point(-1, 1),
            (false, false) => new Point(-1, -1)
        };
    }

    public override string ToString() => $"[{Row}, {Col}]";
}