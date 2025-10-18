using System.Text;

namespace AdventOfCode2023.Core.Day23;

public record Day23Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var map = new HikeMap(Input.ToArray());
        var ans = map.LongestHike();
        yield return ans.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var map = new HikeMap(Input.ToArray(), true);
        var ans = map.LongestHike();
        yield return ans.ToString();
    }
}

public class HikeMap(IReadOnlyList<string> lines, bool ignoreSlides = false)
{
    private Vector Start { get; } = new(0, lines[0].IndexOf('.'));
    private Vector End { get; } = new(lines.Count - 1, lines[^1].IndexOf('.'));
    
    public int LongestHike()
    {
        var startPath = new Path(null, Start, 0);

        var ans = LongestPathJunctions();
        return ans;
        
        var queue = new Queue<Path>();
        queue.Enqueue(startPath);
        var solutions = new List<Path>();
        var highestSeen = 0;

        while (queue.TryDequeue(out var path))
        {
            // Write(path);
            if (path.Current == End)
            {
                if (!IsCircular(path)) solutions.Add(path);
                continue;
            }
            if (path.Len % 500 == 0 && IsCircular(path))
            {
                if (path.Len > highestSeen)
                {
                    GC.Collect();
                    highestSeen = path.Len;
                }
                Console.WriteLine(path.Len);
                continue;
            }
            var e = path.East();
            var s = path.South();
            var w = path.West();
            var n = path.North();
            if (Traversable(e)) queue.Enqueue(e);
            if (Traversable(s)) queue.Enqueue(s);
            if (Traversable(w)) queue.Enqueue(w);
            if (Traversable(n)) queue.Enqueue(n);
        }

        return solutions.Max(s => s.Steps().Count() - 1);
    }

    private int LongestPathJunctions()
    {
        var nodes = new List<Vector> { Start, End };
        for (var row = 1; row < lines.Count - 1; row++)
        {
            for (var col = 1; col < lines[row].Length - 1; col++)
            {
                if (lines[row][col] == '#') continue;
                var p = new Path(null, new(row, col), 0);
                var isJunction = new[] { p.North(), p.East(), p.South(), p.West() }.Count(Traversable) > 2;
                if (isJunction) nodes.Add(p.Current);
            }
        }

        var edges = new HashSet<Edge>();
        foreach (var startVector in nodes)
        {
            var queue = new Queue<Path>();
            var startPath = new Path(null, startVector, 0);
            queue.Enqueue(startPath);
            while (queue.TryDequeue(out var p))
            {
                if (!Traversable(p)) continue;
                if (p.Current != startVector && nodes.Contains(p.Current))
                {
                    var startPos = (startVector.Row << 8) + startVector.Col;
                    var endPos = (p.Current.Row << 8) + p.Current.Col;
                    var s = startPos < endPos ? startVector : p.Current;
                    var e = s == startVector ? p.Current : startVector;
                    edges.Add(new(s, e, p.Len));
                }
                else
                {
                    queue.Enqueue(p.East());
                    queue.Enqueue(p.South());
                    queue.Enqueue(p.West());
                    queue.Enqueue(p.North());
                }
            }
        }

        var solutions = new List<Edge[]>();
        var tsQueue = new Queue<Edge[]>();
        foreach (var edge in edges)
        {
            if (edge.Start == Start)
            {
                tsQueue.Enqueue([edge]);
            }
        }
        while (tsQueue.TryDequeue(out var route))
        {
            var edgesInRoute = route.SelectMany(e => new[] { e.Start, e.End }).ToArray();
            var last = edgesInRoute.Single(v => v != Start && edgesInRoute.Count(e => e == v) == 1);
            if (last == End)
            {
                solutions.Add(route);
                continue;
            }

            foreach (var edge in edges)
            {
                if (edge.Start == Start) continue;
                if (route.Contains(edge)) continue;
                if (edge.Start == last && edgesInRoute.All(e => e != edge.End))
                {
                    tsQueue.Enqueue(route.Append(edge).ToArray());
                }
                if (edge.End == last && edgesInRoute.All(e => e != edge.Start))
                {
                    tsQueue.Enqueue(route.Append(edge).ToArray());
                }
            }
        }

        return solutions.Select(s => s.Sum(e => e.Len)).Max();
    }

    private int? LongestPathIterative(Path start)
    {
        var stack = new Stack<Path>();
        stack.Push(start);
        var attempts = 0;
        var solutions = new List<Path>{start};
        while (stack.TryPop(out var p))
        {
            if (p.Current == End)
            {
                solutions.Add(p);
                continue;
            }
            if (!Traversable(p)) continue;
            attempts++;
            if (attempts % 1000 == 0)
            {
                Console.WriteLine(attempts);
                Console.WriteLine(p.Len);
                Console.WriteLine(solutions.Select(s => s.Len).Max());
            }
            if (p.Len % 100 == 0)
            {
                var isCircular = IsCircular(p);
                var topIsCircular = true;
                while (isCircular && topIsCircular)
                {
                    if (!stack.TryPeek(out var top)) break;
                    
                    topIsCircular = IsCircular(top);
                    if (topIsCircular) stack.Pop();
                }
                if (isCircular) continue;
            }
            stack.Push(p.South());
            stack.Push(p.East());
            stack.Push(p.West());
            stack.Push(p.North());
        }

        return solutions.Select(s => s.Len).Max();
    }

    private int? LongestPathRecursive(Path p)
    {
        if (p.Current == End) return p.Len;
        if (!Traversable(p)) return null;
        if (p.Len % 100 == 0 && IsCircular(p)) return null;
        var e = p.East();
        var s = p.South();
        var w = p.West();
        var n = p.North();
        if (p.Len % 500 == 0)
        {
            Console.WriteLine(p.Len);
        }

        var next = new[] { LongestPathRecursive(n), LongestPathRecursive(e), LongestPathRecursive(s), LongestPathRecursive(w) }.OfType<int>().ToArray();
        return next.Any() ? next.Max() : null;
    }

    private void Write(Path p) => Console.WriteLine(Print(p));
    
    private string Print(Path p)
    {
        var steps = p.Steps().Reverse().Select((s, i) => (s, i)).DistinctBy(t => t.s).ToDictionary(t => t.s, t => t.i);
        var sb = new StringBuilder();
        for (var row = 0; row < lines.Count; row++)
        {
            for (var col = 0; col < lines[row].Length; col++)
            {
                sb.Append(steps.TryGetValue(new(row, col), out var step) ? (step % 10).ToString()[0] : lines[row][col]);
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
    
    private bool IsCircular(Path p)
    {
        var seen = new HashSet<long>(p.Len * 2);
        foreach (var vector in p.Steps())
        {
            long val = (vector.Col << 8) + vector.Row;
            if (!seen.Add(val)) return true;
        }

        return false;
    }

    private bool OnMap(Vector v) => v.Col >= 0 && v.Col < lines[0].Length && v.Row >= 0 && v.Row < lines.Count;
    private bool Traversable(Path p)
    {
        if (!OnMap(p.Current)) return false;
        if (lines[p.Current.Row][p.Current.Col] == '#') return false;
        if (p.Prev is null) return true;
        if (p.Prev.Prev?.Current == p.Current) return false;
        var prev = p.Prev.Current;
        if (ignoreSlides) return true;
        var prevChar = lines[prev.Row][prev.Col];
        return prevChar switch
        {
            '.' => true,
            '>' => p.Current.Col > prev.Col,
            'v' => p.Current.Row > prev.Row,
            '<' => p.Current.Col < prev.Col,
            '^' => p.Current.Row < prev.Row,
            _ => throw new Exception()
        };
    }
}

public record struct Vector(int Row, int Col);

public record struct Edge(Vector Start, Vector End, int Len);

public record Path(Path? Prev, Vector Current, int Len)
{
    public IEnumerable<Vector> Steps()
    {
        yield return Current;
        if (Prev == null) yield break;
        foreach (var prevStep in Prev.Steps())
        {
            yield return prevStep;
        }
    }

    public Path North() => new(this, Current with { Row = Current.Row - 1 }, Len + 1);
    public Path East() => new(this, Current with { Col = Current.Col + 1 }, Len + 1);
    public Path South() => new(this, Current with { Row = Current.Row + 1 }, Len + 1);
    public Path West() => new(this, Current with { Col = Current.Col - 1 }, Len + 1);
}