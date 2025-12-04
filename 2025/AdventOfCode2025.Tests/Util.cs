namespace AdventOfCode2025.Tests;

public static class Util
{
    public static string[] ReadFile(string name)
    {
        var filename = Directory.GetFiles(Environment.CurrentDirectory, $"{name}.input.txt", SearchOption.AllDirectories).Single();
        var lines = File.ReadAllLines(filename);
        return CleanInput(lines);
    }

    private static string[] CleanInput(string[] lines)
    {
        if (string.IsNullOrWhiteSpace(lines[0])) lines = lines[1..];
        if (string.IsNullOrWhiteSpace(lines[^1])) lines = lines[..^1];
        return lines.Select(l => l.Trim()).ToArray();
    }
    
    public static string[] ReadRaw(string lines) => CleanInput(lines.Split("\n"));
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