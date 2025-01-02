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
        var input = Util.ReadRaw(ShortExample);
        //var input = Util.ReadRaw(Example);
        //var input = Util.ReadRaw(SecondExample);
        //var input = Util.ReadFile("day20");

        var r = new RobotArrangement(input, [Directional, Directional, Directional, Numeric]);
        var sum = r.CountScore();
        sum.Should().Be(64 * 379);
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
                var shortestSequence = ShortestSequence(target);
                var numeric = target.PlusMinusLongs().Single();
                score += shortestSequence.Length * numeric;
            }

            return score;
        }

        private string ShortestSequence(string target)
        {
            var moves = target;
            for (var i = levels.Length - 1; i > 0; i--)
            {
                var lowerLevel = levels[i];
                var lowerPointer = new Pointer(lowerLevel.Single(kv => kv.Value == 'A').Key, lowerLevel);
                var newMoves = "";
                foreach (var move in moves)
                {
                    var (upperMoves, newLowerPointer) = lowerPointer.NeededToPress(move, ' ');
                    newMoves += upperMoves;
                    lowerPointer = newLowerPointer;
                }
                moves = newMoves;
            }

            return moves;
        }
    }

    private static readonly PointMap<char> Directional;

    private static readonly PointMap<char> Numeric;

    private record Pointer(Point Position, PointMap<char> KeyPad)
    {
        private readonly Point _void = KeyPad.Single(kv => kv.Value == '.').Key;
        
        public (string moves, Pointer newPointer) NeededToPress(char end, char upperChar)
        {
            var endPoint = KeyPad.Single(kv => kv.Value == end).Key;
            var move = endPoint - Position;
            var verticals = Math.Abs(move.Row);
            var horizontals = Math.Abs(move.Col);
            var verticalPart = string.Concat(Enumerable.Repeat(move.Row > 0 ? 'v' : '^', (int)verticals));
            var horizontalPart = string.Concat(Enumerable.Repeat(move.Col > 0 ? '>' : '<', (int)horizontals));
            var canDoVerticalFirst = Position.Col != _void.Col;
            var shouldDoVerticalFirst = canDoVerticalFirst && false;
            var moves = shouldDoVerticalFirst ? verticalPart + horizontalPart : horizontalPart + verticalPart;
            return (moves + "A", this with { Position = endPoint });
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