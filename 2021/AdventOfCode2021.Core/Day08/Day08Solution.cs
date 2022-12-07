namespace AdventOfCode2021.Core.Day08;

public record Day08Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    /**
     *  aaaa
     * b    c
     * b    c
     *  dddd
     * e    f
     * e    f
     *  gggg
     */
    
    public override IEnumerable<string> FirstSolution()
    {
        var uniqueCount = 0;
        
        foreach (var line in Input)
        {
            var output = line
                .Split(" | ")[1]
                .Split(" ");

            uniqueCount += output.Count(set => set.Length is 2 or 3 or 4 or 7);
        }

        yield return uniqueCount.ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        var outputSum = 0;
        foreach (var line in Input)
        {
            var decoder = new Decoder(line);

            var output = line
                .Split(" | ")[1]
                .Split(" ");

            var outputWord = string.Empty;
            foreach (var encodedWord in output)
            {
                var decoded = decoder.Decode(encodedWord);
                outputWord += decoded;
            }

            outputSum += int.Parse(outputWord);
        }

        yield return outputSum.ToString();
    }

    private void Print(Decoder decoder)
    {
        foreach (var line in decoder.GetPrint())
        {
            Console.WriteLine(line);
        }
        Console.WriteLine();
    }
}

internal class Decoder
{
    private const string SegmentSet = "abcdefg";
    
    private readonly Dictionary<char, HashSet<char>>
        _mappingOptions = SegmentSet.ToDictionary(c => c, _ => GetNewOptionSet());

    private readonly HashSet<char> _singlesDone = new();

    private readonly Dictionary<char, int> _countByEncoded = new();

    private static readonly Dictionary<int, HashSet<char>> SegmentsByDigit = new()
    {
        { 0, new HashSet<char>("abcefg") },
        { 1, new HashSet<char>("cf") },
        { 2, new HashSet<char>("acdeg") },
        { 3, new HashSet<char>("acdfg") },
        { 4, new HashSet<char>("bcdf") },
        { 5, new HashSet<char>("abdfg") },
        { 6, new HashSet<char>("abdefg") },
        { 7, new HashSet<char>("acf") },
        { 8, new HashSet<char>("abcdefg") },
        { 9, new HashSet<char>("abcdfg") },
    };

    public Decoder(string input)
    {
        var encodedSets = input
            .Split(" | ")[0]
            .Split(" ");

        foreach (var encodedSet in encodedSets)
        {
            AddUnknown(encodedSet);
            foreach (var encoded in encodedSet)
            {
                if (!_countByEncoded.ContainsKey(encoded))
                    _countByEncoded[encoded] = 0;

                _countByEncoded[encoded] += 1;
            }
        }
        
        CheckByAppearanceCount();
    }

    public int Decode(string encodedDigit)
    {
        var decodedChars = new HashSet<char>();
        foreach (var encoded in encodedDigit)
        {
            var decoded = _mappingOptions[encoded].Single();
            decodedChars.Add(decoded);
        }

        var digitOptions = SegmentsByDigit.ToList();
        foreach (var decoded in decodedChars)
        {
            digitOptions.RemoveAll(opt => !opt.Value.Contains(decoded) || opt.Value.Count != decodedChars.Count);
        }

        return digitOptions.Single().Key;
    }
    
    public IEnumerable<string> GetPrint()
    {
        foreach (var (encoded, options) in _mappingOptions)
        {
            yield return $"{encoded}: {string.Join("", options.Order())}";
        }
    }

    private bool Solved => _mappingOptions.Values.All(opt => opt.Count == 1);

    private void AddUnknown(string encodedSet)
    {
        int? knownDigit = encodedSet.Length switch
        {
            1 => 1,
            3 => 7,
            4 => 4,
            7 => 8,
            _ => null
        };

        if (knownDigit is null) return;

        var decodedSegmentsForDigit = SegmentsByDigit[knownDigit.Value];

        AddMapping(encodedSet, decodedSegmentsForDigit);
    }

    private void AddMapping(string encodedSet, IReadOnlySet<char> decodedSet)
    {
        // Options for decoding should only be the known ones
        foreach (var encoded in encodedSet)
        {
            _mappingOptions[encoded].RemoveWhere(decoded => !decodedSet.Contains(decoded));
        }
        // Remove chars in encoded set from all other possible sets
        var otherEncoded = SegmentSet.Where(c => !encodedSet.Contains(c));
        foreach (var encoded in otherEncoded)
        {
            _mappingOptions[encoded].RemoveWhere(decodedSet.Contains);
        }

        var shouldCheckSingles = true;
        while (shouldCheckSingles)
        {
            shouldCheckSingles = CheckSingles();
        }
    }

    private bool CheckSingles()
    {
        var removed = 0;
        foreach (var (encoded, options) in _mappingOptions)
        {
            if (_singlesDone.Contains(encoded)) continue;
            if (options.Count > 1) continue;
            _singlesDone.Add(encoded);
            
            var decoded = options.Single();
            
            foreach (var otherOptions in _mappingOptions.Values)
            {
                if (otherOptions == options) continue;
                removed += otherOptions.Remove(decoded) ? 1 : 0;
            }
        }
        
        return removed > 0;
    }

    private void CheckByAppearanceCount()
    {
        foreach (var (encoded, count) in _countByEncoded)
        {
            switch (count)
            {
                case 4: // e occurs 4 times
                {
                    AddMapping(encoded.ToString(), "e".ToHashSet());
                    break;
                }
                case 6: // b occurs 6 times
                {
                    AddMapping(encoded.ToString(), "b".ToHashSet());
                    break;
                }
                case 9: // f occurs 9 times
                {
                    AddMapping(encoded.ToString(), "f".ToHashSet());
                    break;
                }
            }
        }
    }

    private static HashSet<char> GetNewOptionSet() => SegmentSet.ToHashSet();
}