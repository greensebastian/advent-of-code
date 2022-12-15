namespace AdventOfCode2022.Core.Day14;

public record Day14Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var cave = new BottomlessCave(Input);

        while (cave.DoRound())
        {
            //cave.Print();
        }

        yield return cave.SandCount.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var cave = new WideBottomCave(Input);

        while (cave.DoRound())
        {
            //cave.Print();
        }

        yield return cave.SandCount.ToString();
    }
}

public enum CaveItem
{
    Rock,
    Sand,
    Start
}

public class WideBottomCave : Cave
{
    public WideBottomCave(IEnumerable<string> input) : base(input)
    {
        GenerateWideBottom();
    }

    private void GenerateWideBottom()
    {
        var bottomHeight = MaxY + 2;
        var caveHeight = MaxY - MinY;

        const int margin = 10;
        var farLeft = MinX - caveHeight - margin;
        var farRight = MaxX + caveHeight + margin;

        for (var x = farLeft; x <= farRight; x++)
        {
            Items[new Vector(x, bottomHeight)] = CaveItem.Rock;
        }
    }

    public override bool EndConditionMet => Items.ContainsKey(StartPos);
}

public class BottomlessCave : Cave
{
    private int LowestRock { get; }
    private int GetLowestRock() => Items
        .Where(kv => kv.Value == CaveItem.Rock)
        .Select(kv => kv.Key.Y)
        .Max();
    
    public BottomlessCave(IEnumerable<string> input) : base(input)
    {
        LowestRock = GetLowestRock();
    }
    
    public override bool EndConditionMet => ActiveSand.Y == LowestRock;
}

public abstract class Cave
{
    protected Dictionary<Vector, CaveItem> Items { get; } = new();

    protected static Vector StartPos => new(500, 0);

    public int SandCount => Items.Values.Count(i => i == CaveItem.Sand);

    protected Vector ActiveSand { get; set; } = StartPos;

    protected Cave(IEnumerable<string> input)
    {
        var paths = input.Select(line => new RockPath(line));
        foreach (var path in paths)
        {
            foreach (var rockPosition in path.OccupiedPositions)
            {
                Items[rockPosition] = CaveItem.Rock;
            }
        }
    }

    public abstract bool EndConditionMet { get; }
    
    /// <summary>
    /// Do a round of simulation
    /// </summary>
    /// <returns>True if round occurred</returns>
    public bool DoRound()
    {
        if (EndConditionMet)
            return false;
        
        // Check below, left, right
        var below = ActiveSand.Below;
        if (!Items.TryGetValue(below, out _))
        {
            ActiveSand = below;
            return true;
        }

        var belowLeft = below.Left;
        if (!Items.TryGetValue(belowLeft, out _))
        {
            ActiveSand = belowLeft;
            return true;
        }
        
        var belowRight = below.Right;
        if (!Items.TryGetValue(belowRight, out _))
        {
            ActiveSand = belowRight;
            return true;
        }

        // Create new at top
        Items.Add(ActiveSand, CaveItem.Sand);
        ActiveSand = StartPos;
        return true;
    }

    public void Print()
    {
        foreach (var line in GetPrintLines())
        {
            Console.WriteLine(line);
        }
    }

    protected int MaxY => Items.Keys.Select(k => k.Y).Max();
    protected int MinY => Items.Keys.Select(k => k.Y).Min();
    protected int MaxX => Items.Keys.Select(k => k.X).Max();
    protected int MinX => Items.Keys.Select(k => k.X).Min();

    private IEnumerable<string> GetPrintLines()
    {
        var maxY = MaxY;
        var minY = MinY;
        var maxX = MaxX;
        var minX = MinX;

        for (var y = minY; y <= maxY; y++)
        {
            var row = $"{y}:\t";
            for (var x = minX; x <= maxX; x++)
            {
                if (ActiveSand.X == x && ActiveSand.Y == y)
                {
                    row += "~";
                }
                else
                {
                    var symbol = Items.TryGetValue(new Vector(x, y), out var item)
                        ? item switch
                        {
                            CaveItem.Rock => "#",
                            CaveItem.Sand => "o",
                            CaveItem.Start => "+",
                            _ => " "
                        }
                        : " ";
                    row += symbol;
                }
            }
            yield return row;
        }
    }
}

public record RockPath(string Input)
{
    private List<Vector> Lines { get; } = Input
        .Ints()
        .Chunk(2)
        .Select(points => new Vector(points[0], points[1]))
        .ToList();

    public HashSet<Vector> OccupiedPositions
    {
        get
        {
            var set = new HashSet<Vector>();

            var current = Lines[0];
            for (var lineIndex = 1; lineIndex < Lines.Count; lineIndex++)
            {
                var next = Lines[lineIndex];
                foreach (var point in current.PointsOnVectorTo(next))
                {
                    set.Add(point);
                }

                current = next;
            }
            
            return set;
        }
    }
}

public record Vector(int X, int Y)
{
    public Vector Below => this with { Y = Y + 1 };
    public Vector Left => this with { X = X - 1 };
    public Vector Right => this with { X = X + 1 };
    
    private Vector VectorTo(Vector other) => new(other.X - X, other.Y - Y);

    public IEnumerable<Vector> PointsOnVectorTo(Vector other)
    {
        var vectorToOther = VectorTo(other);
        if (vectorToOther.X < 0)
        {
            for (var dx = 0; dx >= vectorToOther.X; dx--)
            {
                yield return this with { X = X + dx };
            }
        }
        else if (vectorToOther.X > 0)
        {
            for (var dx = 0; dx <= vectorToOther.X; dx++)
            {
                yield return this with { X = X + dx };
            }
        }
        else if (vectorToOther.Y < 0)
        {
            for (var dy = 0; dy >= vectorToOther.Y; dy--)
            {
                yield return this with { Y = Y + dy };
            }
        }
        else if (vectorToOther.Y > 0)
        {
            for (var dy = 0; dy <= vectorToOther.Y; dy++)
            {
                yield return this with { Y = Y + dy };
            }
        }
    }
}

internal static class EnumerableExtensions
{
    public static IEnumerable<int> Ints(this IEnumerable<char> source)
    {
        var currentNumber = string.Empty;
        foreach (var c in source)
        {
            if (char.IsNumber(c))
            {
                currentNumber += c;
            }
            else
            {
                if (string.IsNullOrEmpty(currentNumber)) continue;
                
                yield return int.Parse(currentNumber);
                currentNumber = string.Empty;
            }
        }

        if (currentNumber.Length > 0) yield return int.Parse(currentNumber);
    }
}