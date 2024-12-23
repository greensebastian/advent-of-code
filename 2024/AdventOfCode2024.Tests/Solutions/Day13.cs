using System.Text.RegularExpressions;
using FluentAssertions;
using MathNet.Numerics.LinearAlgebra;

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
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day13");

        var sum = LowestTokenCostOffsetLinAlg(input);
        sum.Should().Be(53201610784591);
        
        // 898582 too low
        // 58655757198402L too low
        // 64252052785279L too low
        // 75777525502554L
        // 78228660089107L
        // 82663975580247L
        // 84191045080623
        // 80819862732327L
        // 80819862732327
        // 80819862732327
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
    
    private long LowestTokenCostOffsetLinAlg(string[] input)
    {
        var sum = 0L;
        var counter = 0;
        foreach (var machineInput in input.SplitByDivider(string.IsNullOrWhiteSpace).Select(i => i.ToArray()))
        {
            Console.WriteLine($"On machine {counter++}");
            var machine = Machine.FromInput(machineInput, 10000000000000);
            // Row = A * ARow + B * BRow = TargetRow
            // Col = A * ACol + B * BCol = TargetCol
            
            var factors = Matrix<double>.Build.DenseOfArray(new double[,] {
                { machine.ButtonA.Row, machine.ButtonB.Row },
                { machine.ButtonA.Col, machine.ButtonB.Col }});
            
            var target = Vector<double>.Build.Dense([machine.PrizeLocation.Row, machine.PrizeLocation.Col]);

            var x = factors.Solve(target);

            var cost = (long?)(x?[0] * 3 + x?[1]);
            var valid = cost != null && Math.Abs((long)x?[0]! - x[0]) < 0.01 && Math.Abs((long)x?[1]! - x[1]) < 0.01;

            if (valid) sum += cost ?? 0;
        }

        return sum;
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

        public MoveSet GetLinearRepeating(Machine machine)
        {
            var t = Util.LowestCommonMultiple(machine.ButtonA.Row, machine.ButtonA.Col, machine.ButtonB.Row,
                machine.ButtonB.Col);

            return new MoveSet(Point.Origin, t, t);
        }
    }

    private readonly record struct MoveSet(Point Position, long ACount, long BCount)
    {
        public MoveSet PressA(Machine machine) =>
            this with { Position = Position + machine.ButtonA, ACount = ACount + 1 };
        
        public MoveSet PressABackwards(Machine machine) =>
            this with { Position = Position - machine.ButtonA, ACount = ACount + 1 };
        
        public MoveSet PressB(Machine machine) =>
            this with { Position = Position + machine.ButtonB, BCount = BCount + 1 };
        
        public MoveSet PressBBackwards(Machine machine) =>
            this with { Position = Position - machine.ButtonB, BCount = BCount + 1 };

        public bool IsImpossible(Machine machine)
        {
            return ACount > 100 || BCount > 100 || Position.Col > machine.PrizeLocation.Col ||
                   Position.Row > machine.PrizeLocation.Row;
        }

        public long Cost() => ACount * 3 + BCount;

        public MoveSet AlignWith(Machine machine, Point target)
        {
            var targetIncline = target.Incline();
            var currentIncline = Position.Incline();
            var movesByIncline = new[] { machine.ButtonA, machine.ButtonB }.OrderBy(p => p.Incline()).ToArray();
            var move = currentIncline > targetIncline ? movesByIncline[0] : movesByIncline[1];
            var moveWasA = move == machine.ButtonA;
            return new MoveSet(Position + move, ACount + (moveWasA ? 1 : 0), BCount + (moveWasA ? 0 : 1));
        }
    }
}