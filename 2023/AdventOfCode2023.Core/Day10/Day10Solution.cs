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
        var map = PipeMap.FromInput(Input.ToArray());
        yield return map.CountWithinLoop().ToString();
    }
}

public record PipeMap(IReadOnlyDictionary<Point, PipeSection> Pipes, Point Start)
{
    public int FarthestFromStart()
    {
        var distances = GetLoopPipesAndDistances();

        return distances.Values.Max();
    }

    private char GetStartType()
    {
        var conn = GetConnectedToStart();
        var type = (conn.Contains(Start.North), conn.Contains(Start.East), conn.Contains(Start.South),
                conn.Contains(Start.West)) switch
            {
                (true, true, _, _) => 'L',
                (true, _, true, _) => '|',
                (true, _, _, true) => 'J',
                (_, true, true, _) => 'F',
                (_, true, _, true) => '-',
                (_, _, true, true) => '7',
                _ => throw new ArgumentOutOfRangeException()
            };
        return type;
    }
    
    public int CountWithinLoop()
    {
        var pipesInLoop = GetLoopPipesAndDistances().Keys.ToHashSet();
        var topLeft = new Point(pipesInLoop.Select(p => p.Row).Min() - 2, pipesInLoop.Select(p => p.Col).Min() - 2);
        var bottomRight = new Point(pipesInLoop.Select(p => p.Row).Max() + 2, pipesInLoop.Select(p => p.Col).Max() + 2);

        var segmentsOnCurrentEdge = "";
        var counted = new List<Point>();
        for (var row = topLeft.Row; row <= bottomRight.Row; row++)
        {
            var startedInLoop = false;
            var inLoop = false;
            for (var col = topLeft.Col; col <= bottomRight.Col; col++)
            {
                var curr = new Point(row, col);

                var movingToEdge = segmentsOnCurrentEdge == "" && pipesInLoop.Contains(curr);
                var movingOffEdge = inLoop && !pipesInLoop.Contains(curr) && pipesInLoop.Contains(curr.West);
                if (segmentsOnCurrentEdge.Length > 0 && pipesInLoop.Contains(curr))
                {
                    var type = curr == Start ? GetStartType() : Pipes[curr].Type;

                    segmentsOnCurrentEdge += type;
                }
                
                if (movingToEdge)
                {
                    startedInLoop = inLoop;
                    inLoop = true;
                    var type = curr == Start ? GetStartType() : Pipes[curr].Type;
                    segmentsOnCurrentEdge = type.ToString();
                }
                if (movingOffEdge)
                {
                    var segments = segmentsOnCurrentEdge.Replace("-", "");
                    segments = segments.Replace("FJ", "|");
                    segments = segments.Replace("F7", "");
                    segments = segments.Replace("LJ", "");
                    segments = segments.Replace("L7", "|");
                    if (startedInLoop)
                    {
                        inLoop = segments.Length % 2 == 0;
                    }
                    else
                    {
                        inLoop = segments.Length % 2 == 1;
                    }
                    segmentsOnCurrentEdge = "";
                }
                
                if (inLoop) counted.Add(curr);
            }
        }

        var enclosed = counted.Where(p => !pipesInLoop.Contains(p)).ToList();
        
        return counted.Count - pipesInLoop.Count;
    }

    private Dictionary<Point, int> GetLoopPipesAndDistances()
    {
        var distances = new Dictionary<Point, int>
        {
            { Start, 0 }
        };
        var connectedNeighbours = GetConnectedToStart();

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

        return distances;
    }

    private List<Point> GetConnectedToStart()
    {
        var startNeighbours = new[] { Start.North, Start.East, Start.South, Start.West };
        var connectedNeighbours = startNeighbours.Where(p => Pipes.ContainsKey(p) && Pipes[p].Connections.Contains(Start))
            .ToList();
        return connectedNeighbours;
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