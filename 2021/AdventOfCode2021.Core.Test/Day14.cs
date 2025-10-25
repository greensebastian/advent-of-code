using Shouldly;

namespace AdventOfCode2021.Core.Test;

public class Day14
{
    private const string Example = """
                                   NNCB
                                   
                                   CH -> B
                                   HH -> N
                                   CB -> H
                                   NH -> C
                                   HB -> C
                                   HC -> B
                                   HN -> C
                                   NN -> C
                                   BH -> H
                                   NC -> B
                                   NB -> B
                                   BN -> B
                                   BB -> N
                                   BC -> B
                                   CC -> N
                                   CN -> C
                                   """;
    
    private const string SimpleExample = """
                                   NNCB

                                   NN -> N
                                   CB -> F
                                   """;
    
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    public void Day14_1_Simple(int rounds, int expected)
    {
        var input = SimpleExample.Split('\n');
        var pc = new PolymerChain(input);
        var len = pc.BiggestSubSmallest(rounds);
        len.ShouldBe(expected);
    }
    
    [Fact]
    public void Day14_1_Example()
    {
        var input = Example.Split('\n');
        var pc = new PolymerChain(input);
        var len = pc.BiggestSubSmallest(10);
        len.ShouldBe(1588);
    }
    
    [Fact]
    public void Day14_1_Real()
    {
        var input = Util.ReadFile("Day14");
        var pc = new PolymerChain(input);
        var len = pc.BiggestSubSmallest(10);
        len.ShouldBe(3411L);
    }
    
    [Fact]
    public void Day14_2_Real()
    {
        var input = Util.ReadFile("Day14");
        var pc = new PolymerChain(input);
        var len = pc.BiggestSubSmallest(40);
        len.ShouldBe(7477815755570L);
    }
}

public class PolymerChain(IReadOnlyList<string> input)
{
    private const int ResLength = 28;
    public string Template { get; } = input[0];

    public Dictionary<string, char> InsertionRules { get; } =
        input.Skip(2).ToDictionary(l => l.Split(" -> ")[0], l => l.Split(" -> ")[1][0]);

    public long BiggestSubSmallest(int rounds)
    {
        var res = new long[ResLength];
        for (var i = 0; i < Template.Length - 1; i++)
        {
            var ab = Template.Substring(i, 2);
            var subRes = Counts(ab, rounds, new Dictionary<(string, long), long[]>());
            for (var j = 0; j < subRes.Length; j++)
            {
                res[j] += subRes[j];
            }

            if (i != 0) res[ab[0] - 'A']--; // Avoid double counting
        }

        var countByChar = res.Select((c, i) => (Character: (char)('A' + i), Count: c))
            .ToDictionary(p => p.Character, p => p.Count);
        var min = countByChar.Where(p => p.Value != 0).MinBy(p => p.Value);
        var max = countByChar.Where(p => p.Value != 0).MaxBy(p => p.Value);

        return max.Value - min.Value;
    }

    public long[] Counts(string ab, int depth, Dictionary<(string, long), long[]> cache)
    {
        var key = (ab, depth);
        if (cache.TryGetValue(key, out var cached)) return cached;
        
        var res = new long[ResLength];
        var insertionExists = InsertionRules.TryGetValue(ab, out var mid);
        if (depth == 0 || !insertionExists)
        {
            res[ab[0] - 'A']++;
            res[ab[1] - 'A']++;
            cache[key] = res;
            return res;
        }

        var left = Counts($"{ab[0]}{mid}", depth - 1, cache);
        var right = Counts($"{mid}{ab[1]}", depth - 1, cache);
        for (var i = 0; i < res.Length; i++)
        {
            res[i] = left[i] + right[i];
        }

        res[mid - 'A']--; // Avoid double count
        cache[key] = res;
        return res;
    }
}