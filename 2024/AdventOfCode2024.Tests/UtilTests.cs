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

    [Fact]
    public void Combinations()
    {
        var c1 = "ab".Combinations();
        c1.Select(c => string.Join("", c)).Should().ContainInOrder(["aa", "ab", "ba", "bb"]);
        
        var c2 = "ab".Combinations(3);
        c2.Select(c => string.Join("", c)).Should().ContainInOrder(["aaa", "aab", "aba", "abb", "baa", "bab", "bba", "bbb"]);
        
        var c3 = "abc".Combinations();
        c3.Select(c => string.Join("", c)).Should().ContainInOrder(["aa", "ab", "ac", "ba", "bb", "bc", "ca", "cb", "cc"]);
    }
    
    [Theory]
    [InlineData(1, 1, 2, 2, true)]
    [InlineData(1, 2, 2, 4, true)]
    [InlineData(2, 1, 1, 1, false)]
    [InlineData(3, 2, 12, 6, false)]
    [InlineData(3, 2, 12, 8, true)]
    public void RepeatsIn(long r1, long c1, long r2, long c2, bool expectedEqual)
    {
        new Point(r1, c1).RepeatsIn(new Point(r2, c2)).Should().Be(expectedEqual);
    }

    [Fact]
    public void ReachableIn()
    {
        var p = new Point(10, 10);
        p.ReachableIn(1).Should()
            .ContainInOrder([new Point(9, 10), new Point(10, 9), new Point(10, 11), new Point(11, 10)]);

        var reached = p.ReachableIn(2).ToArray();
        reached.Should()
            .ContainInOrder([
                new Point(8, 10),
                new Point(9, 9), new Point(9, 10), new Point(9, 11),
                new Point(10, 8), new Point(10, 9), new Point(10, 11), new Point(10, 12),
                new Point(11, 9), new Point(11, 10), new Point(11, 11),
                new Point(12, 10)
            ]);
    }
}