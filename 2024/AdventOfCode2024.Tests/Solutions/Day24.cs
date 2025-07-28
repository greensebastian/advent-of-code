using System.Diagnostics.CodeAnalysis;
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

        var sum = new WireDiagram(input).WrongWiresSorted();

        sum.Should().Be("z00,z01,z02,z05");
    }

    private class WireDiagram(string[] input)
    {
        private Dictionary<string, bool> StartingValues { get; } = input
            .TakeWhile(line => !string.IsNullOrWhiteSpace(line))
            .ToDictionary(
                line => line.Split(':')[0],
                line => line.Split(':')[1].Trim() == "1");

        private record GateLine(string Left, string Right, string Operand, string Output);

        private IEnumerable<GateLine> GetGateLines()
        {
            foreach (var connection in input.SkipWhile(line => !string.IsNullOrWhiteSpace(line)).Skip(1))
            {
                var split = connection.Split(' ');
                var i1 = split[0];
                var operand = split[1];
                var i2 = split[2];
                var o = split[4];
                yield return new GateLine(i1, i2, operand, o);
            }
        }

        private Dictionary<string, GateNode> GetGateNodes(IReadOnlyDictionary<string, bool> inputs)
        {
            var nodes = new Dictionary<string, GateNode>();
            foreach (var i in inputs)
            {
                var node = new StaticGateNode
                {
                    Value = i.Value,
                    Operand = '\0',
                    Left = null!,
                    Right = null!,
                    Key = i.Key
                };
                node.Left = node;
                node.Right = node;
                nodes[i.Key] = node;
            }

            var unresolved = new Queue<GateLine>();
            foreach (var gateLine in GetGateLines())
            {
                unresolved.Enqueue(gateLine);
            }
            
            var unchangesCycles = 0;

            while (unresolved.TryDequeue(out var connection))
            {
                if (unchangesCycles > unresolved.Count + 5) throw new Exception("Failed to solve system");
                if (!nodes.TryGetValue(connection.Left, out var left) || !nodes.TryGetValue(connection.Right, out var right))
                {
                    unresolved.Enqueue(connection);
                    unchangesCycles++;
                    continue;
                }
                unchangesCycles = 0;
                
                nodes[connection.Output] = new GateNode
                {
                    Operand = connection.Operand switch
                    {
                        "AND" => '&',
                        "OR" => '|',
                        "XOR" => '^',
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    Left = left,
                    Right = right,
                    Key = connection.Output
                };
            }

            return nodes;
        }

        private class GateNode
        {
            private bool? _outputCache;
            private GateNode _left;
            private GateNode _right;

            public required string Key { get; init; }
            public required char Operand { get; init; }
            
            public required GateNode Left
            {
                get => _left;
                [MemberNotNull(nameof(_left))]
                set
                {
                    _outputCache = false;
                    _left = value;
                }
            }

            public required GateNode Right
            {
                get => _right;
                [MemberNotNull(nameof(_right))]
                set
                {
                    _outputCache = false;
                    _right = value;
                }
            }

            public virtual bool Output()
            {
                return _outputCache ??= Operand switch
                {
                    '^' => Left.Output() != Right.Output(),
                    '|' => Left.Output() || Right.Output(),
                    '&' => Left.Output() && Right.Output(),
                    _ => throw new ArgumentOutOfRangeException(nameof(Operand), Operand, null)
                };
            }

            public override string ToString() => $"[{Key}]( {Left} {Operand} {Right} )";
        }

        private class StaticGateNode : GateNode
        {
            public required bool Value { get; init; }
            public override bool Output() => Value;
            
            public override string ToString() => $"[{Key}]( " + (Value ? "1" : "0") + " )";
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
            foreach (var gateLine in GetGateLines())
            {
                var swap = swaps.SingleOrDefault(swap => swap.Left == gateLine.Output || swap.Right == gateLine.Output);
                if (swap is not null)
                {
                    var swappedOutput = swap.Left == gateLine.Output ? swap.Right : swap.Left;
                    toAdd.Enqueue(gateLine with { Output = swappedOutput });
                }
                else
                {
                    toAdd.Enqueue(gateLine);
                }
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

        public string WrongWiresSorted()
        {
            return string.Join(',', FaultyWires().OrderBy(w => w));
        }

        private int InputLength => StartingValues.Keys.Select(k => int.Parse(k.Substring(1))).Max() + 1;

        private IEnumerable<string> FaultyWires(params Swap[] swaps)
        {
            Dictionary<string, bool> GetBaseInput()
            {
                var output = new Dictionary<string, bool>();
                for (var i = 0; i < InputLength; i++)
                {
                    output[$"x{i:00}"] = false;
                    output[$"y{i:00}"] = false;
                }

                return output;
            }
            
            for (var i = 1; i < InputLength; i++)
            {
                var prevLeft = $"x{i - 1:00}";
                var prevRight = $"y{i - 1:00}";
                var left = $"x{i:00}";
                var right = $"y{i:00}";
                var output = $"z{i:00}";
                var carry = $"z{i + 1:00}";
                var valid = true;
                foreach (var variant in Cases)
                {
                    var inputs = GetBaseInput();
                    inputs[left] = variant.Key.Left;
                    inputs[right] = variant.Key.Right;
                    if (variant.Key.Carry && i > 0)
                    {
                        inputs[prevLeft] = true;
                        inputs[prevRight] = true;
                    }
                    var nodes = GetGateNodes(inputs);
                    var result = nodes[output].Output();
                    var carryResult = nodes[carry].Output();
                    if (result != variant.Value.Output)
                    {
                        valid = false;
                        break;
                    }

                    if (i > 0 && carryResult != variant.Value.Carry)
                    {
                        valid = false;
                        break;
                    }
                }

                if (!valid) yield return output;
            }
        }

        public record Swap(string Left, string Right);

        private record Case(bool Left, bool Right, bool Carry);

        private record Result(bool Output, bool Carry);

        private Dictionary<Case, Result> Cases { get; } = new Case[]
        {
            new(false, false, false),
            new(false, false, true),
            new(false, true, false),
            new(false, true, true),
            new(true, false, false),
            new(true, false, true),
            new (true, true, false),
            new(true, true, true)
        }.ToDictionary(inp => inp,
            inp =>
            {
                var total = new[] { inp.Left, inp.Right, inp.Carry }.Sum(b => b ? 1 : 0);
                return total switch
                {
                    0 => new Result(false, false),
                    1 => new Result(true, false),
                    2 => new Result(false, true),
                    3 => new Result(true, true),
                    _ => throw new ArgumentOutOfRangeException()
                };
            });
    }
}