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
        var mappingTable = MappingTable.FromInputLines(Input.ToArray());
        yield return mappingTable.LowestLocationByBreakpointEvaluation().ToString();
    }
}

public record MappingTable(IReadOnlyList<ulong> Seeds, IReadOnlyDictionary<string, Mapping> MappingsFromSource)
{
    private IReadOnlyDictionary<string, Mapping> MappingsFromDestination =
        MappingsFromSource.Values.ToDictionary(m => m.Destination);
    
    public ulong LowestLocationByBreakpointEvaluation()
    {
        var locations = ExpandBreakpoints(SeedBreakpoints(), "seed").Order().ToList();
        var bestLocation = locations.Where(l => IsValidSeed(MapToStart(l, "location")))
            .Select(l => MapToEnd(MapToStart(l, "location")))
            .Min();
        return bestLocation;
    }

    private IEnumerable<ulong> SeedBreakpoints()
    {
        for (var i = 0; i < Seeds.Count; i += 2)
        {
            var start = Seeds[i];
            var width = Seeds[i + 1];
            yield return start - 1;
            yield return start;
            yield return start + width - 1;
            yield return start + width;
        }
    }

    private bool IsValidSeed(ulong id)
    {
        for (var i = 0; i < Seeds.Count; i += 2)
        {
            var start = Seeds[i];
            var width = Seeds[i + 1];
            if (start <= id && id < start + width) return true;
        }

        return false;
    }

    private IEnumerable<ulong> ExpandBreakpoints(IEnumerable<ulong> breakpoints, string sourceType)
    {
        if (!MappingsFromSource.ContainsKey(sourceType)) return breakpoints;
        var mapping = MappingsFromSource[sourceType];
        var newBreakpoints =
            breakpoints.Concat(mapping.Ranges.SelectMany(r => new[] { r.SourceStart - 1, r.SourceStart, r.SourceStart + r.Width - 1, r.SourceStart + r.Width })).Distinct();
        return ExpandBreakpoints(newBreakpoints.Select(mapping.Map).Order(), mapping.Destination);
    }
    
    public ulong LowestLocationId()
    {
        var mappings = Seeds.Select(MapToEnd).ToList();
        return mappings.Min();
    }
    
    public ulong MapToEnd(ulong seedId)
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
    
    public ulong MapToStart(ulong id, string type)
    {
        var sourceType = type;
        var sourceId = id;
        while (MappingsFromDestination.ContainsKey(sourceType))
        {
            sourceId = MappingsFromDestination[sourceType].MapReverse(sourceId);
            sourceType = MappingsFromDestination[sourceType].Source;
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

public record Mapping(string Source, string Destination, IReadOnlyList<RangeMap> Ranges)
{
    public ulong Map(ulong sourceId)
    {
        var range = Ranges.FirstOrDefault(r => r.SourceStart <= sourceId && r.SourceStart + r.Width > sourceId);
        if (range is null) return sourceId;
        return sourceId - range.SourceStart + range.DestinationStart;
    }
    
    public ulong MapReverse(ulong destinationId)
    {
        var range = Ranges.FirstOrDefault(r => r.DestinationStart <= destinationId && r.DestinationStart + r.Width > destinationId);
        if (range is null) return destinationId;
        return destinationId - range.DestinationStart + range.SourceStart;
    }
    
    public static Mapping FromInputLines(IList<string> lines)
    {
        var source = lines[0].Split(" ")[0].Split("-")[0];
        var destination = lines[0].Split(" ")[0].Split("-")[2];

        var ranges = new SortedList<ulong, RangeMap>();
        foreach (var line in lines.Skip(1))
        {
            var range = RangeMap.FromInput(line);
            ranges.Add(range.SourceStart, range);
        }

        return new Mapping(source, destination, ranges.Values.AsReadOnly());
    }
}

public record RangeMap(ulong SourceStart, ulong DestinationStart, ulong Width)
{
    public static RangeMap FromInput(string line)
    {
        var sourceStart = ulong.Parse(line.Split(" ")[1]);
        var destinationStart = ulong.Parse(line.Split(" ")[0]);
        var width = ulong.Parse(line.Split(" ")[2]);

        return new RangeMap(sourceStart, destinationStart, width);
    }
}