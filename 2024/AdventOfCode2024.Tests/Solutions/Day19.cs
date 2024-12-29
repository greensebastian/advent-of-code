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
        var input = Util.ReadRaw(Example);
        //var input = Util.ReadFile("day18");

        var result = Solve(input);
        result.Should().Be(454);
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
                var possible = new Node<string>("", null).DepthFirstSearch(0, node => _options.Select(o => $"{node.Value}{o}"), _ => 1, node => node.Value == target, (node, l) => !target.StartsWith(node.Value)).Any();
                if (possible) sum++;
            }

            return sum;
        }
    }
    
    private long Solve(string[] input) => throw new NotImplementedException();
}