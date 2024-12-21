using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day20 : ISolution
{
    private const string Example = """
                                   ###############
                                   #...#...#.....#
                                   #.#.#.#.#.###.#
                                   #S#...#.#.#...#
                                   #######.#.#.###
                                   #######.#.#...#
                                   #######.#.###.#
                                   ###..E#...#...#
                                   ###.#######.###
                                   #...###...#...#
                                   #.#####.#.###.#
                                   #.#...#.#.#...#
                                   #.#.#.#.#.#.###
                                   #...#...#...###
                                   ###############
                                   """;
    
    private const string SimpleExample = """
                                   #####
                                   #S#E#
                                   #.#.#
                                   #...#
                                   #####
                                   """;
    
    [Fact]
    public void Solution1()
    {
        var input = Util.ReadRaw(Example);
        //var input = Util.ReadFile("day20");

        var sum = NumberOfFastCheats(input);
        sum.Should().Be(31065L);
        // 6643L too high
        // 6643L
    }

    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day13");

        var sum = NumberOfFastCheats(input);
        sum.Should().Be(53201610784591);
        
        // 898582 too low
        // 58655757198402L too low
        // 64252052785279L too low
        // 75777525502554L
        // 78228660089107L
        // 82663975580247L
        // 84191045080623
    }

    public long NumberOfFastCheats(string[] lines)
    {
        var map = Point.GetMap(lines, c => c);
        var startPoint = map.Single(kv => kv.Value == 'S').Key;
        var endPoint = map.Single(kv => kv.Value == 'E').Key;
        var start = new Node<Point>(startPoint, null);
        var latestMove = start;
        while (latestMove.Value != endPoint)
        {
            var next = latestMove.Value.ClockwiseOrthogonalNeighbours().Single(n => map[n] != '#' && n != latestMove.Previous?.Value);
            latestMove = latestMove.ConcatWith(next);
        }

        var racePath = latestMove.Enumerate().Reverse().ToList();
        var racePathNodes = racePath.Select(n => n.Value).ToList();

        var cheats = new List<Cheat>();
        foreach (var point in racePath.SkipLast(2))
        {
            var cheatOptions = point.Dijkstra(0,
                node => node.Value.ClockwiseOrthogonalNeighbours().Where(n => !node.Enumerate().Any(o => o.Value == n)),
                _ => 1, node => map.TryGetValue(node.Value, out var c) && c != '#', (_, l) => l > 3);

            var cheatedPaths = new List<Node<Point>>();
            foreach (var (endNode, dist) in cheatOptions)
            {
                var skipped = racePathNodes.Count - 1 + racePathNodes.IndexOf(point.Value) - racePathNodes.IndexOf(endNode.)
            }
        }
        
        var goodCheats = cheats.Where(c => (racePath.Count - 1) - c.Time() > 0).OrderByDescending(c => c.Time()).ToArray();

        //var goodCheats = cheats.Distinct().Count(c => (racePath.Count - 1) - c.Time() >= 100);

        return goodCheats.Count();
    }

    private record Cheat(Point Start, Point End, List<Point> CleanPath)
    {
        public Point[] Path => [
            ..CleanPath[..(CleanPath.IndexOf(Start) + 1)], ..Start.OrthogonalStepsTo(End),
            ..CleanPath[(CleanPath.IndexOf(End) + 1)..]
        ];

        public long Time() => Path.Length - 1;

        public IEnumerable<Cheat> GetCheats(PointMap<char> map, List<Point> originalPath)
        {
            foreach (var startPoint in originalPath)
            {
                var startMap = new PointMap<char>(new []{ new KeyValuePair<Point, char>(startPoint, '.') });
                startMap.SurroundWith(3, cheatDestination => map.GetValueOrDefault(cheatDestination, '#'));
                var toLookAt = startMap
                    .Where(kv => kv.Value != '#')
                    .OrderBy(kv => (kv.Key - startPoint).Length())
                    .ToList();
                
                while (toLookAt.Count > 0)
                {
                    if (kv.Value != '#' && originalPath.IndexOf(kv.Key) > originalPath.IndexOf(startPoint))
                    {
                        
                    }
                }
            }
        }
    }
}