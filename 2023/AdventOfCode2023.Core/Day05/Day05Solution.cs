namespace AdventOfCode2023.Core.Day05;

public record Day05Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var stacks = new Dictionary<int, LinkedList<char>>();

        var inSetup = true;
        foreach (var line in Input)
        {
            // Parse stack lines
            if (line.Contains('['))
            {
                HandleSetupLine(line, stacks);
                continue;
            }
            
            // Separator for setup
            if (string.IsNullOrEmpty(line))
            {
                inSetup = false;
                continue;
            }

            if (inSetup) continue;
            
            // Handle move instructions
            var instructions = line.Ints().ToArray();
            for (var count = 0; count < instructions[0]; count++)
            {
                var from = stacks[instructions[1]];
                var to = stacks[instructions[2]];
                var c = from.First!.Value;
                from.RemoveFirst();
                to.AddFirst(c);
            }
        }

        yield return string.Join("", stacks
            .OrderBy(stack => stack.Key)
            .Select(stack => stack.Value.First?.Value)
            .Where(c => c is not null));
    }

    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var stacks = new Dictionary<int, LinkedList<char>>();

        var inSetup = true;
        foreach (var line in Input)
        {
            // Parse stack lines
            if (line.Contains('['))
            {
                HandleSetupLine(line, stacks);
                continue;
            }
            
            // Separator for setup
            if (string.IsNullOrEmpty(line))
            {
                inSetup = false;
                continue;
            }

            if (inSetup) continue;
            
            // Handle move instructions
            var instructions = line.Ints().ToArray();
            var count = instructions[0];
            var from = stacks[instructions[1]];
            var to = stacks[instructions[2]];
                
            var chars = from.Take(count).ToArray();
            for (var c = chars.Length - 1; c >= 0; c--)
            {
                from.RemoveFirst();
                to.AddFirst(chars[c]);
            }
        }
        
        yield return string.Join("", stacks
            .OrderBy(stack => stack.Key)
            .Select(stack => stack.Value.First?.Value)
            .Where(c => c is not null));
    }

    private static void HandleSetupLine(string line, IDictionary<int, LinkedList<char>> stacks)
    {
        var batched = line
            .Batch(4)
            .Select(b => string.Join("", b))
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();
        for (var batchIndex = 0; batchIndex < batched.Count; batchIndex++)
        {
            var stackIndex = batchIndex + 1;
            if (!stacks.ContainsKey(stackIndex))
                stacks[stackIndex] = new LinkedList<char>();

            var text = batched[batchIndex];
            if (!string.IsNullOrWhiteSpace(text))
                stacks[stackIndex].AddLast(text[1]);
        }
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

    public static IEnumerable<int> Ints(this IEnumerable<char> source)
    {
        var currentNumber = string.Empty;
        foreach (var c in source)
        {
            if (char.IsNumber(c))
            {
                currentNumber += c;
            }
            else
            {
                if (string.IsNullOrEmpty(currentNumber)) continue;
                
                yield return int.Parse(currentNumber);
                currentNumber = string.Empty;
            }
        }

        if (currentNumber.Length > 0) yield return int.Parse(currentNumber);
    }
}