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
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day15");

        var map = new DoubleWideLanternFishMap(input);
        map.DoActions();
        map.GpsSum().Should().Be(1472235L);
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
    
    private class DoubleWideLanternFishMap
    {
        private PointMap<char> _map;
        private Point[] _actions;

        public DoubleWideLanternFishMap(string[] lines)
        {
            var parsed = lines.SplitByDivider(string.IsNullOrWhiteSpace).ToArray();
            _map = Point.GetMap(
                parsed[0]
                    .Select(l => l
                        .Replace("#", "##")
                        .Replace("O", "[]")
                        .Replace(".", "..")
                        .Replace("@", "@."))
                    .ToArray(), c => c);
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
            return _map.Where(kv => kv.Value == '[').Sum(kv => kv.Key.Row * 100 + kv.Key.Col);
        }

        public void DoActions()
        {
            var c = 0L;

            foreach (var action in _actions)
            {
                c++;
                var robotPos = _map.Single(kv => kv.Value == '@').Key;
                Move(robotPos, action);
            }
        }

        private void Move(Point pos, Point dir)
        {
            if (dir.Row == 0)
            {
                var node = new Node<Point>(pos, null);
                while (_map[node.Value] is not '.' and not '#')
                {
                    node = node.ConcatWith(node.Value + dir);
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
            else
            {
                var toMove = new List<Point>();
                toMove.Add(pos);
                var keepMoving = true;
                while (keepMoving)
                {
                    var allDots = true;
                    foreach (var currentLinePoint in toMove.ToArray())
                    {
                        var next = currentLinePoint + dir;
                        if (toMove.Contains(next)) continue;
                        if (_map[next] == '#') keepMoving = false;
                        if (_map[next] != '.') allDots = false;
                        if (_map[next] == '[')
                        {
                            toMove.Add(next);
                            toMove.Add(next.Right);
                        }
                        if (_map[next] == ']')
                        {
                            toMove.Add(next);
                            toMove.Add(next.Left);
                        }
                    }

                    if (allDots) keepMoving = false;
                }

                if (toMove.All(p => _map[p + dir] != '#'))
                {
                    foreach (var pointToMove in toMove.Distinct().Reverse())
                    {
                        _map[pointToMove + dir] = _map[pointToMove];
                        if (!toMove.Contains(pointToMove - dir)) _map[pointToMove] = '.';
                    }
                }
            }
        }
    }

    private long Solve(string[] input) => throw new NotImplementedException();
}