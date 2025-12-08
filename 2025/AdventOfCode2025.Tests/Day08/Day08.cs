using Shouldly;

namespace AdventOfCode2025.Tests.Day08;

public class Day08
{
    private const string Example = """
                                   162,817,812
                                   57,618,57
                                   906,360,560
                                   592,479,940
                                   352,342,300
                                   466,668,158
                                   542,29,236
                                   431,825,988
                                   739,650,466
                                   52,470,668
                                   216,146,977
                                   819,987,18
                                   117,168,530
                                   805,96,715
                                   346,949,466
                                   970,615,88
                                   941,993,340
                                   862,61,35
                                   984,92,344
                                   425,690,689
                                   """;
    
    [Fact]
    public void Example_1()
    {
        var lines = Util.ReadRaw(Example);
        var bn = new BoxNetwork(lines);
        bn.NetworksMultiplied(10).ShouldBe(40);
    }
    
    [Fact]
    public void Real_1()
    {
        var lines = Util.ReadFile("day08");
        var bn = new BoxNetwork(lines);
        bn.NetworksMultiplied(1000).ShouldBe(115885L);
        // 707 too low (overflow)
    }
    
    [Fact]
    public void Example_2()
    {
        var lines = Util.ReadRaw(Example);
    }
    
    [Fact]
    public void Real_2()
    {
        var lines = Util.ReadFile("day07");
    }
}

public record Vector(long X, long Y, long Z)
{
    public static Vector FromLine(string l) => new(int.Parse(l.Split(',')[0]), int.Parse(l.Split(',')[1]),
        int.Parse(l.Split(',')[2]));

    public long Sum() => X + Y + Z;

    public Vector Sub(Vector other) => new(X - other.X, Y - other.Y, Z - other.Z);

    public double Len() => Math.Sqrt(X * X + Y * Y + Z * Z);
}

public record NodeDistance(Vector Small, Vector Big, double Distance)
{
    public static NodeDistance FromPair(Vector a, Vector b)
    {
        if (a == b) return new NodeDistance(a, b, 0);
        var diff = b.Sub(a);
        var distance = diff.Len();
        if (a.Sum() < b.Sum()) return new NodeDistance(a, b, distance);
        if (a.Sum() > b.Sum()) return new NodeDistance(b, a, distance);

        if (a.X < b.X) return new NodeDistance(a, b, distance);
        if (a.X > b.X) return new NodeDistance(b, a, distance);

        if (a.Y < b.Y) return new NodeDistance(a, b, distance);
        if (a.Y > b.Y) return new NodeDistance(b, a, distance);

        if (a.Z < b.Z) return new NodeDistance(a, b, distance);
        return new NodeDistance(b, a, distance);
    }
}

public class BoxNetwork(IReadOnlyList<string> input)
{
    private IReadOnlyList<Vector> Boxes { get; } = input.Select(Vector.FromLine).ToArray();

    public long NetworksMultiplied(int connectionCount)
    {
        var distances = Boxes.SelectMany((b1, i) => Boxes.Skip(i + 1).Select(b2 => NodeDistance.FromPair(b1, b2)))
            .OrderBy(d => d.Distance)
            .ToArray();
        var circuits = new List<HashSet<Vector>>();
        for (var i = 0; i < connectionCount; i++)
        {
            var distance = distances[i];
            var cSmall = circuits.SingleOrDefault(c => c.Contains(distance.Small));
            var cBig = circuits.SingleOrDefault(c => c.Contains(distance.Big));

            if (cSmall == null && cBig == null)
            {
                circuits.Add([distance.Small, distance.Big]);
            }
            else if (cSmall != null && cBig != null)
            {
                if (cSmall == cBig) continue;
                foreach (var vector in cBig)
                {
                    cSmall.Add(vector);
                }

                circuits.Remove(cBig);
            }
            else if (cSmall != null) cSmall.Add(distance.Big);
            else if (cBig != null) cBig.Add(distance.Small);
            else
            {
                throw new Exception("Should not happen");
            }
            Print(circuits);
        }

        return circuits.OrderByDescending(c => c.Count).Take(3).Aggregate(1L, (product, circuit) => product * circuit.Count );
    }

    private static void Print(List<HashSet<Vector>> vectors)
    {
        Console.WriteLine(string.Join('\n', vectors));
    }
}
