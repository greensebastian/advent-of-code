using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day19 : ISolution
{
    private const string Example = """
                                   r, wr, b, g, bwu, rb, gb, br
                                   
                                   brwrr
                                   bggr
                                   gbbr
                                   rrbgbr
                                   ubwu
                                   bwurrg
                                   brgr
                                   bbrgwb
                                   """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day19");

        var result = new TowelSolution(input).CountPossibleSolutions();
        result.Should().Be(374);
    }

    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day19");

        var result = new TowelSolution(input).CountPossibleMutations();
        result.Should().Be(1100663950563322L);
        // 1098 too low
        // 1100663950563322L wrong
    }

    private class TowelSolution(string[] input)
    {
        private string[] _options = input.SplitByDivider(string.IsNullOrWhiteSpace).ToArray()[0].Single().Split(", ");
        private string[] _targets = input.SplitByDivider(string.IsNullOrWhiteSpace).ToArray()[1].ToArray();

        public int CountPossibleSolutions()
        {
            var sum = 0;
            foreach (var target in _targets)
            {
                var possible = new Node<string>("", null).DepthFirstSearch(node => _options.Select(o => $"{node.Value}{o}"), node => node.Value == target, node => !target.StartsWith(node.Value)).Any();
                if (possible) sum++;
            }

            return sum;
        }
        
        public long CountPossibleMutations()
        {
            var sum = 0L;
            foreach (var target in _targets)
            {
                var count = CountPossibleMutationsRecursive("", s => _options.Select(o => s + o), s => s == target, s => !target.StartsWith(s));
                sum += count;
            }

            return sum;
        }

        public long CountPossibleMutationsRecursive(string current, Func<string, IEnumerable<string>> findNeighbours, Func<string, bool> done, Func<string, bool> failed, Dictionary<string, long>? cache = null)
        {
            cache ??= new Dictionary<string, long>();
            if (cache.TryGetValue(current, out var cached)) return cached;
            if (failed(current)) return 0;
            if (done(current))
            {
                cache[current] = 1;
                return 1;
            }

            var val = 0L;
            foreach (var neighbour in findNeighbours(current))
            {
                var neighbourVal =
                    CountPossibleMutationsRecursive(neighbour, findNeighbours, done, failed, cache);
                val += neighbourVal;
            }
            
            cache[current] = val;
            return val;
        }
    }
    
    private long Solve(string[] input) => throw new NotImplementedException();
}