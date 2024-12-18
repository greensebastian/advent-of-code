using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day12 : ISolution
{
    private const string Example = """
                                   RRRRIICCFF
                                   RRRRIICCCF
                                   VVRRRCCFFF
                                   VVRCCCJFFF
                                   VVVVCJJCFE
                                   VVIVCCJJEE
                                   VVIIICJJEE
                                   MIIIIIJJEE
                                   MIIISIJEEE
                                   MMMISSJEEE
                                   """;
    
    private const string Example2 = """
                                    AAAAAA
                                    AAABBA
                                    AAABBA
                                    ABBAAA
                                    ABBAAA
                                    AAAAAA
                                    """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day12");

        var sum = FencePrice(input);
        sum.Should().Be(1461752L);
    }

    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day12");

        var sum = FenceBulkPrice(input);
        sum.Should().Be(904114L);
        
        // 898582 too low
    }
    
    [Fact]
    public void Solution2Example2()
    {
        var input = Util.ReadRaw(Example2);
        //var input = Util.ReadFile("day12");

        var sum = FenceBulkPrice(input);
        sum.Should().Be(368);
        
        // 898582 too low
    }

    private long FencePrice(string[] input)
    {
        var sum = 0L;
        var map = Point.GetMap(input, c => c);
        var cropTypes = map.GroupBy(p => p.Value);
        foreach (var cropType in cropTypes)
        {
            var unseen = cropType.Select(ct => ct.Key).ToList();
            var regions = new List<List<Point>>();
            while (unseen.Count > 0)
            {
                var newRegion = unseen.First().DepthFirstEnumerate(unseen,
                    point => point.ClockwiseOrthogonalNeighbours().Where(p => map.TryGetValue(p, out var val) && val == cropType.Key)).ToList();
                regions.Add(newRegion);
            }

            foreach (var region in regions)
            {
                var fences = new HashSet<Vector>();
                foreach (var square in region)
                {
                    foreach (var orthogonalNeighbour in square.ClockwiseOrthogonalNeighbours())
                    {
                        if (!map.TryGetValue(orthogonalNeighbour, out var neighbourChar) || neighbourChar != cropType.Key)
                        {
                            fences.Add(new Vector(square, orthogonalNeighbour));
                        }
                    }
                }

                var regionCost = region.Count * fences.Count;
                sum += regionCost;
            }
        }

        return sum;
    }
    
    private long FenceBulkPrice(string[] input)
    {
        var sum = 0L;
        var map = Point.GetMap(input, c => c);
        var cropTypes = map.GroupBy(p => p.Value);
        foreach (var cropType in cropTypes)
        {
            var unseen = cropType.Select(ct => ct.Key).ToList();
            var regions = new List<List<Point>>();
            while (unseen.Count > 0)
            {
                var newRegion = unseen.First().DepthFirstEnumerate(unseen,
                    point => point.ClockwiseOrthogonalNeighbours().Where(p => map.TryGetValue(p, out var val) && val == cropType.Key)).ToList();
                regions.Add(newRegion);
            }

            foreach (var region in regions)
            {
                var fences = new HashSet<Vector>();
                foreach (var square in region)
                {
                    foreach (var orthogonalNeighbour in square.ClockwiseOrthogonalNeighbours())
                    {
                        if (!map.TryGetValue(orthogonalNeighbour, out var neighbourChar) || neighbourChar != cropType.Key)
                        {
                            fences.Add(new Vector(square, orthogonalNeighbour));
                        }
                    }
                }

                var verticalFences = fences.Where(f => f.Start.Row == f.End.Row).OrderBy(f => f.OrderFromTopLeft()).GroupBy(f => $"{f.Start.Col}-{f.End.Col}");
                var horizontalFences = fences.Where(f => f.Start.Col == f.End.Col).OrderBy(f => f.OrderFromTopLeft()).GroupBy(f => $"{f.Start.Row}-{f.End.Row}");

                var sideCount = 0;
                foreach (var col in verticalFences.Select(c => c.ToArray()))
                {
                    sideCount++;
                    for (var i = 1; i < col.Length; i++)
                    {
                        if (col[i].FirstInReadingOrder().Row - col[i - 1].FirstInReadingOrder().Row != 1) sideCount++;
                    }
                }
                
                foreach (var row in horizontalFences.Select(c => c.ToArray()))
                {
                    sideCount++;
                    for (var i = 1; i < row.Length; i++)
                    {
                        if (row[i].FirstInReadingOrder().Col - row[i - 1].FirstInReadingOrder().Col != 1) sideCount++;
                    }
                }
                
                var regionCost = region.Count * sideCount;
                sum += regionCost;
            }
        }

        return sum;
    }
}