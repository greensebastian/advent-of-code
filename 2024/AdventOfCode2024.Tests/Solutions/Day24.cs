using System.Text;
using System.Text.RegularExpressions;
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

    private const string P2Example = """
                                     x00: 0
                                     x01: 1
                                     x02: 0
                                     x03: 1
                                     x04: 0
                                     x05: 1
                                     y00: 0
                                     y01: 0
                                     y02: 1
                                     y03: 1
                                     y04: 0
                                     y05: 1

                                     x00 AND y00 -> z05
                                     x01 AND y01 -> z02
                                     x02 AND y02 -> z01
                                     x03 AND y03 -> z03
                                     x04 AND y04 -> z04
                                     x05 AND y05 -> z00
                                     """;

    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(LargeExample);
        var input = Util.ReadFile("day24");

        var sum = new WireDiagram(input).GetStartingOutput();
        sum.Should().Be(2024);
    }

    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw(P2Example);
        var input = Util.ReadFile("day24");

        var wg = new WireDiagram(input);

        WireDiagram.Swap[] swaps =
        [
            new("qjj", "gjc"),
            new("wmp", "z17"),
            new("vsm", "z39"),
            new("gvm", "z26")
        ];
        var visualized = new WireDiagram(input).ToGraphViz(swaps, true);

        var orderedSwaps = string.Join(",", swaps.SelectMany(s => new[] { s.Left, s.Right }).Order());
        orderedSwaps.Should().Be("");
    }

    private class WireDiagram(string[] input)
    {
        private Dictionary<string, bool> StartingValues { get; } = input
            .TakeWhile(line => !string.IsNullOrWhiteSpace(line))
            .ToDictionary(
                line => line.Split(':')[0],
                line => line.Split(':')[1].Trim() == "1");

        private record GateLine(string Left, string Right, string Operand, string Output)
        {
            public override string ToString() => $"{Left} {Operand} {Right} => {Output}";

            public string Expand(IReadOnlyDictionary<string, GateLine> gates, bool wrap = false)
            {
                var left = gates.TryGetValue(Left, out var leftMatch) ? leftMatch.Expand(gates, wrap) : Left;
                var right = gates.TryGetValue(Right, out var rightMatch) ? rightMatch.Expand(gates, wrap) : Right;
                var wrapLeft = wrap ? "(" : "";
                var wrapRight = wrap ? ")" : "";
                return $"{wrapLeft}{left} {Operand} {right}{wrapRight}";
            }
        }

        private IEnumerable<GateLine> GetGateLines(params Swap[] swaps)
        {
            foreach (var connection in input.SkipWhile(line => !string.IsNullOrWhiteSpace(line)).Skip(1))
            {
                var split = connection.Split(' ');
                var i1 = split[0];
                var operand = split[1];
                var i2 = split[2];
                var o = split[4];
                var swap = swaps.FirstOrDefault(s => s.Right == o || s.Left == o);
                if (swap is not null)
                {
                    o = swap.Left == o ? swap.Right : swap.Left;
                }
                var i1First = string.Compare(i1, i2, StringComparison.InvariantCultureIgnoreCase) <= 0;
                yield return new GateLine(i1First ? i1 : i2, i1First ? i2 : i1, operand, o);
            }
        }

        public long GetStartingOutput() => GetOutput(StartingValues);
        
        private long GetOutput(IReadOnlyDictionary<string, bool> inputs, params Swap[] swaps)
        {
            var outputs = new Dictionary<string, bool>();
            foreach (var startingValue in inputs)
            {
                outputs[startingValue.Key] = startingValue.Value;
            }
            var toAdd = new Queue<GateLine>();
            foreach (var gateLine in GetGateLines(swaps))
            {
                toAdd.Enqueue(gateLine);
            }

            var unchangesCycles = 0;

            while (toAdd.TryDequeue(out var connection))
            {
                if (unchangesCycles > toAdd.Count + 5) throw new Exception("Failed to solve system");
                if (!outputs.TryGetValue(connection.Left, out var left) || !outputs.TryGetValue(connection.Right, out var right))
                {
                    toAdd.Enqueue(connection);
                    unchangesCycles++;
                    continue;
                }

                unchangesCycles = 0;

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
        
        public string ToGraphViz(Swap[] swaps, bool rename = false)
        {
            var gates = GetGateLines(swaps).ToDictionary(gate => gate.Output);
            var t = gates.Values.ToArray();
            var numReg = new Regex("\\d+");
            var mappings = new Dictionary<string, string>();
            if (rename)
            {
                var unmarked = t.Where(g => !numReg.IsMatch(g.Output)).ToArray();
                while (unmarked.Length > 0)
                {
                    unmarked = t.Where(g => !numReg.IsMatch(g.Output)).ToArray();
                    for (var i = 0; i < unmarked.Length; i++)
                    {
                        var old = unmarked[i];
                        var oldKey = old.Output;
                        if (numReg.IsMatch(oldKey)) continue;

                        var left = numReg.Match(old.Left);
                        var right = numReg.Match(old.Right);
                        if (left.Success && right.Success)
                        {
                            var n = Math.Max(int.Parse(left.Value), int.Parse(right.Value));
                            var newKey = $"{old.Operand.ToLower()[..2]}{n:00}";
                            if (mappings.ContainsKey(newKey)) newKey += "_n";
                            mappings[newKey] = oldKey;

                            for (var j = 0; j < t.Length; j++)
                            {
                                if (t[j].Left == oldKey) t[j] = t[j] with { Left = newKey };
                                if (t[j].Right == oldKey) t[j] = t[j] with { Right = newKey };
                                if (t[j].Output == oldKey) t[j] = t[j] with { Output = newKey };
                            }
                        }
                    }
                }
            }
            
            var sb = new StringBuilder();
            sb.AppendLine("digraph G {");
            foreach (var gateLine in t)
            {
                sb.AppendIndented($"{gateLine.Left} -> {gateLine.Output}", 4);
                sb.AppendIndented($"{gateLine.Right} -> {gateLine.Output}", 4);
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        public record Swap(string Left, string Right);
    }
}