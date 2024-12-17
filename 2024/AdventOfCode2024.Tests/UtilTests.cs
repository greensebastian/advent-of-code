using FluentAssertions;

namespace AdventOfCode2024.Tests;

public class UtilTests
{
    [Fact]
    public void PointsEqual()
    {
        var p1 = new Point(1, 1);
        var dict = new Dictionary<Point, int>(new []{ new KeyValuePair<Point, int>(p1, 1) });
        dict[p1].Should().Be(1);
        dict[new Point(1, 1)].Should().Be(1);
        
        var map = new PointMap<int>(new []{ new KeyValuePair<Point, int>(p1, 1) });
        map[p1].Should().Be(1);
        map[new Point(1, 1)].Should().Be(1);
    }
}