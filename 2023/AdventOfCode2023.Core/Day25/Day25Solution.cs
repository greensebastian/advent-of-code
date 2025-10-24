using System.Text;

namespace AdventOfCode2023.Core.Day25;

public record Day25Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var ap = new Apparatus(Input.ToArray());
        var ans = ap.GroupSums();
        yield return ans.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return "0";
    }
}

public class Apparatus(IReadOnlyList<string> input)
{
    public IReadOnlyList<Connection> Connections { get; } = input.SelectMany(Connection.Connections).ToArray();

    public int GroupSums()
    {
        var gv = GraphViz();
        Console.WriteLine(gv);
        
        var severings = new Connection[] { new("bvz", "nvf"), new("cbl", "vmq"), new("klk", "xgz") };

        var cons = Connections.Where(c => !severings.Contains(c)).ToArray();
        var all = cons.SelectMany(c => new[] { c.Left, c.Right }).Distinct().ToHashSet();
        var seen = new HashSet<string>();
        var queue = new Queue<string>();
        queue.Enqueue(cons[0].Left);
        while (queue.TryDequeue(out var curr))
        {
            if (!seen.Add(curr)) continue;

            var next = cons.SelectMany(c => c.Left == curr ? [c.Right] : c.Right == curr ? new [] {c.Left} : []).ToArray();
            foreach (var s in next)
            {
                queue.Enqueue(s);
            }
        }
        
        return seen.Count * (all.Count - seen.Count);
    }

    public string GraphViz()
    {
        var sb = new StringBuilder();
        sb.AppendLine("graph G {");
        foreach (var connection in Connections)
        {
            sb.AppendLine($"    {connection.Left} -- {connection.Right}");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }
}

public record Connection(string Left, string Right)
{
    public static IEnumerable<Connection> Connections(string input)
    {
        var first = input.Split(':')[0];
        var others = input.Split(':')[1]
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        foreach (var second in others)
        {
            var left = string.Compare(first, second, StringComparison.InvariantCulture) < 0 ? first : second;
            var right = left == first ? second : first;
            yield return new Connection(left, right);
        }
    }
}