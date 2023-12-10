namespace AdventOfCode2023.Core.Day10;

public record Day10Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var map = PipeMap.FromInput(Input.ToArray());
        yield return map.FarthestFromStart().ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return 0.ToString();
    }
}

public record PipeMap(IReadOnlyDictionary<Point, PipeSection> Pipes, Point Start)
{
    public int FarthestFromStart()
    {
        var distances = new Dictionary<Point, int>
        {
            { Start, 0 }
        };
        var startNeighbours = new[] { Start.North, Start.East, Start.South, Start.West };
        var connectedNeighbours = startNeighbours.Where(p => Pipes.ContainsKey(p) && Pipes[p].Connections.Contains(Start))
                .ToList();

        var points = connectedNeighbours.ToList();
        var c = 0;
        while (points.Count > 0)
        {
            c++;
            var newPoints = new List<Point>();
            foreach (var point in points)
            {
                if (distances.ContainsKey(point)) continue;

                distances[point] = c;
                newPoints.AddRange(Pipes[point].Connections);
            }

            points = newPoints;
        }

        return distances.Values.Max();
    }
    
    public static PipeMap FromInput(IList<string> lines)
    {
        var pipes = new Dictionary<Point, PipeSection>();
        Point? start = null;
        for (var row = 0; row < lines.Count; row++)
        {
            for (var col = 0; col < lines[row].Length; col++)
            {
                var c = lines[row][col];
                if (c != '.')
                {
                    var pipe = new PipeSection(new Point(row, col), c);
                    pipes[pipe.Position] = pipe;
                    if (c == 'S') start = pipe.Position;
                }
            }
        }

        return new PipeMap(pipes, start!);
    }
}

public record PipeSection(Point Position, char Type)
{
    public override string ToString() => $"{Position}: {Type}";

    public IReadOnlyList<Point> Connections { get; } = Type switch
    {
        '|' => new[] { Position.North, Position.South },
        '-' => new[] { Position.West, Position.East },
        'L' => new[] { Position.North, Position.East },
        'J' => new[] { Position.North, Position.West },
        '7' => new[] { Position.South, Position.West },
        'F' => new[] { Position.South, Position.East },
        'S' => Array.Empty<Point>(),
        _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, null)
    };
}

public record Point(int Row, int Col)
{
    public override string ToString() => $"[{Row}, {Col}]";

    public Point South => this with { Row = Row + 1 };
    public Point North => this with { Row = Row - 1 };
    public Point West => this with { Col = Col - 1 };
    public Point East => this with { Col = Col + 1 };
}