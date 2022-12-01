using System.Globalization;

namespace AdventOfCode2021.Core.Day01;

public record Day01Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var count = 0;
        var previousDepth = (long?)null;
        foreach (var line in Input)
        {
            var depth = long.Parse(line, CultureInfo.InvariantCulture);
            if (previousDepth < depth) count++;

            previousDepth = depth;
        }

        yield return count.ToString();
    }

    public override IEnumerable<string> SecondSolution()
    {
        var count = 0;
        var slidingSum = new Queue<long>();
        foreach (var line in Input)
        {
            var depth = long.Parse(line, CultureInfo.InvariantCulture);
            if (slidingSum.Count < 3)
            {
                slidingSum.Enqueue(depth);
                continue;
            }

            var previousSum = slidingSum.Sum();
            slidingSum.Dequeue();
            slidingSum.Enqueue(depth);
            var currentSum = slidingSum.Sum();

            if (currentSum > previousSum) count++;
        }

        yield return count.ToString();
    }
}