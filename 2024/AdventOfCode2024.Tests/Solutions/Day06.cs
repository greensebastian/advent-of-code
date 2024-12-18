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
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day06");

        var answer = DistinctLocationsCausingGuardLoop(input);
        answer.Should().Be(1443);
        
        // 1326 too low
        // 1327 too low
        // 1444 too high
    }

    private int DistinctGuardLocationsCount(string[] lines)
    {
        var map = Point.GetMap(lines, c => c);
        
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

    private int DistinctLocationsCausingGuardLoop(string[] lines)
    {
        var map = Point.GetMap(lines, c => c);
        //map.SurroundWith(1, '.');

        var startPos = map.Single(pair => pair.Value == '^').Key;
        
        var position = startPos;
        var dir = Point.Origin.Up;
        var seen = new HashSet<Point>();

        while (map.ContainsKey(position))
        {
            seen.Add(position);
            if (map.TryGetValue(position + dir, out var forwardChar) && forwardChar == '#') dir = dir.RotateClockwise(1);
            position += dir;
        }

        var blockingCount = 0;

        foreach (var potentialBlockPosition in seen.Where(p => p != startPos))
        {
            position = startPos;
            dir = Point.Origin.Up;
            var blockedMap = new PointMap<char>(map)
            {
                [potentialBlockPosition] = '#'
            };

            var vectorsInSolution = new HashSet<Vector>();
            while (blockedMap.ContainsKey(position))
            {
                var posDir = new Vector(position, dir);
                if (!vectorsInSolution.Add(posDir))
                {
                    blockingCount++;
                    break;
                }

                while (blockedMap.TryGetValue(position + dir, out var forwardChar) && forwardChar == '#') dir = dir.RotateClockwise(1);
                position += dir;
            }
        }
        
        return blockingCount;
    }
}