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
        var input = Util.ReadRaw(Example);
        //var input = Util.ReadFile("day12");

        var sum = FencePrice(input);
        sum.Should().Be(140);
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
}