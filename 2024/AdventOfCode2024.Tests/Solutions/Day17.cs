using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day17 : ISolution
{
    private const string Example = """
                                   Register A: 729
                                   Register B: 0
                                   Register C: 0
                                   
                                   Program: 0,1,5,4,3,0
                                   """;
    
    private const string ExampleTwo = """
                                      Register A: 2024
                                      Register B: 0
                                      Register C: 0
                                      
                                      Program: 0,3,5,4,3,0
                                      """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day17");

        var result = SolveProgram(input);
        result.Should().Be("3,6,7,0,5,7,3,1,4");
    }

    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw(ExampleTwo);
        var input = Util.ReadFile("day17");

        var result = BruteForceInput(input);
        result.Should().Be(117440);
    }

    private string SolveProgram(string[] input)
    {
        return Machine.FromInput(input).Run(_ => false);
    }
    
    private long BruteForceInput(string[] input)
    {
        for (var a = 1L; a < 1_000_000; a++)
        {
            var machine = Machine.FromInput(input);
            var output = Machine.FromInput(input).Run(output =>
            {
                if (output.Length > machine.Sequence.Length) return true;
                return output.Where((t, i) => t != machine.Sequence[i]).Any();
            }, a);
            if (output == string.Join(",", machine.Sequence)) return a;
        }

        return -1;
    }

    private class Machine(long a, long b, long c, long[] sequence)
    {
        public long[] Sequence => sequence;
        
        public static Machine FromInput(string[] input)
        {
            return new Machine(input[0].PlusMinusLongs().Single(), input[1].PlusMinusLongs().Single(),
                input[2].PlusMinusLongs().Single(), input[4].PlusMinusLongs().ToArray());
        }
        
        public string Run(Func<long[], bool> exitCondition, long? aReplacement = null)
        {
            if (aReplacement is not null) a = aReplacement.Value;
            var output = new List<long>();
            for (var pointer = 0L; pointer < sequence.Length; pointer += 2)
            {
                if (exitCondition(output.ToArray())) return "";
                var opcode = sequence[pointer];
                var operand = sequence[pointer + 1];
                switch (opcode)
                {
                    case 0:
                    {
                        a = (long)(a / Math.Pow(2, ComboOperand(operand)));
                        break;
                    }
                    case 1:
                    {
                        b = b ^ operand;
                        break;
                    }
                    case 2:
                    {
                        b = ComboOperand(operand) % 8;
                        break;
                    }
                    case 3 when a != 0:
                    {
                        pointer = operand - 2;
                        break;
                    }
                    case 4:
                    {
                        b = b ^ c;
                        break;
                    }
                    case 5:
                    {
                        output.Add(ComboOperand(operand) % 8);
                        break;
                    }
                    case 6:
                    {
                        b = (long)(a / Math.Pow(2, ComboOperand(operand)));
                        break;
                    }
                    case 7:
                    {
                        c = (long)(a / Math.Pow(2, ComboOperand(operand)));
                        break;
                    }
                }
            }

            return string.Join(",", output);
        }

        private long ComboOperand(long n) => n switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            3 => 3,
            4 => a,
            5 => b,
            6 => c,
            _ => throw new ArgumentOutOfRangeException(nameof(n), n, null)
        };
    }
    
    private long Solve(string[] input) => throw new NotImplementedException();
}