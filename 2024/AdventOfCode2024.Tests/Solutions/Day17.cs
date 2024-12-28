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

        var res = FindAOptionRecursive(Seq);
        res.Should().Be(164278496489149L);
    }
    
    [Fact]
    public void TestStuff()
    {
        foreach (var option in FindAOptions([5,4,2,5,5,0,3,3,0]))
        {
            Console.WriteLine($"{option:B48}");
        }
    }
    
    [Fact]
    public void TestRecursion()
    {
        var res = FindAOptionRecursive(Seq);
        Console.WriteLine($"{res:B48}");
    }

    private string SolveProgram(string[] input)
    {
        return Machine.FromInput(input).Run(_ => false);
    }
    
    private long BruteForceInput(string[] input)
    {
        var baseMachine = Machine.FromInput(input);
        var targetOutput = string.Join(",", baseMachine.Sequence);
        for (var a = 1L; a < 10_000_000_000; a++)
        {
            var output = new Machine(a, 0, 0, baseMachine.Sequence).Run(ImpossibleOutput, a);
            if (output == targetOutput) return a;
        }

        return -1;

        bool ImpossibleOutput(ICollection<long> output)
        {
            if (baseMachine == null) throw new Exception();
            
            if (output.Count > baseMachine.Sequence.Length) return true;
            return output.Where((t, i) => t != baseMachine.Sequence[i]).Any();
        }
    }

    private static readonly byte[] Seq = { 2, 4, 1, 1, 7, 5, 1, 5, 4, 2, 5, 5, 0, 3, 3, 0 };
    
    private static bool MatchesOutput(ICollection<byte> output, byte[] seq)
    {
        if (output.Count != seq.Length) return false;
        return !output.Where((t, i) => t != seq[i]).Any();
    }
    
    private static long FindA()
    {
        for (var startA = 1L << Seq.Length * 3 - 1; startA < 1L << (Seq.Length * 3 + 3); startA++)
        {
            if (PrintsItself(startA, Seq)) return startA;
        }

        return -1;
    }
    
    private static IEnumerable<long> FindAOptions(byte[] seq)
    {
        for (var startA = 1L << (seq.Length - 1) * 3; startA < 1L << seq.Length * 3; startA++)
        //for (var startA = 1L; startA < 1_000_000_000; startA++)
        {
            if (PrintsItself(startA, seq)) yield return startA;
        }
    }
    
    private static long FindAOptionRecursive(byte[] seq)
    {
        if (seq.Length == 1)
        {
            for (var startA = 1L; startA < 8; startA++)
            {
                if (PrintsItself(startA, seq)) return startA;
            }

            throw new Exception();
        }

        var min = FindAOptionRecursive(seq[1..]) << 3;
        for (var startA = min; startA < min << 3; startA++)
        {
            if (PrintsItself(startA, seq)) return startA;
        }

        throw new Exception();
    }
    
    private static bool PrintsItself(long a, byte[] seq)
    {
        var output = new List<byte>(seq.Length);
        while (a != 0 && output.Count < seq.Length)
        {
            // ((((a & 0b111) ^ 1) ^ 5) ^ (a >> ((a & 0b111) ^ 1))) & 0b111
            // 
            // 2,4
            var b = a & 0b111;
            // 1,1
            b ^= 1;
            // 7,5
            var c = a >> (int)b;
            // 1,5
            b ^= 5;
            // 4,2
            b ^= c;
            // 5,5
            var needed = seq[output.Count];
            var toOutput = (byte)(b & 0b111);
            if (toOutput != needed) break;
            output.Add(toOutput);
            // 0,3
            a >>= 3;
            // 3,0
        }

        return MatchesOutput(output, seq);
    }

    private class Machine(long a, long b, long c, long[] sequence)
    {
        public long[] Sequence => sequence;
        
        public static Machine FromInput(string[] input)
        {
            return new Machine(input[0].PlusMinusLongs().Single(), input[1].PlusMinusLongs().Single(),
                input[2].PlusMinusLongs().Single(), input[4].PlusMinusLongs().ToArray());
        }
        
        public string Run(Func<IList<long>, bool> impossibleOutput, long? aReplacement = null)
        {
            if (aReplacement is not null) a = aReplacement.Value;
            var output = new List<long>();
            for (var pointer = 0L; pointer < sequence.Length; pointer += 2)
            {
                var opcode = sequence[pointer];
                var operand = sequence[pointer + 1];
                switch (opcode)
                {
                    case 0:
                    {
                        a >>= (int)ComboOperand(operand);
                        break;
                    }
                    case 1:
                    {
                        b ^= operand;
                        break;
                    }
                    case 2:
                    {
                        b = ComboOperand(operand) & 0b111;
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
                        output.Add(ComboOperand(operand) & 0b111);
                        if (impossibleOutput(output)) return "";
                        break;
                    }
                    case 6:
                    {
                        b = a >> (int)ComboOperand(operand);
                        break;
                    }
                    case 7:
                    {
                        c = a >> (int)ComboOperand(operand);
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