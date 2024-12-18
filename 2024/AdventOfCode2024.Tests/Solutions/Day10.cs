using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day10 : ISolution
{
    private const string Example = """
                                   89010123
                                   78121874
                                   87430965
                                   96549874
                                   45678903
                                   32019012
                                   01329801
                                   10456732
                                   """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day10");
        
        var answer = TrailheadScoreSum(input);
        answer.Should().Be(737);
    }

    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day10");

        var answer = TrailheadRatingSum(input);
        answer.Should().Be(1619);
    }

    private int TrailheadScoreSum(string[] input)
    {
        var map = Point.GetMap(input, c => int.Parse(c.ToString()));

        var validPaths = map.Where(kv => kv.Value == 9).SelectMany(kv => PathsToTrailhead(kv.Key, map)).Select(p => p.ToArray()).ToArray();

        var trailheads = validPaths.GroupBy(path => path.Last()).OrderBy(group => group.Key.Row * 100000 + group.Key.Col).ToArray();

        var trailheadScores = trailheads.Select(th => th.Select(trail => trail.First()).Distinct().Count()).ToArray();
        
        return trailheadScores.Sum();
    }
    
    private int TrailheadRatingSum(string[] input)
    {
        var map = Point.GetMap(input, c => int.Parse(c.ToString()));

        var validPaths = map.Where(kv => kv.Value == 9).SelectMany(kv => PathsToTrailhead(kv.Key, map)).Select(p => p.ToArray()).ToArray();

        var trailheads = validPaths.GroupBy(path => path.Last()).OrderBy(group => group.Key.Row * 100000 + group.Key.Col).ToArray();

        var trailheadRatings = trailheads.Select(th => th.Count()).ToArray();
        
        return trailheadRatings.Sum();
    }

    private IEnumerable<IEnumerable<Point>> PathsToTrailhead(Point pos, PointMap<int> map)
    {
        var height = map[pos];
        if (height == 0) yield return [pos];
        foreach (var neighbour in pos.ClockwiseOrthogonalNeighbours())
        {
            var isDirectlyBelow = map.TryGetValue(neighbour, out var neighbourHeight) && neighbourHeight == height - 1;
            if (isDirectlyBelow)
            {
                foreach (var neighbourPath in PathsToTrailhead(neighbour, map))
                {
                    yield return neighbourPath.Prepend(pos);
                }
            }
        }
    }

    private void Print(IEnumerable<Point> path, PointMap<int> map)
    {
        Console.WriteLine(map.ToString(p => path.Contains(p) ? map[p].ToString() : "."));
    }
}