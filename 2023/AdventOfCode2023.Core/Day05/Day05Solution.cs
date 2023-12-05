namespace AdventOfCode2023.Core.Day05;

public record Day05Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var mappingTable = MappingTable.FromInputLines(Input.ToArray());
        yield return mappingTable.LowestLocationId().ToString();
    }

    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return 0.ToString();
    }
}

public record MappingTable(IReadOnlyList<ulong> Seeds, IReadOnlyDictionary<string, Mapping> MappingsFromSource)
{
    public ulong LowestLocationId()
    {
        var mappings = Seeds.Select(MapToEnd).ToList();
        return mappings.Min();
    }
    
    private ulong MapToEnd(ulong seedId)
    {
        var sourceType = "seed";
        var sourceId = seedId;
        while (MappingsFromSource.ContainsKey(sourceType))
        {
            sourceId = MappingsFromSource[sourceType].Map(sourceId);
            sourceType = MappingsFromSource[sourceType].Destination;
        }

        return sourceId;
    }
    
    public static MappingTable FromInputLines(IList<string> lines)
    {
        var seeds = lines[0].Split(" ").Skip(1).Select(ulong.Parse).ToList();

        var mappings = new Dictionary<string, Mapping>();
        var startOfMap = 2;
        var i = 3;
        while (i <= lines.Count)
        {
            if (i < lines.Count && !string.IsNullOrWhiteSpace(lines[i]))
            {
                i++;
                continue;
            };
            
            var mapping = Mapping.FromInputLines(lines.Skip(startOfMap).Take(i - startOfMap).ToArray());
            mappings[mapping.Source] = mapping;
            startOfMap = i + 1;
            i = startOfMap + 1;
        }

        return new MappingTable(seeds.AsReadOnly(), mappings.AsReadOnly());
    }
}

public record Mapping(string Source, string Destination, IReadOnlyList<Range> Ranges)
{
    public ulong Map(ulong sourceId)
    {
        var range = Ranges.FirstOrDefault(r => r.SourceStart <= sourceId && r.SourceStart + r.Width > sourceId);
        if (range is null) return sourceId;
        return sourceId - range.SourceStart + range.DestinationStart;
    }
    
    public static Mapping FromInputLines(IList<string> lines)
    {
        var source = lines[0].Split(" ")[0].Split("-")[0];
        var destination = lines[0].Split(" ")[0].Split("-")[2];

        var ranges = new SortedList<ulong, Range>();
        foreach (var line in lines.Skip(1))
        {
            var range = Range.FromInput(line);
            ranges.Add(range.SourceStart, range);
        }

        return new Mapping(source, destination, ranges.Values.AsReadOnly());
    }
}

public record Range(ulong SourceStart, ulong DestinationStart, ulong Width)
{
    public static Range FromInput(string line)
    {
        var sourceStart = ulong.Parse(line.Split(" ")[1]);
        var destinationStart = ulong.Parse(line.Split(" ")[0]);
        var width = ulong.Parse(line.Split(" ")[2]);

        return new Range(sourceStart, destinationStart, width);
    }
}