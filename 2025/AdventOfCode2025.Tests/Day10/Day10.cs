using System.Text;
using Shouldly;
// ReSharper disable IdentifierTypo
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand

namespace AdventOfCode2025.Tests.Day10;

public class Day10
{
    private const string Example = """
                                   [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
                                   [...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
                                   [.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}
                                   """;
    
    [Fact]
    public void Example_1()
    {
        var lines = Util.ReadRaw(Example);
        var f = new Factory(lines);
        f.PressesNeededForAll().ShouldBe(7);
    }
    
    [Fact]
    public void Real_1()
    {
        var lines = Util.ReadFile("day10");
        var f = new Factory(lines);
        f.PressesNeededForAll().ShouldBe(375);
    }
    
    [Fact]
    public void Example_2()
    {
        var lines = Util.ReadRaw(Example);
    }
    
    [Fact]
    public void Real_2()
    {
        var lines = Util.ReadFile("day10");
    }
}

public class Factory(IReadOnlyList<string> input)
{
    private IReadOnlyList<Machine> Machines { get; } = input.Select(Machine.FromLine).ToArray();

    public int PressesNeededForAll()
    {
        var sum = 0;
        foreach (var machine in Machines)
        {
            sum += machine.Solve();
        }

        return sum;
    }
}

public class Machine(long GoalState, int[] Operations, int[] JoltageRequirement)
{
    public static Machine FromLine(string l)
    {
        var goalState = 0L;
        foreach (var c in l.Skip(1).TakeWhile(c => c != ']').Reverse())
        {
            goalState <<= 1;
            if (c == '#') goalState++;
        }

        var ops = l.Split(' ')[1..^1].Select(s => s.Trim('(', ')').Split(',').Select(int.Parse).Aggregate(0,
            (op, i) =>
            {
                op |= 1 << i;
                return op;
            })).ToArray();

        return new Machine(goalState, ops, []);
    }

    public static void PrintState(long state, ushort l = 10)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < l; i++)
        {
            sb.Append((state & 1) != 0 ? '#' : '.');
            state >>= 1;
        }

        Console.WriteLine(sb.ToString());
    }

    public int Solve()
    {
        var queue = new Queue<(long state, long opsDone)>();
        queue.Enqueue((0, 0));
        var seenOpsDone = new HashSet<long>([0]);
        while (queue.TryDequeue(out var s))
        {
            var state = s.state;
            var opsDone = s.opsDone;
            Console.WriteLine("New op");
            PrintState(opsDone);
            PrintState(state);
            if (state == GoalState) return Operations.Length - GetRemainingOpIndexes(opsDone).Count();
            foreach (var remainingOpIndex in GetRemainingOpIndexes(opsDone))
            {
                var nextOpDone = opsDone | (1 << remainingOpIndex);
                if (seenOpsDone.Add(nextOpDone))
                {
                    queue.Enqueue((state ^ Operations[remainingOpIndex], nextOpDone));
                }
            }
        }

        throw new Exception("Failed to solve");
    }

    private IEnumerable<int> GetRemainingOpIndexes(long opsDone)
    {
        for (var i = 0; i < Operations.Length; i++)
        {
            if ((opsDone & (1 << i)) == 0) yield return i;
        }
    }
}
