using Shouldly;

namespace AdventOfCode2025.Tests.Day04;

public class Day04
{
    private const string Example = """
                                   ..@@.@@@@.
                                   @@@.@.@.@@
                                   @@@@@.@.@@
                                   @.@@@@..@.
                                   @@.@@@@.@@
                                   .@@@@@@@.@
                                   .@.@.@.@@@
                                   @.@@@.@@@@
                                   .@@@@@@@@.
                                   @.@.@@@.@.
                                   """;
    
    [Fact]
    public void Example_1()
    {
        var lines = Util.ReadRaw(Example);
        var ranges = new PaperRollWarehouse(lines);
        ranges.AccessibleRollCount().ShouldBe(13);
    }
    
    [Fact]
    public void Real_1()
    {
        var lines = Util.ReadFile("day04");
        var ranges = new PaperRollWarehouse(lines);
        ranges.AccessibleRollCount().ShouldBe(1523);
    }
    
    [Fact]
    public void Example_2()
    {
        var lines = Util.ReadRaw(Example);
        var ranges = new PaperRollWarehouse(lines);
        ranges.RemovableRollCount().ShouldBe(43);
    }
    
    [Fact]
    public void Real_2()
    {
        var lines = Util.ReadFile("day04");
        var ranges = new PaperRollWarehouse(lines);
        ranges.RemovableRollCount().ShouldBe(9290);
    }
}

public class PaperRollWarehouse(IReadOnlyList<string> input)
{
    public int AccessibleRollCount()
    {
        var map = new PointMap<bool>(input.SelectMany((l, row) =>
            l.Select((c, col) => new KeyValuePair<Point, bool>(new(row, col), c == '@'))).ToArray());

        var accessible = 0;
        
        for (var row = map.Min.Row; row <= map.Max.Row; row++)
        {
            for (var col = map.Min.Col; col <= map.Max.Col; col++)
            {
                var p = new Point(row, col);
                if (map.Get(p) && p.ClockwiseNeighbours().Count(n => map.Get(n)) < 4) accessible++;
            }
        }

        return accessible;
    }
    
    public int RemovableRollCount()
    {
        var map = new PointMap<bool>(input.SelectMany((l, row) =>
            l.Select((c, col) => new KeyValuePair<Point, bool>(new(row, col), c == '@'))).ToArray());
        
        var doMore = true;
        var removed = 0;
        
        while (doMore)
        {
            var removable = new List<Point>();
        
            for (var row = map.Min.Row; row <= map.Max.Row; row++)
            {
                for (var col = map.Min.Col; col <= map.Max.Col; col++)
                {
                    var p = new Point(row, col);
                    if (map.Get(p) && p.ClockwiseNeighbours().Count(n => map.Get(n)) < 4) removable.Add(p);
                }
            }

            map = new PointMap<bool>(map.Points.Where(kv => !removable.Contains(kv.Key)).ToArray());
            removed += removable.Count;
            if (removable.Count == 0) doMore = false;
        }
        
        return removed;
    }
}