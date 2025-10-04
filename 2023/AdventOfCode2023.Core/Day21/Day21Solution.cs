using System.Text;

namespace AdventOfCode2023.Core.Day21;

public record Day21Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var map = new GardenersMap(Input.ToArray());
        var ans = map.ReachablePointsIn(int.Parse(args[0]));
        yield return ans.ToString();
    }

    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return 0.ToString();
    }
}

public class GardenersMap
{
    public GardenersMap(IReadOnlyList<string> input)
    {
        for (var r = 0; r < input.Count; r++)
        {
            for (var c = 0; c < input[r].Length; c++)
            {
                Map[new Point(r, c)] = input[r][c];
            }
        }
    }

    public string Print(IReadOnlySet<Point> current)
    {
        var width = Map.Keys.Max(p => p.Col);
        var height = Map.Keys.Max(p => p.Row);
        var sb = new StringBuilder();
        for (var row = 0; row <= width; row++)
        {
            for (var col = 0; col <= height; col++)
            {
                var p = new Point(row, col);
                var c = current.Contains(p) ? 'O' : Map[p];
                sb.Append(c);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    private Dictionary<Point, char> Map { get; } = new();

    public int ReachablePointsIn(int steps)
    {
        var start = Map.Single(p => p.Value == 'S').Key;
        var current = new HashSet<Point> { start };
        for (var i = 0; i < steps; i++)
        {
            var nextCurrent = new HashSet<Point>();
            foreach (var currentPosition in current)
            {
                var next = new[]
                        { currentPosition.East, currentPosition.South, currentPosition.West, currentPosition.North }
                    .Where(p => Map.TryGetValue(p, out var nextChar) && nextChar != '#' && !current.Contains(p))
                    .ToArray();
                foreach (var point in next)
                {
                    nextCurrent.Add(point);
                }
            }

            current = nextCurrent;
            //Console.WriteLine($"After {i}");
            //Console.WriteLine(Print(current));
        }

        return current.Count;
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
    public Point Mul(int factor) => new(Row * factor, Col * factor);
}