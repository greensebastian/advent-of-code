using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day06 : ISolution
{
    private const string Example = """
                                   ....#.....
                                   .........#
                                   ..........
                                   ..#.......
                                   .......#..
                                   ..........
                                   .#..^.....
                                   ........#.
                                   #.........
                                   ......#...
                                   """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day06");

        var answer = DistinctGuardLocationsCount(input);
        answer.Should().Be(4696);
    }

    [Fact]
    public void Solution2()
    {
        var input = Util.ReadRaw(Example);
        //var input = Util.ReadFile("day06");

        var answer = DistinctGuardLocationsCount(input);
        answer.Should().Be(4260);
    }

    private int DistinctGuardLocationsCount(string[] lines)
    {
        var map = Point.GetMap(lines, c => c);
        map.Print();
        
        var position = map.Single(pair => pair.Value == '^').Key;
        var dir = Point.Origin.Up;
        var seen = new HashSet<Point>();

        while (map.ContainsKey(position))
        {
            seen.Add(position);
            if (map.TryGetValue(position + dir, out var forwardChar) && forwardChar == '#') dir = dir.RotateClockwise(1);
            position += dir;
        }
        
        return seen.Count;
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