namespace AdventOfCode2022.Core.Day06;

public record Day06Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        // Find 4 different characters in a row
        foreach (var line in Input)
        {
            var processed = GetCountBeforeUniqueSequence(line, 4);

            yield return processed.ToString();
        }
    }

    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        // Find 14 different characters in a row
        foreach (var line in Input)
        {
            var processed = GetCountBeforeUniqueSequence(line, 14);

            yield return processed.ToString();
        }
    }
    
    private static int GetCountBeforeUniqueSequence(string line, int requiredUnique)
    {
        var processed = 0;
        var foundSequence = false;
        var data = new LinkedList<char>(line);
        var nextNode = data.First;
        while (nextNode is not null && !foundSequence)
        {
            processed++;
            var current = nextNode;
            nextNode = current.Next;
            var encountered = new HashSet<char> { current.Value };
            // Walk backwards to check uniqueness
            for (var i = 1; i <= requiredUnique; i++)
            {
                current = current.Previous;
                if (current is null) break;
                if (!encountered.Add(current.Value)) break;
            }

            if (encountered.Count == requiredUnique)
                foundSequence = true;
        }

        return processed;
    }
}