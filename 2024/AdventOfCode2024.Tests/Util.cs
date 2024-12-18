using System.Text;

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
        return lines.Select(l => l.Trim()).ToArray();
    }

    public static string[] ReadRaw(string lines) => CleanInput(lines.Split("\n"));

    public static IEnumerable<IEnumerable<string>> SplitByDivider(this IEnumerable<string> lines, Func<string, bool> isDivider)
    {
        var current = new List<string>();
        foreach (var line in lines)
        {
            if (isDivider(line))
            {
                yield return current;
                current = new List<string>();
            }
            else
            {
                current.Add(line);
            }
        }

        if (current.Count > 0) yield return current;
    }

    public static PointMap<T> ToPointMap<T>(this IEnumerable<T> source, Func<T, Point> keySelector) where T : notnull => new(source.ToDictionary(keySelector));

    public static IEnumerable<T[]> Combinations<T>(this IEnumerable<T> source, int elementCount = 2)
    {
        var availableElements = source.ToArray();
        return GetCombinations(elementCount, availableElements).Select(c => c.ToArray());
    }
    
    private static IEnumerable<IEnumerable<T>> GetCombinations<T>(int remainingOps, ICollection<T> availableElements)
    {
        foreach (var availableOp in availableElements)
        {
            if (remainingOps == 1) yield return [availableOp];
            else
            {
                foreach (var combination in GetCombinations(remainingOps - 1, availableElements))
                {
                    yield return combination.Prepend(availableOp);
                }
            }
        }
    }

    public static IEnumerable<(IEnumerable<T>, int)> Dijkstra<T>(T root, Func<T, T[]> neighbourSelector, Func<T, T, int> distDelta, Func<T, bool> solvedPredicate, Func<T, bool> failPredicate) where T : notnull
    {
        var prev = new Dictionary<T, T>();
        var best = new Dictionary<T, int>();
        var queue = new PriorityQueue<T, int>();
        queue.Enqueue(root, 0);
        best[root] = 0;
        while (queue.TryDequeue(out var current, out var dist))
        {
            if (solvedPredicate(current))
            {
                yield return (EnumeratePath(current), dist);
            }
            else
            {
                foreach (var neighbour in neighbourSelector(current))
                {
                    if (failPredicate(neighbour)) continue;
                    best.TryAdd(neighbour, int.MaxValue);
                    var newDist = dist + distDelta(current, neighbour);
                    if (newDist < best[neighbour])
                    {
                        prev[neighbour] = current;
                        best[neighbour] = newDist;
                        queue.Enqueue(neighbour, newDist);
                    }
                }
            }
        }

        yield break;

        IEnumerable<T> EnumeratePath(T rootPos)
        {
            var pos = rootPos;
            while (prev.TryGetValue(pos, out var previous))
            {
                yield return pos;
                pos = previous;
            }
        }
    }
}

public class PointMap<T>(IEnumerable<KeyValuePair<Point, T>> elements) : Dictionary<Point, T>(elements) where T : notnull
{
    public Point Min => new(Keys.Min(k => k.Row), Keys.Min(k => k.Col));
    public Point Max => new(Keys.Max(k => k.Row), Keys.Max(k => k.Col));

    public void SurroundWith(int length, T filler)
    {
        var newMin = new Point(Min.Row - length, Min.Col - length);
        var newMax = new Point(Max.Row + length, Max.Col + length);
        
        for (var row = newMin.Row; row <= newMax.Row; row++)
        {
            for (var col = newMin.Col; col <= newMax.Col; col++)
            {
                TryAdd(new Point(row, col), filler);
            }
        }
    }
    
    public override string ToString()
    {
        return ToString(p => TryGetValue(p, out var value) ? value.ToString() ?? " " : " ");
    }
    
    public string ToString(Func<Point, string> serialize)
    {
        var sb = new StringBuilder();
        for (var row = Min.Row; row <= Max.Row; row++)
        {
            for (var col = Min.Col; col <= Max.Col; col++)
            {
                sb.Append(serialize(new Point(row, col)));
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}

public readonly record struct Point(int Row, int Col)
{
    public Point Up => this with { Row = Row - 1 };
    public Point Right => this with { Col = Col + 1 };
    public Point Down => this with { Row = Row + 1 };
    public Point Left => this with { Col = Col - 1 };

    public IEnumerable<Point> ClockwiseNeighbours() =>
        [Up, Up.Right, Right, Right.Down, Down, Down.Left, Left, Left.Up];
    
    public IEnumerable<Point> ClockwiseOrthogonalNeighbours() => [Up, Right, Down, Left];

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

    public Point Transform(int[][] transform) => new(transform[0][0] * Row + transform[0][1] * Col, transform[1][0] * Row + transform[1][1] * Col);

    private static readonly int[][] RightRotationTransform = [[0, 1], [-1, 0]];
    
    public Point RotateClockwise(int quarters)
    {
        var p = this;
        for (var i = 0; i < quarters % 4; i++)
        {
            p = p.Transform(RightRotationTransform);
        }

        return p;
    }

    public static PointMap<T> GetMap<T>(string[] lines, Func<char, T> transform) where T : notnull
    {
        var map = new Dictionary<Point, T>();
        for (var row = 0; row < lines.Length; row++)
        {
            for (var col = 0; col < lines[row].Length; col++)
            {
                map[new Point(row, col)] = transform(lines[row][col]);
            }
        }

        return new PointMap<T>(map);
    }

    public static Point Origin { get; } = new(0, 0);

    public long ReadingOrderSortValue(long rowMultiple = 100000) => Row * rowMultiple + Col;
    
    public IEnumerable<Point> DepthFirstEnumerate(List<Point> unseen, Func<Point, IEnumerable<Point>> selector)
    {
        if (unseen.Remove(this))
        {
            yield return this;
            foreach (var neighbour in selector(this))
            {
                foreach (var point in neighbour.DepthFirstEnumerate(unseen, selector))
                {
                    yield return point;
                }
            }
        }
    }
    
    public override string ToString() => $"[{Row}, {Col}]";
}

public readonly record struct Vector(Point Start, Point End)
{
    public long OrderFromTopLeft() => FirstInReadingOrder().ReadingOrderSortValue();
    public Point FirstInReadingOrder() => new[] { Start, End }.MinBy(p => p.ReadingOrderSortValue());
}