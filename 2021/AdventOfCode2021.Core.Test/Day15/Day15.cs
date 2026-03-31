using Shouldly;

namespace AdventOfCode2021.Core.Test.Day15;

public class Day15
{
    private const string Example = """
                                   1163751742
                                   1381373672
                                   2136511328
                                   3694931569
                                   7463417111
                                   1319128137
                                   1359912421
                                   3125421639
                                   1293138521
                                   2311944581
                                   """;
    
    [Fact]
    public void Day15_1_Example()
    {
        var input = Example.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var set = new RiskyCave(input, 1);
        var ans = set.LowestRisk();
        ans.ShouldBe(40);
    }
    
    [Fact]
    public void Day15_1_Real()
    {
        var input = Util.ReadFile("day15");
        var set = new RiskyCave(input, 1);
        var ans = set.LowestRisk();
        ans.ShouldBe(673);
    }
    
    [Fact]
    public void Day15_2_Example()
    {
        var input = Example.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var set = new RiskyCave(input, 5);
        var ans = set.LowestRisk();
        ans.ShouldBe(315);
    }
    
    [Fact]
    public void Day15_2_Real()
    {
        var input = Util.ReadFile("day15");
        var set = new RiskyCave(input, 5);
        var ans = set.LowestRisk();
        ans.ShouldBe(2893);
        // 3297 too high
    }
}

public class RiskyCave
{
    public RiskyCave(IReadOnlyList<string> input, int multiplier)
    {
        var height = input.Count;
        var width = input[0].Length;
        for(var rowMult = 0; rowMult < multiplier; rowMult++)
        {
            for (var row = 0; row < input.Count; row++)
            {
                for (var colMult = 0; colMult < multiplier; colMult++)
                {
                    for (var col = 0; col < input[row].Length; col++)
                    {
                        var p = new Point(row + rowMult * height, col + colMult * width);
                        var risk = int.Parse(input[row][col].ToString()) + colMult + rowMult;
                        if (risk > 9) risk -= 9;
                        RiskLevels[p] = risk;
                    }
                }
            }
        }

        End = RiskLevels.MaxBy(p => p.Key.Col + p.Key.Row).Key;
    }

    private Dictionary<Point, int> RiskLevels { get; } = new();
    private Point End { get; }
    private Point Start { get; } = new(0, 0);

    private int Width
    {
        get
        {
            if (field == 0) field = RiskLevels.Max(p => p.Key.Col);
            return field;
        }
    }

    private int Height
    {
        get
        {
            if (field == 0) field = RiskLevels.Max(p => p.Key.Row);
            return field;
        }
    }

    public int LowestRisk()
    {
        var bestPaths = new Dictionary<Point, Path>();
        var queue = new PriorityQueue<Path, int>();
        queue.Enqueue(new Path(Start, null, 0, Remainder(Start)), 0);
        while (queue.TryDequeue(out var curr, out _))
        {
            if (curr.Pos == End) return curr.KnownRisk;
            if (bestPaths.TryGetValue(curr.Pos, out var bestCurr) && curr.KnownRisk >= bestCurr.KnownRisk) continue;
            bestPaths[curr.Pos] = curr;
            foreach (var neighbor in curr.Pos.Neighbors(Start, End))
            {
                if (neighbor == curr.Prev?.Pos) continue;
                var next = new Path(neighbor, curr, curr.KnownRisk + RiskLevels[neighbor], Remainder(neighbor));
                queue.Enqueue(next, next.KnownRisk + next.Remainder);
            }
        }

        throw new ApplicationException("Failed to find path.");
    }
    
    private int Remainder(Point p) => Width + 1 - p.Col + Height + 1 - p.Row;
}


public record Point(int Row, int Col)
{
    public Point Up() => this with { Row = Row - 1 };
    public Point Down() => this with { Row = Row + 1 };
    public Point Left() => this with { Col = Col - 1 };
    public Point Right() => this with { Col = Col + 1 };

    public IEnumerable<Point> Neighbors(Point lowBoundInclusive, Point highBoundInclusive)
    {
        var neighbours = new[] { Up(), Right(), Down(), Left() };
        return neighbours.Where(pos =>
            pos.Col >= lowBoundInclusive.Col && pos.Row >= lowBoundInclusive.Col && pos.Col <= highBoundInclusive.Col &&
            pos.Row <= highBoundInclusive.Row);
    }

    public override string ToString() => $"[R:{Row}\tC{Col}:]";
}

public class Path
{
    public Path(Point pos, Path? prev, int knownRisk, int remainder)
    {
        Pos = pos;
        Prev = prev;
        KnownRisk = knownRisk;
        Remainder = remainder;
    }

    public Point Pos { get; }
    public Path? Prev { get; }
    public int KnownRisk { get; }
    public int Remainder { get; }
}