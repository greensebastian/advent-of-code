using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day15 : ISolution
{
    private const string Example = """
                                   ##########
                                   #..O..O.O#
                                   #......O.#
                                   #.OO..O.O#
                                   #..O@..O.#
                                   #O#..O...#
                                   #O..O..O.#
                                   #.OO.O.OO#
                                   #....O...#
                                   ##########
                                   
                                   <vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
                                   vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
                                   ><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
                                   <<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
                                   ^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
                                   ^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
                                   >^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
                                   <><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
                                   ^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
                                   v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^
                                   """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day15");

        var map = new LanternFishMap(input);
        map.DoActions();
        map.GpsSum().Should().Be(1448589L);
    }

    [Fact]
    public void Solution2()
    {
        var input = Util.ReadRaw(Example);
        //var input = Util.ReadFile("day13");

        var sum = Solve(input);
        sum.Should().Be(8270);
    }

    private class LanternFishMap
    {
        private PointMap<char> _map;
        private Point[] _actions;

        public LanternFishMap(string[] lines)
        {
            var parsed = lines.SplitByDivider(string.IsNullOrWhiteSpace).ToArray();
            _map = Point.GetMap(parsed[0].ToArray(), c => c);
            _actions = parsed[1].SelectMany(s => s.Select(c => c switch
            {
                '^' => Point.Origin.Up,
                '>' => Point.Origin.Right,
                'v' => Point.Origin.Down,
                '<' => Point.Origin.Left,
                _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
            })).ToArray();
        }

        public long GpsSum()
        {
            return _map.Where(kv => kv.Value == 'O').Sum(kv => kv.Key.Row * 100 + kv.Key.Col);
        }

        public void DoActions()
        {
            var c = 0L;

            foreach (var action in _actions)
            {
                c++;
                var robotPos = _map.Single(kv => kv.Value == '@').Key;
                var node = new Node<Point>(robotPos, null);
                while (_map[node.Value] is not '.' and not '#')
                {
                    node = node.ConcatWith(node.Value + action);
                }

                var moved = false;
                if (_map[node.Value] == '.')
                {
                    moved = true;
                    while (node.Previous != null)
                    {
                        _map[node.Value] = _map[node.Previous.Value];
                        node = node.Previous;
                    }
                }

                if (moved) _map[node.Value] = '.';
            }
        }
    }

    private long Solve(string[] input) => throw new NotImplementedException();
}