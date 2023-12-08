namespace AdventOfCode2023.Core.Day08;

public record Day08Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var map = Map.FromInput(Input.ToArray());
        yield return map.CountSteps("AAA", "ZZZ").ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return 0.ToString();
    }
}

public record Map(string Instructions, IReadOnlyDictionary<string, Node> NodesBySource)
{
    public int CountSteps(string start, string end)
    {
        var c = 0;
        var node = NodesBySource[start];
        while (node != NodesBySource[end])
        {
            var action = Instructions[c % Instructions.Length];
            var nextNode = action == 'L' ? node.Left : node.Right;
            node = NodesBySource[nextNode];
            c++;
        }

        return c;
    }
    
    public static Map FromInput(IList<string> lines)
    {
        var nodes = lines
            .Skip(2)
            .Select(Node.FromInput)
            .ToDictionary(n => n.Source)
            .AsReadOnly();

        return new Map(lines.First(), nodes);
    }
}

public record Node(string Source, string Left, string Right)
{
    public static Node FromInput(string line)
    {
        return new Node(line.Substring(0, 3), line.Substring(7, 3), line.Substring(12, 3));
    }
}