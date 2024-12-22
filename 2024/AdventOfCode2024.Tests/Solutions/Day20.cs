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
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day20");

        var sum = NumberOfFastCheats(input, 2);
        sum.Should().Be(1459L);
    }

    [Fact(Skip = "Slow")]
    public void Solution2()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day20");

        var sum = NumberOfFastCheats(input, 20);
        sum.Should().Be(1016066L);
    }

    public long NumberOfFastCheats(string[] lines, int longestCheat)
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
            cheats.AddRange(Cheat.FindCheats(point, map, longestCheat, racePathNodes));
        }
        
        //var goodCheats = cheats.Where(c => c.SavedTime > 0).OrderBy(c => c.Time()).GroupBy(c => c.SavedTime).ToArray();

        var goodCheats = cheats.OrderBy(c => c.Time()).Where(c => c.SavedTime >= 100).Count();

        return goodCheats;
    }

    private record Cheat(Point Start, Point End, List<Point> CleanPath)
    {
        public Point[] Path() => [
            ..CleanPath[..(CleanPath.IndexOf(Start) + 1)], ..Start.OrthogonalStepsTo(End),
            ..CleanPath[(CleanPath.IndexOf(End) + 1)..]
        ];

        public long Time() => Path().Length - 1;

        public long SavedTime => CleanPath.Count - 1 - Time();

        public static IEnumerable<Cheat> FindCheats(Node<Point> start, PointMap<char> map, int maxLength, List<Point> racePathNodes)
        {
            Console.WriteLine($"Finding cheats {start}");
            var options = start.Value.ReachableIn(maxLength).Where(p => map.TryGetValue(p, out var val) && val != '#');

            foreach (var option in options)
            {
                var cheat = new Cheat(start.Value, option, racePathNodes);
                if (cheat.SavedTime > 0) yield return cheat;
            }
        }
    }

    private static void Print(PointMap<char> map, Cheat? cheat)
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