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
                _ => 1, node => map.TryGetValue(node.Value, out var c) && c != '#', (_, l) => l > 2);

            foreach (var (endNode, _) in cheatOptions)
            {
                if (endNode.Enumerate().TakeWhile(n => n.Value != point.Value).Count(p => map[p.Value] != '#') == 1)
                {
                    cheats.Add(new Cheat(point.Value, endNode.Value, racePathNodes));
                }
            }
        }
        
        var goodCheats = cheats.Where(c => racePath.Count - 1 - c.Time() > 0).OrderBy(c => c.Time()).GroupBy(c => c.SavedTime).ToArray();

        //var goodCheats = cheats.Distinct().Count(c => (racePath.Count - 1) - c.Time() >= 100);

        return goodCheats.Count();
    }

    private record Cheat(Point Start, Point End, List<Point> CleanPath)
    {
        public Point[] Path() => [
            ..CleanPath[..(CleanPath.IndexOf(Start) + 1)], ..Start.OrthogonalStepsTo(End),
            ..CleanPath[(CleanPath.IndexOf(End) + 1)..]
        ];

        public long Time() => Path().Length - 1;

        public long SavedTime => CleanPath.Count - 1 - Time();
    }

    private void Print(PointMap<char> map, Cheat? cheat)
    {
        Console.WriteLine(map.ToString(p =>
        {
            if (cheat is null) return map[p].ToString();
            if (map[p] == '#')
            {
                if (cheat.Path().Contains(p)) return "+";
            }
            return map[p].ToString();
        }));
    }
}