using System.Globalization;

namespace AdventOfCode2022.Core.Day03;

public record Day03Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var mismatches = new List<char>();
        
        foreach (var line in Input)
        {
            var leftItems = line[..(line.Length / 2)];
            var rightItems = line[(line.Length / 2)..];

            var encounteredLeft = new HashSet<char>();
            var encounteredRight = new HashSet<char>();
            char? dupe = null;
            for (var i = 0; dupe is null; i++)
            {
                var left = leftItems[i];
                var right = rightItems[i];

                encounteredLeft.Add(left);
                encounteredRight.Add(right);

                if (encounteredLeft.Contains(right))
                    dupe = right;
                if (encounteredRight.Contains(left))
                    dupe = left;
            }
            
            mismatches.Add(dupe.Value);
        }

        var itemsAsNumbers = mismatches.Select(c => char.IsUpper(c) ? c - 'A' + 27 : c - 'a' + 1).ToArray();
        
        yield return itemsAsNumbers.Sum().ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        var groupBadges = new List<char>();
        
        foreach (var group in Input.Batch(3))
        {
            groupBadges.Add(GetCommonChar(group));
        }

        var itemsAsNumbers = groupBadges.Select(c => char.IsUpper(c) ? c - 'A' + 27 : c - 'a' + 1).ToArray();
        
        yield return itemsAsNumbers.Sum().ToString();
    }

    private static char GetCommonChar(string[] backpacks)
    {
        var encountered = new HashSet<char>(backpacks[0]);

        var shared = new HashSet<char>();

        foreach (var item in backpacks[1])
        {
            if (encountered.Contains(item))
                shared.Add(item);
        }

        foreach (var item in backpacks[2])
        {
            if (shared.Contains(item))
                return item;
        }

        return '?';
    }
}

internal static class EnumerableExtensions
{
    public static IEnumerable<T[]> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
        var batch = new List<T>();
        foreach (var item in source)
        {
            batch.Add(item);
            if (batch.Count >= batchSize)
            {
                yield return batch.ToArray();
                batch.Clear();
            }
        }

        if (batch.Count > 0)
            yield return batch.ToArray();
    }
}