namespace AdventOfCode2025.Tests;

public static class Util
{
    public static string[] ReadFile(string name, bool trim = true)
    {
        var filename = Directory.GetFiles(Environment.CurrentDirectory, $"{name}.input.txt", SearchOption.AllDirectories).Single();
        var lines = File.ReadAllLines(filename);
        return CleanInput(lines, trim);
    }

    private static string[] CleanInput(string[] lines, bool trim)
    {
        if (string.IsNullOrWhiteSpace(lines[0])) lines = lines[1..];
        if (string.IsNullOrWhiteSpace(lines[^1])) lines = lines[..^1];
        return lines.Select(l => trim ? l.Trim() : l).ToArray();
    }
    
    public static string[] ReadRaw(string lines, bool trim = true) => CleanInput(lines.Split("\n"), trim);
}

public record Point(long Row, long Col)
{
    public Point North() => this with { Row = Row - 1 };
    public Point East() => this with { Col = Col + 1 };
    public Point South() => this with { Row = Row + 1 };
    public Point West() => this with { Col = Col - 1 };

    public IEnumerable<Point> ClockwiseNeighbours() =>
    [
        North(), North().East(), East(), East().South(), South(), South().West(), West(), West().North()
    ];
}

public class PointMap<TValue>(IReadOnlyList<KeyValuePair<Point, TValue>> points)
{
    public Dictionary<Point, TValue> Points { get; } = points.ToDictionary(p => p.Key, p => p.Value);
    public TValue? Get(Point p) => Points.GetValueOrDefault(p);
    public TValue? Get(long row, long col) => Get(new Point(row, col));
    public Point Min { get; } = new(points.Min(p => p.Key.Row), points.Min(p => p.Key.Col));
    public Point Max { get; } = new(points.Max(p => p.Key.Row), points.Max(p => p.Key.Col));
}