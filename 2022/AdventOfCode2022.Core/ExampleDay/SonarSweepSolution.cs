using System.Globalization;

namespace AdventOfCode2022.Core.ExampleDay;

public record SonarSweepSolution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> Solve()
    {
        var count = 0;
        var previousDepth = (ulong?)null;
        foreach (var line in Input)
        {
            var depth = ulong.Parse(line, CultureInfo.InvariantCulture);
            if (previousDepth < depth) count++;

            previousDepth = depth;
        }

        yield return count.ToString();
    }
}