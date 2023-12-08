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
        var map = Map.FromInput(Input.ToArray());
        yield return map.LCM().ToString();
        //yield return map.CountSimultaneousSteps("A", "Z").ToString();
    }
}

public record Map(string Instructions, IReadOnlyDictionary<string, Node> NodesBySource)
{
    public int CountSteps(string start, string end, int startCount = 0)
    {
        var c = startCount;
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

    public long LCM()
    {
        var nodes = NodesBySource.Values.Where(n => n.Source.EndsWith("A")).ToArray();
        var cycles = nodes.Select(n => FindCycle(n.Source)).ToArray();

        var ans = cycles.Aggregate(1L, (lcm, cycle) => Util.LowestCommonMultiple(lcm, cycle.Length));
        return ans;
    }

    public long CountBrute(string start, string end)
    {
        var treeNodes = NodesBySource.Keys.Select(id => new TreeNode(id)).ToArray();
        foreach (var node in treeNodes)
        {
            node.Left = treeNodes.Single(n => n.Value == NodesBySource[node.Value].Left);
            node.Right = treeNodes.Single(n => n.Value == NodesBySource[node.Value].Right);
        }
        var nodes = treeNodes.Where(n => n.Value.EndsWith(start)).ToArray();

        var c = 0;
        while (true)
        {
            var left = Instructions[c % Instructions.Length] == 'L';
            if (nodes.All(n => n.Value.EndsWith(end))) return c;
            for (var i = 0; i < nodes.Length; i++)
            {
                nodes[i] = left ? nodes[i].Left : nodes[i].Right;
            }

            c++;
        }
    }
    
    // Also works, just slower :(
    public long CountSimultaneousSteps(string start, string end)
    {
        var nodes = NodesBySource.Values.Where(n => n.Source.EndsWith(start)).ToArray();
        var cycles = nodes.Select(n => FindCycle(n.Source)).ToArray();

        var exitsInCycles = cycles.Select(c => FindExitsInCycle(c, end)).ToArray();

        var mult = 0L;
        while (true)
        {
            long potential = cycles[0].SourceToStart + cycles[0].Length * mult + exitsInCycles[0].Exits[0];
            var overlapping = exitsInCycles.All(e =>
            {
                foreach (var exit in e.Exits)
                {
                    var startOfCycle = potential - exit;
                    var remainder = startOfCycle % e.Cycle.Length;
                    var equalsToStart = remainder - e.Cycle.SourceToStart;
                    if (equalsToStart == 0) return true;
                }

                return false;
            });
            if (overlapping)
            {
                return potential;
            }

            mult++;
        }
    }

    private CycleExits FindExitsInCycle(Cycle cycle, string end)
    {
        var start = cycle.Start;
        var exits = new List<int>();
        var c = start.Iteration + 1;
        var seen = new HashSet<NodeAndIter> { start };
        var node = NodesBySource[start.Node];
        var action = Instructions[start.Iteration];
        var nextNode = action == 'L' ? node.Left : node.Right;
        node = NodesBySource[nextNode];
        var nodeAndIter = new NodeAndIter(node.Source, c % Instructions.Length);
        while (!seen.Contains(nodeAndIter))
        {
            seen.Add(nodeAndIter);
            if (node.Source.EndsWith(end)) exits.Add(c - start.Iteration);
            action = Instructions[c % Instructions.Length];
            nextNode = action == 'L' ? node.Left : node.Right;
            node = NodesBySource[nextNode];
            c++;
            nodeAndIter = new NodeAndIter(node.Source, c % Instructions.Length);
        }

        return new CycleExits(cycle, exits.ToArray());
    }
    
    private Cycle FindCycle(string start)
    {
        var c = 1;
        var seen = new HashSet<NodeAndIter> { new(start, 0) };
        var node = NodesBySource[start];
        var action = Instructions[0];
        var nextNode = action == 'L' ? node.Left : node.Right;
        node = NodesBySource[nextNode];
        var nodeAndIter = new NodeAndIter(node.Source, c % Instructions.Length);
        while (!seen.Contains(nodeAndIter))
        {
            seen.Add(nodeAndIter);
            action = Instructions[c % Instructions.Length];
            nextNode = action == 'L' ? node.Left : node.Right;
            node = NodesBySource[nextNode];
            c++;
            nodeAndIter = new NodeAndIter(node.Source, c % Instructions.Length);
        }
        
        var toStart = CountSteps(start, nodeAndIter.Node);
        var cycleLength = c - toStart;

        return new Cycle(start, nodeAndIter, toStart, cycleLength);
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

public class TreeNode(string value)
{
    public string Value { get; } = value;
    public TreeNode Left { get; set; }
    
    public TreeNode Right { get; set; }
}

public record CycleExits(Cycle Cycle, int[] Exits);

public record Cycle(string Source, NodeAndIter Start, int SourceToStart, int Length);

public record NodeAndIter(string Node, int Iteration);

public record Node(string Source, string Left, string Right)
{
    public static Node FromInput(string line)
    {
        return new Node(line.Substring(0, 3), line.Substring(7, 3), line.Substring(12, 3));
    }
}