using FluentAssertions;
// ReSharper disable InvalidXmlDocComment

namespace AdventOfCode2024.Tests.Solutions;

public class Day24 : ISolution
{
    private const string Example = """
                                   x00: 1
                                   x01: 1
                                   x02: 1
                                   y00: 0
                                   y01: 1
                                   y02: 0
                                   
                                   x00 AND y00 -> z00
                                   x01 XOR y01 -> z01
                                   x02 OR y02 -> z02
                                   """;

    private const string LargeExample = """
                                        x00: 1
                                        x01: 0
                                        x02: 1
                                        x03: 1
                                        x04: 0
                                        y00: 1
                                        y01: 1
                                        y02: 1
                                        y03: 1
                                        y04: 1

                                        ntg XOR fgs -> mjb
                                        y02 OR x01 -> tnw
                                        kwq OR kpj -> z05
                                        x00 OR x03 -> fst
                                        tgd XOR rvg -> z01
                                        vdt OR tnw -> bfw
                                        bfw AND frj -> z10
                                        ffh OR nrd -> bqk
                                        y00 AND y03 -> djm
                                        y03 OR y00 -> psh
                                        bqk OR frj -> z08
                                        tnw OR fst -> frj
                                        gnj AND tgd -> z11
                                        bfw XOR mjb -> z00
                                        x03 OR x00 -> vdt
                                        gnj AND wpb -> z02
                                        x04 AND y00 -> kjc
                                        djm OR pbm -> qhw
                                        nrd AND vdt -> hwm
                                        kjc AND fst -> rvg
                                        y04 OR y02 -> fgs
                                        y01 AND x02 -> pbm
                                        ntg OR kjc -> kwq
                                        psh XOR fgs -> tgd
                                        qhw XOR tgd -> z09
                                        pbm OR djm -> kpj
                                        x03 XOR y03 -> ffh
                                        x00 XOR y04 -> ntg
                                        bfw OR bqk -> z06
                                        nrd XOR fgs -> wpb
                                        frj XOR qhw -> z04
                                        bqk OR frj -> z07
                                        y03 OR x01 -> nrd
                                        hwm AND bqk -> z03
                                        tgd XOR rvg -> z12
                                        tnw OR pbm -> gnj
                                        """;

    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(LargeExample);
        var input = Util.ReadFile("day24");

        var sum = new WireDiagram(input).GetOutput();
        sum.Should().Be(2024);
    }

    [Fact]
    public void Solution2()
    {
        var input = Util.ReadRaw(Example);
        //var input = Util.ReadFile("day24");

        var sum = new WireDiagram(input).GetOutput();

        sum.Should().Be(12);
    }

    private class WireDiagram(string[] input)
    {
        private Dictionary<string, bool> StartingValues { get; } = input
            .TakeWhile(line => !string.IsNullOrWhiteSpace(line))
            .ToDictionary(
                line => line.Split(':')[0],
                line => line.Split(':')[1].Trim() == "1");

        private record GateLine(string Left, string Right, string Operand, string Output);
        
        public long GetOutput()
        {
            var outputs = new Dictionary<string, bool>();
            foreach (var startingValue in StartingValues)
            {
                outputs[startingValue.Key] = startingValue.Value;
            }
            var toAdd = new Queue<GateLine>();
            foreach (var connection in input.SkipWhile(line => !string.IsNullOrWhiteSpace(line)).Skip(1))
            {
                var split = connection.Split(' ');
                var i1 = split[0];
                var operand = split[1];
                var i2 = split[2];
                var o = split[4];
                toAdd.Enqueue(new GateLine(i1, i2, operand, o));
            }

            while (toAdd.TryDequeue(out var connection))
            {
                if (!outputs.TryGetValue(connection.Left, out var left) || !outputs.TryGetValue(connection.Right, out var right))
                {
                    toAdd.Enqueue(connection);
                    continue;
                }

                outputs[connection.Output] = connection.Operand switch
                {
                    "AND" => left && right,
                    "OR" => left || right,
                    "XOR" => left != right,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            var output = 0L;
            var outputValues = outputs.Where(kv => kv.Key.StartsWith('z')).OrderByDescending(kv => kv.Key);
            foreach (var kv in outputValues)
            {
                output <<= 1;
                output += kv.Value ? 1 : 0;
            }

            return output;
        }
    }
}