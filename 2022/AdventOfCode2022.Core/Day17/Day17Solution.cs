using System.ComponentModel;

namespace AdventOfCode2022.Core.Day17;

public record Day17Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        using var game = new VentTetris(Input.Single());

        game.Run(2022);
        
        yield return game.HighestPoint.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        using var game = new VentTetris(Input.Single());

        game.Run(long.Parse(args[0]));
        
        yield return game.HighestPoint.ToString();
    }
}

public class VentTetris : IDisposable
{
    private IEnumerator<char> Input { get; }
    private IEnumerator<Shape> Shapes { get; } = TetrisShape.GenerateShapes().GetEnumerator();
    public long HighestPoint { get; private set; }
    public long LowestPoint { get; private set; }
    private long RocksFallen { get; set; }

    private Dictionary<Vector, Shape> Occupied { get; set; } = new()
    {
        { new Vector(0, 0), Shape.Dash },
        { new Vector(1, 0), Shape.Dash },
        { new Vector(2, 0), Shape.Dash },
        { new Vector(3, 0), Shape.Dash },
        { new Vector(4, 0), Shape.Dash },
        { new Vector(5, 0), Shape.Dash },
        { new Vector(6, 0), Shape.Dash }
    };

    private const int LeftLimit = 0;
    private const int RightLimit = 6;
    private Vector CurrentBottomLeft { get; set; }

    public VentTetris(string input)
    {
        Input = GetRepeatableInput(input).GetEnumerator();
        Shapes.MoveNext();
        SetNewBottomLeft();
    }

    private IEnumerable<char> GetRepeatableInput(string input)
    {
        var c = -1;
        while (true)
        {
            c = (c + 1) % input.Length;
            yield return input[c];
        }
    }

    private void SetNewBottomLeft() => CurrentBottomLeft = new Vector(2, HighestPoint + 4);

    public void Run(long rockLimit)
    {
        var c = 0L;
        while (RocksFallen < rockLimit)
        {
            //Print();
            DoRound();
            if (RocksFallen > c)
            {
                c = RocksFallen;
                
            }
        }
    }

    private void DoRound()
    {
        Input.MoveNext();
        var move = Input.Current;
        var shape = Shapes.Current;

        if (move == '<')
        {
            // Check left wall
            if (CurrentBottomLeft.X != LeftLimit)
            {
                // Check left occupied
                var toLeft = CoveredByActive(CurrentBottomLeft.Left);
                if (!toLeft.Any(v => Occupied.ContainsKey(v)))
                {
                    CurrentBottomLeft = CurrentBottomLeft.Left;
                }
            }
        }
        else
        {
            // Check right wall
            if (CurrentBottomLeft.X + TetrisShape.Width[shape] - 1 != RightLimit)
            {
                // Check left occupied
                var toRight = CoveredByActive(CurrentBottomLeft.Right);
                if (!toRight.Any(v => Occupied.ContainsKey(v)))
                {
                    CurrentBottomLeft = CurrentBottomLeft.Right;
                }
            }
        }

        if (BlockedBelow)
        {
            foreach (var coveredByActive in CoveredByActive(CurrentBottomLeft))
            {
                Occupied[coveredByActive] = shape;
            }

            var highestOfActive = CurrentBottomLeft.Y + TetrisShape.Height[shape] - 1;
            HighestPoint = highestOfActive > HighestPoint ? highestOfActive : HighestPoint;
            Shapes.MoveNext();
            SetNewBottomLeft();
            RocksFallen++;
            
            if (RocksFallen % 100000 == 0)
                DoCutoff();
        }
        else
        {
            CurrentBottomLeft = CurrentBottomLeft.Down;
        }
    }

    private long FindCutOff()
    {
        var y = HighestPoint;
        long[] lastOccupied = 
            { LowestPoint, LowestPoint, LowestPoint, LowestPoint, LowestPoint, LowestPoint, LowestPoint };
        while (!lastOccupied.All(height => height - y >= 0 && height - y < 4 ))
        {
            y--;
            for (var x = 0; x < 7; x++)
            {
                if (Occupied.ContainsKey(new Vector(x, y)))
                    lastOccupied[x] = y;
            }
        }

        return y;
    }

    private void DoCutoff()
    {
        var y = FindCutOff();
        if (y == LowestPoint) return;

        Console.WriteLine($"Doing cutoff at {RocksFallen} rocks");

        LowestPoint = y;
        var old = Occupied;
        Occupied = new Dictionary<Vector, Shape>();
        foreach (var occupied in old.Where(occupied => occupied.Key.Y >= y))
        {
            Occupied.Add(occupied.Key, occupied.Value);
        }
    }

    private IEnumerable<Vector> CoveredByActive(Vector bottomLeft) => TetrisShape.CoveredPoints(Shapes.Current, bottomLeft);

    private bool BlockedBelow =>
        CoveredByActive(CurrentBottomLeft.Down).Any(v => Occupied.ContainsKey(v));

    public void Print()
    {
        foreach (var line in GetLines())
        {
            Console.WriteLine(line);
        }
    }
    
    private IEnumerable<string> GetLines()
    {
        var activeShape = TetrisShape.CoveredPoints(Shapes.Current, CurrentBottomLeft).ToHashSet();
        for (var row = HighestPoint + 10; row >= 0; row--)
        {
            var line = $"{row}\t|";
            for (var col = 0; col < 7; col++)
            {
                var v = new Vector(col, row);
                if (activeShape.Contains(v))
                    line += "@";
                else
                    line += Occupied.TryGetValue(v, out _) ? "#" : ".";
            }

            yield return $"{line}|";
        }

        yield return CurrentBottomLeft.ToString();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Shapes.Dispose();
        Input.Dispose();
    }
}

public record Vector(long X, long Y)
{
    public Vector Down => this with { Y = Y - 1 };
    public Vector Up => this with { Y = Y + 1 };
    public Vector Left => this with { X = X - 1 };
    public Vector Right => this with { X = X + 1 };

    public override string ToString() => $"X: {X}, Y: {Y}";
}

public class TetrisShape
{
    public static IEnumerable<Shape> GenerateShapes()
    {
        var c = -1;
        while (true)
        {
            c = (c + 1) % 5;
            yield return (Shape)c;
        }
    }

    public static IReadOnlyDictionary<Shape, int> Width = new Dictionary<Shape, int>
    {
        { Shape.Dash, 4 },
        { Shape.Cross, 3 },
        { Shape.L, 3 },
        { Shape.Pipe, 1 },
        { Shape.Square, 2 }
    };
    
    public static IReadOnlyDictionary<Shape, int> Height = new Dictionary<Shape, int>
    {
        { Shape.Dash, 1 },
        { Shape.Cross, 3 },
        { Shape.L, 3 },
        { Shape.Pipe, 4 },
        { Shape.Square, 2 }
    };

    public static IEnumerable<Vector> CoveredPoints(Shape shape, Vector bottomLeft) =>
        CoveredPoints(shape, bottomLeft.Y, bottomLeft.X);

    private static IEnumerable<Vector> CoveredPoints(Shape shape, long bottomEdge, long leftEdge) => shape switch
    {
        Shape.Dash => new Vector[]
        {
            new(leftEdge, bottomEdge),
            new(leftEdge + 1, bottomEdge),
            new(leftEdge + 2, bottomEdge),
            new(leftEdge + 3, bottomEdge)
        },
        Shape.Cross => new Vector[]
        {
            new(leftEdge, bottomEdge + 1),
            new(leftEdge + 1, bottomEdge + 1),
            new(leftEdge + 2, bottomEdge + 1),
            new(leftEdge + 1, bottomEdge + 2),
            new(leftEdge + 1, bottomEdge),
        },
        Shape.L => new Vector[]
        {
            new(leftEdge, bottomEdge),
            new(leftEdge + 1, bottomEdge),
            new(leftEdge + 2, bottomEdge),
            new(leftEdge + 2, bottomEdge + 1),
            new(leftEdge + 2, bottomEdge + 2)
        },
        Shape.Pipe => new Vector[]
        {
            new(leftEdge, bottomEdge),
            new(leftEdge, bottomEdge + 1),
            new(leftEdge, bottomEdge + 2),
            new(leftEdge, bottomEdge + 3)
        },
        Shape.Square => new Vector[]
        {
            new(leftEdge, bottomEdge),
            new(leftEdge + 1, bottomEdge),
            new(leftEdge, bottomEdge + 1),
            new(leftEdge + 1, bottomEdge + 1)
        },
        _ => throw new InvalidEnumArgumentException(nameof(shape), (int)shape, typeof(Shape))
    };
}

public enum Shape
{
    Dash = 0,
    Cross = 1,
    L = 2,
    Pipe = 3,
    Square = 4
}