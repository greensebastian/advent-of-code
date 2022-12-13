namespace AdventOfCode2022.Core.Day13;

public record Day13Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var pairs = new List<PacketPair>();

        var correctPairSum = 0;
        var pairIndex = 0;
        foreach (var line in Input.Chunk(3))
        {
            pairIndex++;
            var left = new Packet(line[0]);
            var right = new Packet(line[1]);
            var pair = new PacketPair(left, right);
            pairs.Add(pair);

            if (pair.OrderedCorrectly)
            {
                Console.WriteLine($"In order: {pairIndex}");
                Console.WriteLine(left);
                Console.WriteLine(right);
                correctPairSum += pairIndex;
            }
        }

        yield return correctPairSum.ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        yield return "0";
    }
}

public record PacketPair(Packet Left, Packet Right)
{
    public bool OrderedCorrectly { get; } = LeftBeforeRight(Left, Right)!.Value;

    private static bool? LeftBeforeRight(Packet left, Packet right)
    {
        if (left.Value is not null && right.Value is not null)
        {
            if (left.Value < right.Value) return true;
            if (left.Value > right.Value) return false;
            return null;
        }
        
        if (left.Value is null && right.Value is not null)
        {
            return LeftBeforeRight(left, new Packet($"[{right}]"));
        }

        if (left.Value is not null && right.Value is null)
        {
            return LeftBeforeRight(new Packet($"[{left}]"), right);
        }

        for (var i = 0; i < left.Packets.Count; i++)
        {
            if (i >= right.Packets.Count) return false;
            
            var result = LeftBeforeRight(left.Packets[i], right.Packets[i]);
            if (result.HasValue)
                return result.Value;
        }
        
        if (left.Packets.Count < right.Packets.Count) return true;

        return null;
    }
}

public record Packet
{
    public int? Value { get; }

    public List<Packet> Packets { get; } = new();
    
    public Packet(string input)
    {
        if (input[0] == '[')
        {
            var currentPacket = string.Empty;
            var bracketCount = 0;
            foreach (var packetChar in input[1..^1])
            {
                switch (packetChar)
                {
                    case ',' when bracketCount == 0:
                        Packets.Add(new Packet(currentPacket));
                        currentPacket = string.Empty;
                        break;
                    case '[':
                        bracketCount++;
                        currentPacket += packetChar;
                        break;
                    case ']':
                        bracketCount--;
                        currentPacket += packetChar;
                        break;
                    default:
                        currentPacket += packetChar;
                        break;
                }
            }
            if (currentPacket != string.Empty)
                Packets.Add(new Packet(currentPacket));
        }
        else if (input.Contains(','))
        {
            Packets.AddRange(input.Split(",").Select(word => new Packet(word)));
        }
        else
        {
            Value = int.Parse(input);
        }
    }

    public override string ToString()
    {
        return Value is not null 
            ? Value.Value.ToString() 
            : $"[{string.Join(",", Packets.Select(p => p.ToString()))}]";
    }
}