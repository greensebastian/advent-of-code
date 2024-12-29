using System.Text;
using System.Text.RegularExpressions;

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

    private static readonly Regex _longsMatch = new Regex("[+-]{0,1}\\d+");
    public static long[] PlusMinusLongs(this string input)
    {
        return _longsMatch.Matches(input)
            .Select(match => long.Parse(match.Value))
            .ToArray();
    }
    
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
    
    public static long GreatestCommonFactor(long a, long b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
    
    public static long LowestCommonMultiple(long a, long b)
    {
        return a / GreatestCommonFactor(a, b) * b;
    }

    public static long LowestCommonMultiple(params long[] numbers)
    {
        var lcm = LowestCommonMultiple(numbers[0], numbers[1]);
        for (var i = 2; i < numbers.Length; i++)
        {
            lcm = LowestCommonMultiple(lcm, numbers[i]);
        }

        return lcm;
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

    public static int? BinarySearch<TOption>(this IReadOnlyList<TOption> options,
        Func<TOption, int, int> comparer)
    {
        return options.BinarySearchRecursive(comparer, ..options.Count);
    }

    private static int? BinarySearchRecursive<TOption>(this IReadOnlyList<TOption> options, Func<TOption, int, int> comparer, Range toSearch)
    {
        if (toSearch.GetOffsetAndLength(options.Count).Length == 0)
            return null;
        
        var mid = toSearch.Start.Value + (toSearch.End.Value - 1 - toSearch.Start.Value) / 2;
        return comparer(options[mid], mid) switch
        {
            < 0 => options.BinarySearchRecursive(comparer, toSearch.Start.Value..mid),
            > 0 => options.BinarySearchRecursive(comparer, (mid+1)..toSearch.End.Value),
            _ => mid
        };
    }

    public static IEnumerable<(Node<T> EndNode, long Dist)> Dijkstra<T>(this Node<T> root, long rootDist, Func<Node<T>, IEnumerable<T>> neighbourSelector,
        Func<Node<T>, long> distDelta, Func<Node<T>, bool> solvedPredicate, Func<Node<T>, long, bool> failedPredicate)
        where T : notnull
    {
        var best = new Dictionary<T, long>();
        var queue = new PriorityQueue<Node<T>, long>();
        queue.Enqueue(root, rootDist);
        while (queue.TryDequeue(out var current, out var dist))
        {
            if (current != root && best.TryGetValue(current.Value, out var value) && value <= dist) continue;
            best[current.Value] = dist;
            if (current != root && solvedPredicate(current))
            {
                yield return (current, dist);
            }
            if (current != root && failedPredicate(current, dist)) continue;
            foreach (var neighbour in neighbourSelector(current))
            {
                var newNode = current.ConcatWith(neighbour);
                var newDist = dist + distDelta(newNode);
                queue.Enqueue(newNode, newDist);
            }
        }
    }
    
    public static IEnumerable<Node<T>> DepthFirstSearch<T>(this Node<T> node, Func<Node<T>, IEnumerable<T>> neighbourSelector, Func<Node<T>, bool> solvedPredicate, Func<Node<T>, bool> failedPredicate, Dictionary<T, Node<T>[]>? cache = null)
        where T : notnull
    {
        cache ??= new Dictionary<T, Node<T>[]>();
        var cached = cache.TryGetValue(node.Value, out var cachedSolutions);
        if (cached || solvedPredicate(node))
        {
            if (solvedPredicate(node))
            {
                foreach (var nodeInChain in node.Enumerate())
                {
                    cache[nodeInChain.Value] = cache.TryGetValue(nodeInChain.Value, out var existing) ? existing.Append(node).ToArray() : [node];
                }
                yield return node;
            }
            else
            {
                foreach (var cachedSolution in cachedSolutions!)
                {
                    var newSolution = cachedSolution.Copy();
                    newSolution.Replace(n => n!.Value.Equals(node.Value), node);
                    foreach (var nodeInChain in node.Enumerate())
                    {
                        cache[nodeInChain.Value] = cache.TryGetValue(nodeInChain.Value, out var existing) ? existing.Append(newSolution).ToArray() : [newSolution];
                    }
                    yield return newSolution;
                }
            }
        }
        
        if (failedPredicate(node)) yield break;
        if (!cached)
        {
            foreach (var neighbour in neighbourSelector(node))
            {
                var newNode = node.ConcatWith(neighbour);
                foreach (var next in newNode.DepthFirstSearch(neighbourSelector, solvedPredicate, failedPredicate, cache))
                {
                    yield return next;
                }
            }
        }
    }

    public static IEnumerable<(IEnumerable<T>, int)> DEPRECATED_Dijkstra<T>(T root, Func<T, T[]> neighbourSelector, Func<T, T, int> distDelta, Func<T, bool> solvedPredicate, Func<T, bool> failPredicate) where T : notnull
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

    public static Node<TValue> ToNodeChain<TValue>(this IEnumerable<TValue> source) where TValue : notnull
    {
        var enumeratedSource = source.ToArray();
        var copy = new Node<TValue>(enumeratedSource[0], null);
        for (var i = 1; i < enumeratedSource.Length; i++)
        {
            copy = copy.ConcatWith(enumeratedSource[i]);
        }

        return copy;
    }
}

public class Node<TValue>(TValue value, Node<TValue>? previous) where TValue : notnull
{
    public TValue Value { get; } = value;
    public Node<TValue>? Previous { get; private set; } = previous;

    public Node<TValue> Copy() => Enumerate().Select(n => n.Value).Reverse().ToNodeChain();
    
    public Node<TValue> ConcatWith(TValue next)
    {
        return new Node<TValue>(next, this);
    }

    public void Replace(Func<Node<TValue>?, bool> toReplace, Node<TValue>? replacement)
    {
        foreach (var node in Enumerate())
        {
            if (!toReplace(node.Previous)) continue;
            
            node.Previous = replacement;
            return;
        }
    }
        
    public IEnumerable<Node<TValue>> Enumerate()
    {
        var m = this;
        while (m != null)
        {
            yield return m;
            m = m.Previous;
        }
    }

    public override string ToString()
    {
        if (Previous != null) return $"{Previous} <= {Value}";
        return Value.ToString() ?? "";
    }
}

public class PointMap<T>(IEnumerable<KeyValuePair<Point, T>> elements) : Dictionary<Point, T>(elements) where T : notnull
{
    public Point Min => new(Keys.Min(k => k.Row), Keys.Min(k => k.Col));
    public Point Max => new(Keys.Max(k => k.Row), Keys.Max(k => k.Col));

    public void SurroundWith(int length, Func<Point, T> filler)
    {
        var newMin = new Point(Min.Row - length, Min.Col - length);
        var newMax = new Point(Max.Row + length, Max.Col + length);
        
        for (var row = newMin.Row; row <= newMax.Row; row++)
        {
            for (var col = newMin.Col; col <= newMax.Col; col++)
            {
                var p = new Point(row, col);
                TryAdd(p, filler(p));
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

public readonly record struct Point(long Row, long Col)
{
    public Point Up => this with { Row = Row - 1 };
    public Point Right => this with { Col = Col + 1 };
    public Point Down => this with { Row = Row + 1 };
    public Point Left => this with { Col = Col - 1 };

    public enum Direction
    {
        Up, Right, Down, Left
    }

    public Direction GetDirection()
    {
        if (this == Origin.Up) return Direction.Up;
        if (this == Origin.Right) return Direction.Right;
        if (this == Origin.Down) return Direction.Down;
        if (this == Origin.Left) return Direction.Left;
        throw new Exception("No matching direction");
    }

    public static Point FromDirection(Direction d) => d switch
    {
        Direction.Up => Origin.Up,
        Direction.Right => Origin.Right,
        Direction.Down => Origin.Down,
        Direction.Left => Origin.Left,
        _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
    };

    public IEnumerable<Point> ClockwiseNeighbours() =>
        [Up, Up.Right, Right, Right.Down, Down, Down.Left, Left, Left.Up];
    
    public IEnumerable<Point> ClockwiseOrthogonalNeighbours() => [Up, Right, Down, Left];

    public static Point operator +(Point a, Point b) => new(Row: a.Row + b.Row, Col: a.Col + b.Col);
    public static Point operator -(Point a, Point b) => new(Row: a.Row - b.Row, Col: a.Col - b.Col);
    public static Point operator *(Point a, double b) => new(Row: (long)Math.Round(a.Row * b), Col: (long)Math.Round(a.Col * b));

    public double Length() => Math.Sqrt(Math.Pow(Row, 2) + Math.Pow(Col, 2));
    
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

    public IEnumerable<Point> ReachableIn(int steps)
    {
        for (var row = Row - steps; row <= Row + steps; row++)
        {
            var rows = Math.Abs(row - Row);
            var cols = steps - rows;
            for (var col = Col - cols; col <= Col + cols; col++)
            {
                if (row == Row && col == Col) continue;
                yield return new Point(row, col);
            }
        }
    }

    public IEnumerable<Point> OrthogonalStepsTo(Point other)
    {
        var curr = this;
        while (curr != other)
        {
            var dist = (other - curr).Length();
            curr = curr.ClockwiseOrthogonalNeighbours().First(n => (other - n).Length() < dist);
            yield return curr;
        }
    }

    public double Incline() => Row * 1.0 / Col;

    public bool RepeatsIn(Point other)
    {
        if (other.Col == 0 || Col == 0 || other.Row == 0 || Row == 0) return false;
        return other.Col % Col == 0 && other.Row % Row == 0 && other.Col / Col == other.Row / Row;
    }

    public (Point pos, long multiple) ClosestRepetitionFrom(Point target)
    {
        var multiple = (long)Math.Round(target.Length() / Length());
        var repeatedPosition = this * multiple;
        return (repeatedPosition, multiple);
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