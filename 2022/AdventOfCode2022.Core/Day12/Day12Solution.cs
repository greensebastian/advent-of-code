using System.Globalization;

namespace AdventOfCode2022.Core.Day12;

public record Day12Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var lines = Input.ToList();
        var start = GetPoints(lines, c => c == 'S').Single();

        yield return new DijkstraMountain(start, lines).GetShortestPath().ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var lines = Input.ToList();
        var starts = GetPoints(lines, c => c is 'S' or 'a');

        var shortest = int.MaxValue;

        foreach (var start in starts)
        {
            var min = new DijkstraMountain(start, lines).GetShortestPath();
            if (min < shortest)
                shortest = min;
        }
        yield return shortest.ToString();
    }

    private IEnumerable<Point> GetPoints(IList<string> input, Func<char, bool> predicate)
    {
        for (var row = 0; row < input.Count; row++)
        {
            for (var col = 0; col < input[row].Length; col++)
            {
                if (predicate(input[row][col]))
                    yield return new Point(col, row);
            }
        }
    }
}

public record DijkstraMountain(Point Start, List<string> InputLines)
{
    public int GetShortestPath()
    {
        var unvisited = new Dictionary<Point, Node>();
        for (var row = 0; row < InputLines.Count; row++)
        {
            for (var col = 0; col < InputLines[row].Length; col++)
            {
                var point = new Point(col, row);
                unvisited.Add(point, new Node(point, InputLines[row][col], point == Start));
            }
        }
        
        var visited = new Dictionary<Point, Node>();
        var end = unvisited.Single(p => p.Value.IsEnd);

        var current = unvisited[Start];
        while (current is not null && unvisited.ContainsKey(end.Key) && unvisited.Values.Any(node => node.Cost != int.MaxValue))
        {
            foreach (var unvisitedNeighbour in current.ReachableIn(unvisited))
            {
                if (unvisitedNeighbour.Cost > current.Cost + 1)
                    unvisitedNeighbour.Previous = current;
            }

            unvisited.Remove(current.Coordinates);
            visited.Add(current.Coordinates, current);
            current = unvisited.Values.OrderBy(node => node.Cost).FirstOrDefault();
        }

        return visited.TryGetValue(end.Key, out var visitedEnd) ? visitedEnd.Cost : int.MaxValue;
    }
}

public record Node(Point Coordinates, char HeightSymbol, bool IsStart)
{
    public int Height { get; } = CharToHeight(HeightSymbol);
    public bool IsEnd { get; } = HeightSymbol == 'E';
    
    // This should be cached instead, computing takes a lot of time now
    public int Cost => IsStart ? 0 : Previous is null ? int.MaxValue : Previous.Cost + 1;
    public Node? Previous { get; set; }
    public IEnumerable<Node> ReachableIn(Dictionary<Point, Node> pool)
    {
        foreach (var neighbouringPoint in new [] { Coordinates.North, Coordinates.East, Coordinates.South, Coordinates.West })
        {
            if (pool.TryGetValue(neighbouringPoint, out var neighbour))
            {
                if (Height - neighbour.Height >= -1)
                {
                    yield return neighbour;
                }
            }
        }
    }

    private static int CharToHeight(char input) => input switch
    {
        'S' => 0,
        'E' => 'z' - 'a',
        _ => input - 'a'
    };
}

public record Point(int X, int Y)
{
    public Point South => new Point(X, Y + 1);
    public Point North => new Point(X, Y - 1);
    public Point West => new Point(X - 1, Y);
    public Point East => new Point(X + 1, Y);
}