using System.Text.RegularExpressions;
using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day13 : ISolution
{
    private const string Example = """
                                   Button A: X+94, Y+34
                                   Button B: X+22, Y+67
                                   Prize: X=8400, Y=5400
                                   
                                   Button A: X+26, Y+66
                                   Button B: X+67, Y+21
                                   Prize: X=12748, Y=12176
                                   
                                   Button A: X+17, Y+86
                                   Button B: X+84, Y+37
                                   Prize: X=7870, Y=6450
                                   
                                   Button A: X+69, Y+23
                                   Button B: X+27, Y+71
                                   Prize: X=18641, Y=10279
                                   """;
    
    private const string SimpleExample = """
                                   Button A: X+1, Y+1
                                   Button B: X+2, Y+2
                                   Prize: X=10, Y=10
                                   """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day13");

        var sum = LowestTokenCost(input);
        sum.Should().Be(31065L);
    }

    [Fact]
    public void Solution2()
    {
        var input = Util.ReadRaw(Example);
        //var input = Util.ReadFile("day13");

        var sum = LowestTokenCostOffset(input);
        sum.Should().Be(904114);
        
        // 898582 too low
    }

    private long LowestTokenCost(string[] input)
    {
        var sum = 0L;
        foreach (var machineInput in input.SplitByDivider(string.IsNullOrWhiteSpace).Select(i => i.ToArray()))
        {
            var machine = Machine.FromInput(machineInput);

            var res = Util.Dijkstra(new MoveSet(Point.Origin, 0, 0),
                point => [point.PressA(machine), point.PressB(machine)],
                (from, to) => to.Position - from.Position == machine.ButtonA ? 3 : 1,
                point => point.Position == machine.PrizeLocation, set => set.IsImpossible(machine)).FirstOrDefault();
            sum += res.Item2;
        }

        return sum;
    }
    
    private long LowestTokenCostOffset(string[] input)
    {
        var sum = 0L;
        var valid = new List<MoveSet>();
        foreach (var machineInput in input.SplitByDivider(string.IsNullOrWhiteSpace).Select(i => i.ToArray()))
        {
            var machine = Machine.FromInput(machineInput, 10000000000000);

            var repeatingMoves = GetRepeatingLinearMoves(machine).Take(5).OrderBy(m => m.Cost() / m.Position.Length()).ToArray();
            
            var linear = new Point(10000000000000, 10000000000000);

            var smallestMiss = machine.PrizeLocation;
            var toCheck = new Queue<Point>();
            var seen = new HashSet<Point>();
            toCheck.Enqueue(Point.Origin);
            var attempt = 0;
            const long limit = 11_000_000;
            while (toCheck.TryDequeue(out var curr) && attempt++ < limit)
            {
                if (!seen.Add(curr)) continue;
                
                var pos = new Point(machine.ButtonA.Row * curr.Row + machine.ButtonB.Row * curr.Col,
                    machine.ButtonA.Col * curr.Row + machine.ButtonB.Col * curr.Col);
                var repeatedPos = pos.ClosestRepetitionFrom(linear);
                var missFromTarget = repeatedPos.pos - machine.PrizeLocation;
                if (missFromTarget is { Row: < 0, Col: < 0 } && missFromTarget.Length() < smallestMiss.Length()) smallestMiss = repeatedPos.pos;
                if (pos.RepeatsIn(linear) || pos.RepeatsIn(machine.PrizeLocation))
                {
                    valid.Add(new MoveSet(pos, (int)curr.Row, (int)curr.Col));
                    break;
                }
                toCheck.Enqueue(curr with { Row = curr.Row + 1 });
                toCheck.Enqueue(curr with { Col = curr.Col + 1 });
            }
        }

        return sum;
    }

    private IEnumerable<MoveSet> GetRepeatingLinearMoves(Machine machine)
    {
        var res = Util.Dijkstra(new MoveSet(Point.Origin, 0, 0),
            point => [point.PressA(machine), point.PressB(machine)],
            (from, to) => to.Position - from.Position == machine.ButtonA ? 3 : 1,
            point => point.Position.Col != 0 && point.Position.Col == point.Position.Row, _ => false);
        foreach (var (moves, dist) in res)
        {
            var finalMove = moves.FirstOrDefault();
            if (finalMove != default) yield return finalMove;
        }
    }
    
    private record Machine(Point ButtonA, Point ButtonB, Point PrizeLocation)
    {
        private static Regex ButtonRegex { get; } = new("X\\+(\\d+)[^Y]*Y\\+(\\d+)");
        private static Regex PrizeRegex { get; } = new("X=(\\d+)[^Y]*Y=(\\d+)");
        
        public static Machine FromInput(string[] input, long prizeOffset = 0)
        {
            var a = ButtonRegex.Match(input[0]);
            var b = ButtonRegex.Match(input[1]);
            var prize = PrizeRegex.Match(input[2]);
            return new Machine(new Point(long.Parse(a.Groups[2].Value), long.Parse(a.Groups[1].Value)),
                new Point(long.Parse(b.Groups[2].Value), long.Parse(b.Groups[1].Value)),
                new Point(long.Parse(prize.Groups[2].Value) + prizeOffset, long.Parse(prize.Groups[1].Value) + prizeOffset));
        }
    }

    private readonly record struct MoveSet(Point Position, int ACount, int BCount)
    {
        public MoveSet PressA(Machine machine) =>
            this with { Position = Position + machine.ButtonA, ACount = ACount + 1 };
        
        public MoveSet PressB(Machine machine) =>
            this with { Position = Position + machine.ButtonB, BCount = BCount + 1 };

        public bool IsImpossible(Machine machine)
        {
            return ACount > 100 || BCount > 100 || Position.Col > machine.PrizeLocation.Col ||
                   Position.Row > machine.PrizeLocation.Row;
        }

        public long Cost() => ACount * 3 + BCount;
    }
}