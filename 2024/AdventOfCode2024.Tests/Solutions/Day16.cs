using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day16 : ISolution
{
    private const string Example = """
                                   ###############
                                   #.......#....E#
                                   #.#.###.#.###.#
                                   #.....#.#...#.#
                                   #.###.#####.#.#
                                   #.#.#.......#.#
                                   #.#.#####.###.#
                                   #...........#.#
                                   ###.#.#####.#.#
                                   #...#.....#.#.#
                                   #.#.#.###.#.#.#
                                   #.....#...#.#.#
                                   #.###.#.#.#.#.#
                                   #S..#.....#...#
                                   ###############
                                   """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day16");

        var map = new ReindeerMaze(input);
        map.Solve().Should().Be(109516L);
    }

    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day15");

        var map = new ReindeerMaze(input);
        map.Solve().Should().Be(1472235L);
    }

    private class ReindeerMaze(string[] lines)
    {
        private PointMap<char> Map { get; } = Point.GetMap(lines, c => c);
        public long Solve()
        {
            var start = Map.Single(p => p.Value == 'S').Key;
            var end = Map.Single(p => p.Value == 'E').Key;
            var root = new Node<ReindeerPath>(new ReindeerPath(start, Point.Origin.Right), null);
            var solution = root.Dijkstra(0L, node => node.Value.GetMoves(Map),
                node => node.Value.DistFromPrev(node.Previous!.Value), node => node.Value.Position == end,
                (_, _) => false);
            return solution.First().Dist;
        }
    }

    private record ReindeerPath(Point Position, Point Direction)
    {
        public IEnumerable<ReindeerPath> GetMoves(PointMap<char> map)
        {
            if (map[Position + Direction] != '#') yield return this with { Position = Position + Direction };
            if (map[Position + Direction.RotateClockwise(1)] != '#')
                yield return this with { Direction = Direction.RotateClockwise(1) };
            if (map[Position + Direction.RotateClockwise(3)] != '#')
                yield return this with { Direction = Direction.RotateClockwise(3) };
        }

        public long DistFromPrev(ReindeerPath prev)
        {
            return prev.Direction != Direction ? 1000 : 1;
        }
    }
    
    
    private long Solve(string[] input) => throw new NotImplementedException();
}