using System.Text;

namespace AdventOfCode2023.Core.Day22;

public record Day22Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var lines = Input.ToArray();
        var bricks = new FallingBricks(lines);
        var ans = bricks.SafelyDisintegratableBricks();

        yield return ans.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var lines = Input.ToArray();
        var bricks = new FallingBricks(lines);
        var ans = bricks.SumOfChainReactions();

        yield return ans.ToString();
    }
}

public class FallingBricks(IReadOnlyList<string> input)
{
    public Dictionary<string, Brick> FloatingBricks { get; } =
        input.Select((line, i) => Brick.FromInput(line, $"B{i:0000}")).ToDictionary(b => b.Id);

    public int SafelyDisintegratableBricks()
    {
        var maxX = FloatingBricks.Values.Max(b => b.End.X);
        var maxY = FloatingBricks.Values.Max(b => b.End.Y);
        var ground = new Brick("G", new Vector(0, 0, 0), new Vector(maxX, maxY, 0), "GROUND");
        var bricks = new List<Brick> { ground };
        var dependencies = new Dictionary<string, IReadOnlySet<string>>
        {
            { ground.Id, new HashSet<string>() }
        };
        foreach (var kv in FloatingBricks.OrderBy(kv => kv.Value.End.Z))
        {
            var brick = kv.Value;
            var relevantBlocks = bricks.Where(other => brick.Intersects(other)).ToArray();
            var height = relevantBlocks.Max(b => b.End.Z);
            var contacting = relevantBlocks.Where(b => b.End.Z == height).ToArray();
            var droppedBrick = brick.RestingOn(height);
            bricks.Add(droppedBrick);
            dependencies[droppedBrick.Id] = contacting.Select(b => b.Id).ToHashSet();
            //Console.WriteLine(Print(bricks));
        }

        var disintegratable = bricks.Where(b =>
        {
            var dependedOn = dependencies.Values.Any(d => d.Contains(b.Id));
            if (!dependedOn) return true;

            return dependencies.Values.Where(d => d.Contains(b.Id)).All(d => d.Count > 1);
        }).ToArray();
        return disintegratable.Length;
    }

    public int SumOfChainReactions()
    {
        var maxX = FloatingBricks.Values.Max(b => b.End.X);
        var maxY = FloatingBricks.Values.Max(b => b.End.Y);
        var ground = new Brick("G", new Vector(0, 0, 0), new Vector(maxX, maxY, 0), "GROUND");
        var bricks = new List<Brick> { ground };
        var supporting = new Dictionary<string, IReadOnlySet<string>>
        {
            { ground.Id, new HashSet<string>() }
        };
        foreach (var kv in FloatingBricks.OrderBy(kv => kv.Value.End.Z))
        {
            var brick = kv.Value;
            var relevantBlocks = bricks.Where(other => brick.Intersects(other)).ToArray();
            var height = relevantBlocks.Max(b => b.End.Z);
            var contacting = relevantBlocks.Where(b => b.End.Z == height).ToArray();
            var droppedBrick = brick.RestingOn(height);
            bricks.Add(droppedBrick);
            supporting[droppedBrick.Id] = contacting.Select(b => b.Id).ToHashSet();
            //Console.WriteLine(Print(bricks));
        }

        var supportedBy = supporting.Keys.ToDictionary(key => key, key =>
        {
            var above = supporting.Where(kv => kv.Value.Contains(key)).Select(kv => kv.Key).ToHashSet();
            return above;
        });
        
        IReadOnlySet<string> Unsupported(IReadOnlySet<string> removed)
        {
            return removed.SelectMany(removedBlock => supportedBy[removedBlock])
                .Where(supportedByRemoved => !removed.Contains(supportedByRemoved) && supporting[supportedByRemoved].All(removed.Contains)).ToHashSet();
        }

        var chainCounts = bricks.Where(b => b != ground).ToDictionary(b => b.Id, b =>
        {
            var removed = new HashSet<string> { b.Id };

            var unsupported = Unsupported(removed);
            while (unsupported.Count > 0)
            {
                foreach (var unsupportedBlock in unsupported)
                {
                    removed.Add(unsupportedBlock);
                }

                unsupported = Unsupported(removed);
            }
            
            return removed.Count-1;
        });
        return chainCounts.Sum(kv => kv.Value);
    }

    private char Print(IReadOnlyList<Brick> bricks, Brick toCheck)
    {
        var collision = bricks.Where(b => b.Intersects(toCheck, true)).ToArray();
        return collision.Length switch
        {
            0 => '.',
            1 => collision.Single().Id.Last(),
            _ => '?'
        };
    }

    public string Print(IReadOnlyList<Brick> bricks, int height = 50)
    {
        var top = bricks.Max(b => b.End.Z);
        var maxX = bricks.Max(b => b.End.X);
        var maxY = bricks.Max(b => b.End.Y);

        var sb = new StringBuilder();
        for (var z = top; z >= Math.Max(top - height, 0) ; z--)
        {
            sb.Append($"Z: {z,-4}");
            
            for (var x = 0; x <= maxX; x++)
            {
                var v = new Brick("", new(x, 0, z), new Vector(x, maxY, z), "");
                sb.Append(Print(bricks, v));
            }

            sb.Append(" | ");
            
            for (var y = 0; y <= maxY; y++)
            {
                var v = new Brick("", new(0, y, z), new Vector(maxX, y, z), "");
                sb.Append(Print(bricks, v));
            }

            sb.AppendLine();
        }

        sb.AppendLine();
        
        var maxZ = bricks.Max(b => b.End.Z);
        sb.AppendLine($"Z {maxZ}");
        for (var y = 0; y <= maxY; y++)
        {
            for (var x = 0; x <= maxX; x++)
            {
                var v = new Brick("", new(x, y, maxZ), new Vector(x, y, maxZ), "");
                sb.Append(Print(bricks, v));
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}

public readonly record struct Vector(int X, int Y, int Z);

public readonly record struct Brick(string Id, Vector Start, Vector End, string Definition)
{
    public static Brick FromInput(string input, string id)
    {
        var ints = input.Split("~").SelectMany(p => p.Split(",")).Select(int.Parse).ToArray();
        var raw = new Brick(id, new(ints[0], ints[1], ints[2]), new(ints[3], ints[4], ints[5]), input);
        var corners = raw.Corners().OrderBy(v => v.X + v.Y + v.Z).ToArray();
        return raw with { Start = corners.First(), End = corners.Last() };
    }

    public IEnumerable<Vector> Corners()
    {
        yield return new(Start.X, Start.Y, Start.Z);
        yield return new(Start.X, Start.Y, End.Z);
        yield return new(Start.X, End.Y, Start.Z);
        yield return new(Start.X, End.Y, End.Z);
        yield return new(End.X, Start.Y, Start.Z);
        yield return new(End.X, Start.Y, End.Z);
        yield return new(End.X, End.Y, Start.Z);
        yield return new(End.X, End.Y, End.Z);
    }

    public bool Intersects(Brick other, bool checkHeight = false)
    {
        var misses = End.X < other.Start.X || Start.X > other.End.X || End.Y < other.Start.Y || Start.Y > other.End.Y;
        if (checkHeight)
        {
            misses = misses || End.Z < other.Start.Z || Start.Z > other.End.Z;
        }
        return !misses;
    }

    public Brick RestingOn(int otherZ) => this with
    {
        Start = Start with { Z = otherZ + 1 }, End = End with { Z = End.Z - Start.Z + otherZ + 1 }
    };
}