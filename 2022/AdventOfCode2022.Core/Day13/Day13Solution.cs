namespace AdventOfCode2022.Core.Day13;

public record Day13Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
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
                Log.Invoke($"In order: {pairIndex}");
                Log.Invoke(left.ToString());
                Log.Invoke(right.ToString());
                correctPairSum += pairIndex;
            }
        }

        yield return correctPairSum.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var firstIndicator = new Packet("[[2]]");
        var secondIndicator = new Packet("[[6]]");
        var packets = new List<Packet>
        {
            firstIndicator,
            secondIndicator
        };
        
        foreach (var line in Input)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            var p = new Packet(line);
            packets.Add(p);
        }

        packets.Sort();
        
        var firstIndex = packets.IndexOf(firstIndicator) + 1;
        var secondIndex = packets.IndexOf(secondIndicator) + 1;

        yield return (firstIndex * secondIndex).ToString();
    }
}

public record PacketPair(Packet Left, Packet Right)
{
    public bool OrderedCorrectly { get; } = Left.CompareTo(Right) <= 0;
}

public record Packet : IComparable<Packet>
{
    private int? Value { get; }

    private List<Packet> Packets { get; } = new();
    
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
        else
        {
            Value = int.Parse(input);
        }
    }

    public int CompareTo(Packet? other)
    {
        var result = LeftBeforeRight(this, other!);
        return result switch
        {
            null => 0,
            true => -1,
            false => 1
        };
    }
    
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

    public override string ToString()
    {
        return Value is not null 
            ? Value.Value.ToString() 
            : $"[{string.Join(",", Packets.Select(p => p.ToString()))}]";
    }
}