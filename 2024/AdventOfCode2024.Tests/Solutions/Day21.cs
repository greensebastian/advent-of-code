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
        var sum = r.GetMoveSum();
        sum.Should().Be(177814);
    }

    [Fact]
    public void Solution2()
    {
        // I probably need to figure out "what does character X cost at Y levels above"
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day21");
        
        var r = new RobotArrangement(input, 26);
        var sum = r.GetMoveSum();
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
        public long GetMoveSum()
        {
            var sum = 0L;
            foreach (var code in input)
            {
                var pressesNeeded = 0L;
                var pos = 'A';
                foreach (var target in code)
                {
                    var state = new Action(directionalLevels, pos, target);
                    pressesNeeded += PressesNeededForChildAction(state);
                    pos = target;
                }

                var codeNumber = int.Parse(code.Replace("A", ""));
                sum += pressesNeeded * codeNumber;
            }

            return sum;
        }
        
        private long PressesNeededForChildAction(Action action)
        {
            if (action.DirectionalRobotsAbove == 0) return 1;
            if (Cache.TryGetValue(action, out var cached)) return cached;

            var childMap = Numeric.ContainsValue(action.CurrentChar) && Numeric.ContainsValue(action.TargetChar)
                ? Numeric
                : Directional;

            // What input does the child need to make the press
            var childInputOptions = PressOptions(childMap, action.CurrentChar, action.TargetChar).ToArray();
            var best = long.MaxValue;
            foreach (var childInputOption in childInputOptions)
            {
                // What input does this robot need to perform child input
                var thisPos = 'A';
                var thisInputNeeded = 0L;
                foreach (var childInput in childInputOption)
                {
                    // Move to input and press
                    thisInputNeeded += PressesNeededForChildAction(new Action(action.DirectionalRobotsAbove - 1, thisPos, childInput));
                    thisPos = childInput;
                }

                if (thisInputNeeded < best) best = thisInputNeeded;
            }

            Cache[action] = best;
            return best;
        }

        private IEnumerable<string> PressOptions(PointMap<char> map, char startChar, char endChar)
        {
            var start = map.Single(p => p.Value == startChar).Key;
            var end = map.Single(p => p.Value == endChar).Key;
            var diff = end - start;
            var dCol = (diff.Col < 0 ? '<' : '>').Repeat((int)Math.Abs(diff.Col));
            var dRow = (diff.Row < 0 ? '^' : 'v').Repeat((int)Math.Abs(diff.Row));

            var startsInInvalidCol = start.Col == Invalid(map).Col;
            var endsInInvalidCol = end.Col == Invalid(map).Col;
            var startsInInvalidRow = start.Row == Invalid(map).Row;
            var endsInInvalidRow = end.Row == Invalid(map).Row;

            var canNotStartVertical = startsInInvalidCol && endsInInvalidRow;
            var canNotStartHorizontal = startsInInvalidRow && endsInInvalidCol;

            var vertFirst = dRow + dCol + "A";
            var colFirst = dCol + dRow + "A";
            if (!canNotStartVertical) yield return vertFirst;
            if (!canNotStartHorizontal && colFirst != vertFirst) yield return dCol + dRow + "A";
        }

        private Dictionary<Action, long> Cache { get; } = new();
        private record struct Action(int DirectionalRobotsAbove, char CurrentChar, char TargetChar);
    }

    private static readonly PointMap<char> Directional;
    private static Point Invalid(PointMap<char> map) => map.Single(p => p.Value == '.').Key;

    private static readonly PointMap<char> Numeric;

    static Day21()
    {
        Directional = Point.GetMap([".^A", "<v>"], c => c);
        Numeric = Point.GetMap(["789", "456", "123", ".0A"], c => c);
    }
}