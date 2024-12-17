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
    
    [Fact]
    public void RotationsWork()
    {
        var up = Point.Origin.Up;
        var rotations = new[] {up.RotateClockwise(1), up.RotateClockwise(2), up.RotateClockwise(3), up.RotateClockwise(4)};
        var expected = new[] {Point.Origin.Right, Point.Origin.Down, Point.Origin.Left, Point.Origin.Up};
        for (var i = 0; i < 4; i++)
        {
            rotations[i].Row.Should().Be(expected[i].Row);
            rotations[i].Col.Should().Be(expected[i].Col);
        }
    }
}