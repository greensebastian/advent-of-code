using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day07 : ISolution
{
    private const string Example = """
                                   190: 10 19
                                   3267: 81 40 27
                                   83: 17 5
                                   156: 15 6
                                   7290: 6 8 6 15
                                   161011: 16 10 13
                                   192: 17 8 14
                                   21037: 9 7 18 13
                                   292: 11 6 16 20
                                   """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day07");

        var answer = SumOfValidTestOperators(input);
        answer.Should().Be(1289579105366L);
    }

    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day06");

        var answer = SumOfValidTestOperators(input);
        answer.Should().Be(1443);
        
        // 1326 too low
        // 1327 too low
        // 1444 too high
    }

    private delegate long Op(long first, long second);

    private long SumOfValidTestOperators(string[] lines)
    {
        var sum = 0L;
        var availableOps = new Op[] { (a, b) => a + b, (a, b) => a * b };

        foreach (var line in lines)
        {
            sum += IsPossibleResult(line, availableOps);
        }

        return sum;
    }

    private long IsPossibleResult(string line, Op[] availableOps)
    {
        var result = long.Parse(line.Split(':')[0]);
        var inputs = line.Split(' ').Skip(1).Select(long.Parse).ToArray();
        var opCombinations = GetOpCombinations(inputs.Length - 1, availableOps);
        foreach (var opCombination in opCombinations.Select(c => c.ToArray()))
        {
            if (IsPossible(inputs, opCombination, result))
            {
                return result;
            }
        }

        return 0;
    }

    private bool IsPossible(long[] inputs, Op[] opCombination, long result)
    {
        try
        {
            var combinationResult = inputs[0];
            for (var i = 1; i < inputs.Length; i++)
            {
                combinationResult = opCombination[i - 1](combinationResult, inputs[i]);
            }
            if (combinationResult == result) return true;
        }
        catch
        {
            // ignored
        }

        return false;
    }

    private IEnumerable<IEnumerable<Op>> GetOpCombinations(int remainingOps, Op[] availableOps)
    {
        foreach (var availableOp in availableOps)
        {
            if (remainingOps == 1) yield return [availableOp];
            else
            {
                foreach (var combination in GetOpCombinations(remainingOps - 1, availableOps))
                {
                    yield return combination.Prepend(availableOp);
                }
            }
        }
    }
}