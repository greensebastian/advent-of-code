using Shouldly;

namespace AdventOfCode2021.Core.Test.Day16;

public class Day16
{
    private const string Example = """
                                   D2FE28
                                   """;

    [Fact]
    public void Literal_Example()
    {
        const string input = "D2FE28";
        (Packet.FromLine(input) as LiteralPacket)?.Value.ShouldBe(2021);
    }
    
    [Fact]
    public void Operator_Example()
    {
        var p1 = Packet.FromLine("38006F45291200") as OperatorPacket;
        (p1?.SubPackets[0] as LiteralPacket)?.Value.ShouldBe(10);
        (p1?.SubPackets[1] as LiteralPacket)?.Value.ShouldBe(20);
        
        var p2 = Packet.FromLine("EE00D40C823060") as OperatorPacket;
        (p2?.SubPackets[0] as LiteralPacket)?.Value.ShouldBe(1);
        (p2?.SubPackets[1] as LiteralPacket)?.Value.ShouldBe(2);
        (p2?.SubPackets[2] as LiteralPacket)?.Value.ShouldBe(3);
    }
    
    [Fact]
    public void Operator_Nested_Example()
    {
        var p = Packet.FromLine("A0016C880162017C3686B18A3D4780");
        p.GetVersionSum().ShouldBe(31);
    }
    
    [Fact]
    public void Part_1_Example()
    {
        var input = Example.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        0.ShouldBe(0);
    }
    
    [Fact]
    public void Part_1_Real()
    {
        var input = Util.ReadFile("day16");
        var packet = Packet.FromLine(input.Single());
        packet.GetVersionSum().ShouldBe(877);
    }
    
    [Fact]
    public void Part_2_Example()
    {
        var input = Example.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        0.ShouldBe(0);
    }

    [Theory]
    [InlineData("C200B40A82", 3)]
    [InlineData("04005AC33890", 54)]
    [InlineData("880086C3E88112", 7)]
    [InlineData("CE00C43D881120", 9)]
    [InlineData("D8005AC2A8F0", 1)]
    [InlineData("F600BC2D8F", 0)]
    [InlineData("9C005AC2F8F0", 0)]
    [InlineData("9C0141080250320F1802104A08", 1)]
    public void Part_2_Tests(string input, long output)
    {
        Packet.FromLine(input).GetValue().ShouldBe(output);
    }
    
    [Fact]
    public void Part_2_Real()
    {
        var input = Util.ReadFile("day16");
        var packet = Packet.FromLine(input.Single());
        packet.GetValue().ShouldBe(194435634456L);
        // 9752041495L too low
    }
}

public abstract class Packet
{
    public static Range VersionRange { get; } = new(0, 3);
    public static Range TypeRange { get; } = new(3, 6);
    private static Range LiteralValueRange { get; } = Range.StartAt(6);
    private static Range LengthTypeRange { get; } = new(6, 7);
    private static Range SubPackLengthRange { get; } = new(7, 7 + 15);
    private static Range SubPackCountRange { get; } = new(7, 7 + 11);

    public abstract int GetVersionSum();

    public abstract long GetValue();
    
    public required int Version { get; init; }
    public required int Type { get; init; }
    
    public required int Length { get; init; }
    
    public static Packet FromLine(string line)
    {
        var binary = line.Select(c => c <= '9' ? c - '0' : c - 'A' + 10).SelectMany(i =>
            new[] { ((i >> 3) & 1) == 1, ((i >> 2) & 1) == 1, ((i >> 1) & 1) == 1, (i & 1) == 1 }).ToArray();
        Console.WriteLine(string.Join("", binary.Select(b => b ? "1" : "0")));
        return FromBinary(binary);
    }

    private static Packet FromBinary(bool[] bits)
    {
        var h = bits.GetHeader();
        if (h.Type == 4)
        {
            var takenLast = false;
            var chunks = bits[LiteralValueRange].Chunk(5).TakeWhile(c =>
            {
                if (takenLast) return false;
                if (!c[0])
                {
                    takenLast = true;
                }

                return true;
            }).ToArray();
            var num = chunks.SelectMany(c => c[1..]).ToLong();
            return new LiteralPacket(num)
            {
                Length = LiteralValueRange.Start.Value + chunks.Length * 5,
                Version = h.Version,
                Type = h.Type
            };
        }

        var subsPackagedByCount = bits[LengthTypeRange][0];
        if (subsPackagedByCount)
        {
            var numPackets = bits[SubPackCountRange].ToLong();
            var pos = SubPackCountRange.End.Value;
            var packets = new List<Packet>();
            while (packets.Count < numPackets)
            {
                var packet = FromBinary(bits[pos..]);
                packets.Add(packet);
                pos += packet.Length;
            }

            return new OperatorPacket(packets.ToArray())
            {
                Length = SubPackCountRange.End.Value + packets.Sum(packet => packet.Length),
                Version = h.Version,
                Type = h.Type
            };
        }
        else
        {
            var lenPackets = bits[SubPackLengthRange].ToLong();
            var pos = SubPackLengthRange.End.Value;
            var packets = new List<Packet>();
            while (packets.Sum(p => p.Length) < lenPackets)
            {
                var packet = FromBinary(bits[pos..]);
                packets.Add(packet);
                pos += packet.Length;
            }

            return new OperatorPacket(packets.ToArray())
            {
                Length = SubPackLengthRange.End.Value + packets.Sum(packet => packet.Length),
                Version = h.Version,
                Type = h.Type
            };
        }
    }
}

public static class BitOps
{
    extension(bool[] bits)
    {
        public (int Version, int Type) GetHeader()
        {
            return ((int)bits[Packet.VersionRange].ToLong(), (int)bits[Packet.TypeRange].ToLong());
        }
    }
    
    extension(IEnumerable<bool> bits)
    {
        public long ToLong()
        {
            return bits.Reverse().Select((b, i) => (b ? 1L : 0L) << i).Sum();
        }
    }
}

public class LiteralPacket(long value) : Packet
{
    public long Value { get; } = value;
    public override int GetVersionSum()
    {
        return Version;
    }

    public override long GetValue()
    {
        return Value;
    }
}

public class OperatorPacket(Packet[] subPackets) : Packet
{
    public Packet[] SubPackets { get; } = subPackets;
    public override int GetVersionSum()
    {
        return Version + SubPackets.Sum(sp => sp.GetVersionSum());
    }

    public override long GetValue()
    {
        return Type switch
        {
            0 => SubPackets.Sum(p => p.GetValue()),
            1 => SubPackets.Aggregate(1L, (product, packet) => product * packet.GetValue()),
            2 => SubPackets.Min(p => p.GetValue()),
            3 => SubPackets.Max(p => p.GetValue()),
            5 => SubPackets[0].GetValue() > SubPackets[1].GetValue() ? 1L : 0L,
            6 => SubPackets[0].GetValue() < SubPackets[1].GetValue() ? 1L : 0L,
            7 => SubPackets[0].GetValue() == SubPackets[1].GetValue() ? 1L : 0L,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}