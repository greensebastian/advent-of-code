using System.Text;
using Shouldly;

namespace AdventOfCode2025.Tests.Day09;

public class Day09
{
    private const string Example = """
                                   7,1
                                   11,1
                                   11,7
                                   9,7
                                   9,5
                                   2,5
                                   2,3
                                   7,3
                                   """;
    
    [Fact]
    public void Example_1()
    {
        var lines = Util.ReadRaw(Example);
        var mt = new MovieTheatre(lines);
        mt.BiggestArea().ShouldBe(50);
    }
    
    [Fact]
    public void Real_1()
    {
        var lines = Util.ReadFile("day09");
        var mt = new MovieTheatre(lines);
        mt.BiggestArea().ShouldBe(4777409595L);
    }
    
    [Fact]
    public void Example_2()
    {
        var lines = Util.ReadRaw(Example);
        var mt = new MovieTheatre(lines);
        mt.BiggestRedBlueArea().ShouldBe(24);
    }
    
    [Fact]
    public void Real_2()
    {
        var lines = Util.ReadFile("day09");
        var mt = new MovieTheatre(lines);
        mt.BiggestRedBlueArea().ShouldBe(1473551379L);
    }
}

public class MovieTheatre(IReadOnlyList<string> input)
{
    private IReadOnlyList<Vector> Statues { get; } =
        input.Select(l => new Vector(int.Parse(l.Split(',')[1]), int.Parse(l.Split(',')[0]))).ToList();

    public long BiggestArea()
    {
        var combinations = Statues.SelectMany((s1, i) => Statues.Skip(i + 1).Select(s2 => new Square(s1, s2))).ToArray();
        return combinations.Max(c => c.Area());
    }

    public long BiggestRedBlueArea()
    {
        var columns = Statues.Select(s => s.Col).Order().Distinct().ToArray();
        var rows = Statues.Select(s => s.Row).Order().Distinct().ToArray();
        var filled = new Dictionary<Vector, Spot>();
        foreach (var statue in Statues)
        {
            var normCol = columns.IndexOf(statue.Col);
            var normRow = rows.IndexOf(statue.Row);
            filled[new Vector(normRow, normCol)] = Spot.Corner;
        }
        
        Print(filled);

        var boundaries = new List<Vector>();

        for (var i = 0; i < Statues.Count; i++)
        {
            var s1 = Statues[i];
            var s2 = Statues[(i + 1) % Statues.Count];
            var normS1 = new Vector(rows.IndexOf(s1.Row), columns.IndexOf(s1.Col));
            var normS2 = new Vector(rows.IndexOf(s2.Row), columns.IndexOf(s2.Col));
            if (normS1 == normS2) continue;

            boundaries.Add(normS1);
            foreach (var v in normS1.PathTo(normS2))
            {
                boundaries.Add(v);
                if (!filled.TryAdd(v, Spot.Edge))
                {
                    throw new Exception("Overlap");
                }
            }
        }
        
        Print(filled);

        var corners = boundaries.SelectMany((v1, i) =>
        {
            var v2 = boundaries[(i + 1) % boundaries.Count];
            var v3 = boundaries[(i + 2) % boundaries.Count];
            if (v1.Row != v3.Row && v1.Col != v3.Col)
            {
                return new[] { v1.Add(v3.Sub(v2)) };
            }

            return [];
        }).Where(c => !boundaries.Contains(c)).Distinct().ToArray();

        var minC = boundaries.Min(v => v.Col);
        var maxC = boundaries.Max(v => v.Col);
        var minR = boundaries.Min(v => v.Row);
        var maxR = boundaries.Max(v => v.Row);
        
        foreach (var corner in corners)
        {
            var valid = true;
            var seen = new HashSet<Vector>();
            var queue = new Queue<Vector>();
            queue.Enqueue(corner);
            while (queue.TryDequeue(out var v) && valid)
            {
                if (seen.Contains(v) || filled.ContainsKey(v)) continue;
                seen.Add(v);

                if (v.Row == minR || v.Row == maxR || v.Col == minC || v.Col == maxC)
                {
                    valid = false;
                    continue;
                }
                queue.Enqueue(v with { Col = v.Col + 1});
                queue.Enqueue(v with { Col = v.Col - 1});
                queue.Enqueue(v with { Row = v.Row + 1});
                queue.Enqueue(v with { Row = v.Row - 1});
            }

            if (valid)
            {
                foreach (var v in seen)
                {
                    filled[v] = Spot.Center;
                }
            }
        }
        
        Print(filled);
        
        var combinations = Statues.SelectMany((s1, i) => Statues.Skip(i + 1).Select(s2 => new Square(s1, s2))).OrderByDescending(s => s.Area()).ToArray();
        foreach (var square in combinations)
        {
            if (AllFilled(square)) return square.Area();
        }

        throw new Exception("Failed to solve");

        bool AllFilled(Square square)
        {
            var normFirst = new Vector(rows.IndexOf(square.First.Row), columns.IndexOf(square.First.Col));
            var normSecond = new Vector(rows.IndexOf(square.Second.Row), columns.IndexOf(square.Second.Col));
            var col0 = Math.Min(normFirst.Col, normSecond.Col);
            var col1 = Math.Max(normFirst.Col, normSecond.Col);
            var row0 = Math.Min(normFirst.Row, normSecond.Row);
            var row1 = Math.Max(normFirst.Row, normSecond.Row);
            for (var row = row0; row <= row1; row++)
            {
                for (var col = col0; col <= col1; col++)
                {
                    if (!filled.ContainsKey(new Vector(row, col))) return false;
                }
            }

            return true;
        }
    }

    private static void Print(Dictionary<Vector, Spot> spots)
    {
        var sb = new StringBuilder();
        var minR = spots.Keys.Min(p => p.Row);
        var maxR = spots.Keys.Max(p => p.Row);
        
        var minC = spots.Keys.Min(p => p.Col);
        var maxC = spots.Keys.Max(p => p.Col);

        for (var row = minR; row <= maxR; row++)
        {
            for (var col = minC; col <= maxC; col++)
            {
                if (spots.TryGetValue(new Vector(row, col), out var s))
                {
                    sb.Append(s switch
                    {
                        Spot.Corner => '#',
                        Spot.Edge => '+',
                        Spot.Center => 'X',
                        _ => throw new ArgumentOutOfRangeException()
                    });
                }
                else sb.Append('.');
            }

            sb.AppendLine();
        }
        
        Console.WriteLine(sb.ToString());
    }
}

public enum Spot
{
    Corner,
    Edge,
    Center
}

public record Vector(long Row, long Col)
{
    public IEnumerable<Vector> PathTo(Vector other)
    {
        if (Row == other.Row)
        {
            var dir = other.Col > Col ? 1 : -1;
            for (var col = Col + dir; col != other.Col; col += dir)
            {
                yield return this with { Col = col };
            }
        }
        else
        {
            var dir = other.Row > Row ? 1 : -1;
            for (var row = Row + dir; row != other.Row; row += dir)
            {
                yield return this with { Row = row };
            }
        }
    }

    public Vector Add(Vector other) => new(Row + other.Row, Col + other.Col);
    public Vector Sub(Vector other) => new(Row - other.Row, Col - other.Col);
}

public record Square(Vector First, Vector Second)
{
    public long Area() => (Math.Abs(Second.Row - First.Row) + 1) * (Math.Abs(Second.Col - First.Col) + 1);
}
