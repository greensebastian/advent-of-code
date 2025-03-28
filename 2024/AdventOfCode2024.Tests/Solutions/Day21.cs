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

        var r = new RobotArrangement(input, 3);
        var sum = r.CountOptimalScore();
        sum.Should().Be(177814);
        //sum.Should().Be(126384);
        // 184390L too high
        // 184058L too high
        // 184058L
    }

    [Fact]
    public void Solution2()
    {
        // I probably need to figure out "what does character X cost at Y levels above"
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day21");
        
        var r = new RobotArrangement(input, 25);
        var sum = r.CountOptimalScore();
        sum.Should().Be(126384L);
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

    private class RobotArrangement(string[] input, int directionalLevels)
    {
        private string[] Targets { get; } = input;
        
        private static readonly Dictionary<(char Start, char End), string> OptimalMovesDirectional = OptimalParentMoves(Directional);
        private static readonly Dictionary<(char Start, char End), List<string>> PossibleMovesNumeric = PossibleNumericParentMoves();


        public long CountOptimalScore()
        {
            var score = 0L;
            foreach (var target in Targets)
            {
                var shortestSequence = ShortestSequenceOptimal(target);
                var numeric = target.PlusMinusLongs().Single();
                score += shortestSequence * numeric;
            }

            return score;
        }

        private long ShortestSequenceOptimal(string target)
        {
            var totalMoves = "";
            var pos = 'A';
            foreach (var next in target)
            {
                var possibleMoves = PossibleMovesNumeric[(pos, next)].Select(p => ParentMoves(p, directionalLevels - 1)).ToList();
                totalMoves += possibleMoves.MinBy(p => p.Length);
                pos = next;
            }

            return totalMoves.Length;
        }

        private string ParentMoves(string moves, int levelsRemaining)
        {
            if (levelsRemaining == 0) return moves;
            var movesNeeded = "";
            var pos = 'A';
            foreach (var move in moves)
            {
                movesNeeded += OptimalMovesDirectional[(pos, move)];
                pos = move;
            }
            return ParentMoves(movesNeeded, levelsRemaining - 1);
        }
    }

    private static readonly PointMap<char> Directional;

    private static readonly PointMap<char> Numeric;

    private static Dictionary<(char Start, char End), List<string>> PossibleNumericParentMoves()
    {
        var moves = new Dictionary<(char Start, char End), List<string>>();
        
        foreach (var possibleMove in Numeric.Combinations())
        {
            var start = possibleMove[0];
            var end = possibleMove[1];
            if (start.Value == end.Value)
            {
                moves[(start.Value, end.Value)] = ["A"];
            }
            if (possibleMove.Any(m => m.Value == '.')) continue;

            var working = new List<string>();
            var possiblePaths = new Queue<string>();
            possiblePaths.Enqueue("");
            while (possiblePaths.TryDequeue(out var path))
            {
                if (path.Length > 3) continue;
                var pos = start.Key;
                var failed = false;
                foreach (var dir in path)
                {
                    var move = dir switch
                    {
                        '>' => Point.Origin.Right,
                        'v' => Point.Origin.Down,
                        '<' => Point.Origin.Left,
                        '^' => Point.Origin.Up,
                        _ => throw new Exception("Not possible")
                    };
                    pos += move;
                    if (!Numeric.ContainsKey(pos) || Numeric[pos] == '.') failed = true;
                }
                if (failed) continue;
                if (pos == end.Key)
                {
                    working.Add(path + "A");
                }
                else
                {
                    possiblePaths.Enqueue(path + ">");
                    possiblePaths.Enqueue(path + "v");
                    possiblePaths.Enqueue(path + "<");
                    possiblePaths.Enqueue(path + "^");
                }
            }

            moves[(start.Value, end.Value)] = working;
        }

        return moves;
    }
    
    /// <summary>
    /// From any point S on the child map, to get to E, the moves M are needed.
    /// For each move M, there is an optimal path P the parent can take from 'A' to get there, and press 'A' again.
    /// This method finds the optimal P for all possible S and E
    /// </summary>
    private static Dictionary<(char Start, char End), string> OptimalParentMoves(PointMap<char> childMap)
    {
        var moves = new Dictionary<(char Start, char End), string>();
        
        foreach (var possibleMove in childMap.Combinations())
        {
            var start = possibleMove[0];
            var end = possibleMove[1];
            if (start.Value == end.Value)
            {
                moves[(start.Value, end.Value)] = "A";
            }
            if (possibleMove.Any(m => m.Value == '.')) continue;

            var parentMoves = ParentPressesNeeded(end.Value.ToString(), new Robot(start.Key, childMap));

            moves[(start.Value, end.Value)] = parentMoves;
        }

        return moves;
    }

    private static string ParentPressesNeeded(string childCharsToPress, Robot childStart)
    {
        var totalMoves = "";
        var child = childStart;
        foreach (var nextChildChar in childCharsToPress)
        {
            var childOptionOne = child.NeededToPress(nextChildChar, true);
            var childOptionTwo = child.NeededToPress(nextChildChar, false);
            var childMovesNeeded = childOptionOne.moves.Length <= childOptionTwo.moves.Length
                ? childOptionOne
                : childOptionTwo;
            child = childMovesNeeded.newPointer;
            totalMoves += childMovesNeeded.moves;
        }

        Console.WriteLine($"{childStart.Char()} making moves {childCharsToPress} requires {totalMoves}");
        return totalMoves;
    }

    private record Robot(Point Position, PointMap<char> KeyPad)
    {
        public override string ToString() => $"{Position}({KeyPad[Position]} in " + (KeyPad == Directional ? "Directional": "Numerical") + ")";

        private readonly Point _void = KeyPad.Single(kv => kv.Value == '.').Key;
        
        public (string moves, Robot newPointer) NeededToPress(char end, bool prioritizeVertical)
        {
            if (KeyPad[Position] == end) return ("A", this);
            var endPoint = KeyPad.Single(kv => kv.Value == end).Key;
            var move = endPoint - Position;
            var verticals = Math.Abs(move.Row);
            var horizontals = Math.Abs(move.Col);
            var verticalPart = string.Concat(Enumerable.Repeat(move.Row > 0 ? 'v' : '^', (int)verticals));
            var horizontalPart = string.Concat(Enumerable.Repeat(move.Col > 0 ? '>' : '<', (int)horizontals));
            // starting in void row and ending in void column
            var hasToDoVerticalFirst = Position.Row == _void.Row && endPoint.Col == _void.Col;
            // starting in void column and ending in void row
            var hasToDoHorizontalFirst = Position.Col == _void.Col && endPoint.Row == _void.Row;
            var shouldDoVerticalFirst = hasToDoVerticalFirst ? true : hasToDoHorizontalFirst ? false : prioritizeVertical;
            var moves = (shouldDoVerticalFirst ? verticalPart + horizontalPart : horizontalPart + verticalPart) + "A";
            //Console.WriteLine(moves);
            return (moves, this with { Position = endPoint });
        }

        public char Char() => KeyPad[Position];
    }

    static Day21()
    {
        Directional = Point.GetMap([".^A", "<v>"], c => c);
        Numeric = Point.GetMap(["789", "456", "123", ".0A"], c => c);
    }
}