namespace AdventOfCode2022.Core.Day14;

public record Day14Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var cave = new Cave(Input);

        while (cave.DoRound())
        {
            cave.Print();
        }

        yield return cave.SandCount.ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        yield return "0";
    }
}

public enum CaveItem
{
    Empty,
    Rock,
    Sand,
    Start
}

public class Cave
{
    private Dictionary<Vector, CaveItem> Items { get; } = new()
    {
        { StartPos, CaveItem.Start }
    };

    private static Vector StartPos => new(500, 0);

    private int BottomRock => Items
        .Where(kv => kv.Value == CaveItem.Rock)
        .Select(kv => kv.Key.Y)
        .Max();

    public int SandCount => Items.Values.Count(i => i == CaveItem.Sand);
    
    private Vector ActiveSand { get; set; } = StartPos;

    public Cave(IEnumerable<string> input)
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

    /// <summary>
    /// Do a round of simulation
    /// </summary>
    /// <returns>True if round occurred</returns>
    public bool DoRound()
    {
        if (ActiveSand.Y == BottomRock)
        {
            // End
            return false;
        }

        // Check below, left, right
        var below = ActiveSand with { Y = ActiveSand.Y + 1 };
        if (!Items.TryGetValue(below, out _))
        {
            ActiveSand = below;
            return true;
        }

        var belowLeft = below with { X = below.X - 1 };
        if (!Items.TryGetValue(belowLeft, out _))
        {
            ActiveSand = belowLeft;
            return true;
        }
        
        var belowRight = below with { X = below.X + 1 };
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

    private IEnumerable<string> GetPrintLines()
    {
        var maxY = Items.Keys.Select(k => k.Y).Max();
        var minY = Items.Keys.Select(k => k.Y).Min();
        var maxX = Items.Keys.Select(k => k.X).Max();
        var minX = Items.Keys.Select(k => k.X).Min();

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