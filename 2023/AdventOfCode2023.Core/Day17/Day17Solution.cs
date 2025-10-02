using System.Text;

namespace AdventOfCode2023.Core.Day17;

public record Day17Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public const string ExamplePath = "2411545323135424535653733333";
    
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var map = Map.FromInput(Input.ToArray(), Log);
        yield return map.LeastHeatLossPath().ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var map = Map.FromInput(Input.ToArray(), Log);
        yield return map.LeastHeatLossPathUltra().ToString();
    }
}

public record Map(IReadOnlyDictionary<Point, int> HeatCost, Action<string> Log)
{
    public Point Min { get; } = new(HeatCost.Keys.Min(p => p.Row), HeatCost.Keys.Min(p => p.Col));
    public Point Max { get; } = new(HeatCost.Keys.Max(p => p.Row), HeatCost.Keys.Max(p => p.Col));
    
    public int LeastHeatLossPath()
    {
        var start = new Path(new[] { Min }, this);
        var queue = new PriorityQueue<Path, int>();
        var cache = new Dictionary<State, Path[]>();
        queue.Enqueue(start, start.Cost);
        while (true)
        {
            var curr = queue.Dequeue();
            if (curr.Done) return curr.Cost;
            var state = curr.State();
            if (cache.TryGetValue(state, out var cached))
            {
                if (cached.Any(p => p.InRow() <= curr.InRow())) continue;
                cache[state] = cache[state].Append(curr).ToArray();
            }
            else
            {
                cache[state] = new[] { curr };
            }

            var nextOnes = curr.NextPaths().ToArray();
            foreach (var next in nextOnes)
            {
                queue.Enqueue(next, next.Cost);
            }
        }
    }
    
    public int LeastHeatLossPathUltra()
    {
        var start = new Path(new[] { Min }, this);
        var queue = new PriorityQueue<Path, int>();
        var cache = new Dictionary<InRowState, Path>();
        queue.Enqueue(start, start.Cost);
        while (true)
        {
            var curr = queue.Dequeue();
            if (curr.Done) return curr.Cost;
            
            var state = curr.InRowState();
            if (cache.ContainsKey(state)) continue;
            cache[state] = curr;

            var nextOnes = curr.NextPathsUltra().ToArray();
            foreach (var next in nextOnes)
            {
                queue.Enqueue(next, next.Cost);
            }
        }
    }
    
    public bool InGrid(Point p)
    {
        return Min.Col <= p.Col && p.Col <= Max.Col && Min.Row <= p.Row && p.Row <= Max.Row;
    }
    
    public static Map FromInput(IList<string> lines, Action<string> log)
    {
        var cost = new Dictionary<Point, int>();
        for (var row = 0; row < lines.Count; row++)
        {
            for (var col = 0; col < lines[row].Length; col++)
            {
                cost[new Point(row, col)] = int.Parse(lines[row][col].ToString());
            }
        }

        return new Map(cost, log);
    }
}

public record State(Point Direction, Point Position);

public record InRowState(Point Direction, Point Position, int InRow);

public record Path(Point[] Visited, Map Map)
{
    public string ShortPath() => string.Join("", Visited.Select(p => Map.HeatCost[p]));
    
    public override string ToString() => $"{Cost} {string.Join(' ', Visited.Select(p => $"({Map.HeatCost[p]})[{p.Row},{p.Col}]"))}";

    public string Print()
    {
        var sb = new StringBuilder();
        for (var row = Map.Min.Row; row <= Map.Max.Row; row++)
        {
            for (var col = Map.Min.Col; col <= Map.Max.Col; col++)
            {
                var c = new Point(row, col);
                var p = Visited.FirstOrDefault(p => p == c);
                sb.Append(p is null ? Map.HeatCost[c].ToString() : "*");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
    
    public int Cost { get; } = Visited.Skip(1).Sum(p => Map.HeatCost[p]);
    public bool Done { get; } = Visited[^1] == Map.Max;
    private Point Last { get; } = Visited[^1];
    private bool LastFourOnColumn { get; } = Visited.AsEnumerable().Reverse().Take(4).Count(p => p.Col == Visited[^1].Col) == 4;
    private bool LastFourOnRow { get; } = Visited.AsEnumerable().Reverse().Take(4).Count(p => p.Row == Visited[^1].Row) == 4;

    public State State()
    {
        var dir = Visited.Length > 1 ? Visited[^1].Sub(Visited[^2]) : new Point(0, 0).East;
        return new State(dir, Last);
    }
    
    public InRowState InRowState()
    {
        var dir = Visited.Length > 1 ? Visited[^1].Sub(Visited[^2]) : new Point(0, 0).East;
        return new InRowState(dir, Last, InRow());
    }

    public int InRow() => Visited.AsEnumerable().Reverse().TakeWhile(p => p.Row == Last.Row || p.Col == Last.Col).Count();

    public IEnumerable<Path> NextPaths() => Next().Select(p => new Path(Visited.Append(p).ToArray(), Map));
    
    public IEnumerable<Path> NextPathsUltra() => NextUltra()
        .Where(p => Map.InGrid(p))
        .OrderBy(p => Map.HeatCost[p])
        .Select(p =>
        {
            var inLine = Last.PathTo(p);
            return new Path(Visited.Concat(inLine).ToArray(), Map);
        });
    
    private IEnumerable<Point> Next() => new[] { Last.North, Last.East, Last.South, Last.West }
        .Where(p => Visited.Length < 2 || p != Visited[^2])
        .Where(p => Map.InGrid(p))
        .Where(p => !(p.Row == Last.Row && LastFourOnRow))
        .Where(p => !(p.Col == Last.Col && LastFourOnColumn))
        .OrderBy(p => Map.HeatCost[p]);
    
    private IEnumerable<Point> NextUltra(){
        if (Visited.Length == 1)
        {
            yield return new Point(0, 4);
            yield return new Point(4, 0);
            yield break;
        }

        var dir = Last.Sub(Visited[^2]);
        if (InRow() < 11)
        {
            yield return Last.Add(dir);
        }

        if (dir.Col == 0)
        {
            yield return Last with { Col = Last.Col - 4 };
            yield return Last with { Col = Last.Col + 4 };
        }
        
        if (dir.Row == 0)
        {
            yield return Last with { Row = Last.Row - 4 };
            yield return Last with { Row = Last.Row + 4 };
        }
    }
}

public record Point(int Row, int Col)
{
    public override string ToString() => $"[{Row}, {Col}]";

    public Point South => this with { Row = Row + 1 };
    public Point North => this with { Row = Row - 1 };
    public Point West => this with { Col = Col - 1 };
    public Point East => this with { Col = Col + 1 };
    public Point Add(Point other) => new(Row + other.Row, Col + other.Col);
    public Point Sub(Point other) => new(Row - other.Row, Col - other.Col);

    public IEnumerable<Point> PathTo(Point other)
    {
        var step = other.Sub(this).Norm();
        var c = this;
        while (c != other)
        {
            c = c.Add(step);
            yield return c;
        }
    }

    public Point Norm()
    {
        return new Point(Math.Clamp(Row, -1, 1), Math.Clamp(Col, -1, 1));
    }
}