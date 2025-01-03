using FluentAssertions;
// ReSharper disable InvalidXmlDocComment

namespace AdventOfCode2024.Tests.Solutions;

public class Day21 : ISolution
{
    private const string Example = """
                                   029A
                                   980A
                                   179A
                                   456A
                                   379A
                                   """;
    
    private const string ShortExample = """
                                        179A
                                        """;
    
    private const string SecondExample = """
                                         379A
                                         """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(ShortExample);
        //var input = Util.ReadRaw(Example);
        //var input = Util.ReadRaw(SecondExample);
        var input = Util.ReadFile("day21");

        var r = new RobotArrangement(input, [Directional, Directional, Directional, Numeric]);
        var sum = r.CountScore();
        sum.Should().Be(177814);
        
        // 184390L too high
        // 184058L too high
        // 184058L
    }

    [Fact]
    public void Solution2()
    {
        var input = Util.ReadRaw(Example);
        //var input = Util.ReadFile("day20");

        var r = new RobotArrangement(input, [Directional, Directional, Directional, Numeric]);
        var sum = r.CountScore();
        sum.Should().Be(1459L);
    }

    /**
     * _ ^ A
     * < v >
     *
     * 7 8 9
     * 4 5 6
     * 1 2 3
     * _ 0 A
     */

    private class RobotArrangement(string[] input, PointMap<char>[] levels)
    {
        private string[] Targets { get; } = input;

        public long CountScore()
        {
            var score = 0L;
            foreach (var target in Targets)
            {
                var shortestSequence = ShortestSequenceDijkstra(target);
                var numeric = target.PlusMinusLongs().Single();
                Console.WriteLine($"{target}: {shortestSequence.Length} * {numeric}");
                score += shortestSequence.Length * numeric;
            }

            return score;
        }

        private record MovePointer(string Moves, Pointer Pointer);

        private record DijkstraState(int Level, string Moves, string Remainder, Pointer LowerLevelPointer)
        {
            public override string ToString() => $"{Level}, [{Moves} - {Remainder}]";
        }
        
        private string ShortestSequenceDijkstra(string target)
        {
            var best = new DijkstraState(levels.Length - 1, "", target,
                new Pointer(levels.Last().Single(kv => kv.Value == 'A').Key, levels.Last()));
            var root = new Node<DijkstraState>(best, null);
            var responses = root.Dijkstra(0, node =>
            {
                if (node.Value.Remainder == "")
                {
                    var nextLevelIndex = node.Value.Level - 1;
                    return
                    [
                        new DijkstraState(nextLevelIndex, "", node.Value.Moves,
                            new Pointer(levels[nextLevelIndex].Single(kv => kv.Value == 'A').Key,
                                levels[nextLevelIndex]))
                    ];
                }

                var primary = node.Value.LowerLevelPointer.NeededToPress(node.Value.Remainder[0], true);
                var alternate = node.Value.LowerLevelPointer.NeededToPress(node.Value.Remainder[0], false);
                return
                [
                    node.Value with
                    {
                        Moves = node.Value.Moves + primary.moves,
                        Remainder = node.Value.Remainder[1..],
                        LowerLevelPointer = primary.newPointer
                    },
                    node.Value with
                    {
                        Moves = node.Value.Moves + alternate.moves,
                        Remainder = node.Value.Remainder[1..],
                        LowerLevelPointer = alternate.newPointer
                    }
                ];
            }, node =>
            {
                if (node.Value.Moves.Length == 0) return 0;
                return node.Value.Moves.Length - node.Previous!.Value.Moves.Length;
            }, node => node.Value is { Level: 1, Remainder.Length: 0 }, (node, dist) =>
            {
                if (node.Value.Level != 0) return false;

                if (best.Level > 0)
                {
                    best = node.Value;
                    return false;
                }

                var remainderDiff = best.Remainder.Length - node.Value.Remainder.Length;
                var movesDiff = node.Value.Moves.Length - best.Moves.Length;
                var botInSameSpot = node.Value.LowerLevelPointer.Position == best.LowerLevelPointer.Position;
                if (remainderDiff > 0)
                {
                    best = node.Value;
                    return false;
                }

                if (movesDiff == 0 && remainderDiff == 0 && botInSameSpot) return true;
                if (node.Value.Moves.Length > 0 && best.Moves.Length > 0 && movesDiff > 0) return true;
                return false;
            });

            return responses.First().EndNode.Value.Moves;
        }

        private string ShortestSequence(string target)
        {
            var moveOptions = new List<MovePointer> {new(target, new Pointer(levels.Last().Single(kv => kv.Value == 'A').Key, levels.Last()))};
            for (var i = levels.Length - 1; i > 0; i--)
            {
                var newMoveOptions = new List<MovePointer>();
                foreach (var moves in moveOptions)
                {
                    var lowerLevel = levels[i];
                    var lowerPointer = moves.Pointer with { KeyPad = lowerLevel };
                    var newMoves = "";
                    foreach (var move in moves.Moves)
                    {
                        var primary = lowerPointer.NeededToPress(move, true);
                        var alternate = lowerPointer.NeededToPress(move, false);
                        var (upperMoves, newLowerPointer) =
                            alternate.moves.Length < primary.moves.Length ? alternate : primary;
                        newMoves += upperMoves;
                        lowerPointer = newLowerPointer;
                    }
                    Console.WriteLine($"Level {i}: {moves} => {newMoves}");
                    newMoveOptions.Add(new MovePointer(newMoves, lowerPointer));
                }

                moveOptions = newMoveOptions;
            }

            return moveOptions.MinBy(s => s.Moves.Length)!.Moves;
        }
    }

    private static readonly PointMap<char> Directional;

    private static readonly PointMap<char> Numeric;

    private record Pointer(Point Position, PointMap<char> KeyPad)
    {
        private readonly Point _void = KeyPad.Single(kv => kv.Value == '.').Key;
        
        public (string moves, Pointer newPointer) NeededToPress(char end, bool prioritizeVertical)
        {
            var endPoint = KeyPad.Single(kv => kv.Value == end).Key;
            var move = endPoint - Position;
            var verticals = Math.Abs(move.Row);
            var horizontals = Math.Abs(move.Col);
            var verticalPart = string.Concat(Enumerable.Repeat(move.Row > 0 ? 'v' : '^', (int)verticals));
            var horizontalPart = string.Concat(Enumerable.Repeat(move.Col > 0 ? '>' : '<', (int)horizontals));
            var hasToDoVerticalFirst = (endPoint.Col == _void.Col && Position.Row == _void.Row) || (endPoint.Row == _void.Row && Position.Col == _void.Col);
            var hasToDoHorizontalFirst = (endPoint.Row == _void.Row && Position.Col == _void.Col) || (endPoint.Col == _void.Col && Position.Row == _void.Row);
            var shouldDoVerticalFirst = hasToDoVerticalFirst ? true : hasToDoHorizontalFirst ? false : prioritizeVertical;
            var moves = (shouldDoVerticalFirst ? verticalPart + horizontalPart : horizontalPart + verticalPart) + "A";
            //Console.WriteLine(moves);
            return (moves, this with { Position = endPoint });
        }

        private bool WillHitEmpty(Point start, Point end)
        {
            if (start.Col > 0 && end.Col > 0) return false;
            return (start.Col == _void.Col && end.Row == _void.Row) || (start.Row == _void.Row && end.Col == _void.Col);
        }
    }

    static Day21()
    {
        Directional = Point.GetMap([".^A", "<v>"], c => c);
        Numeric = Point.GetMap(["789", "456", "123", ".0A"], c => c);
    }
}