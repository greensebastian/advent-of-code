namespace AdventOfCode2022.Core.Day16;

public record Day16Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var valveSystem = new ValveSystem(Input);

        var connections = ValveDijkstra.GetQuickestConnections(valveSystem).ToList();

        yield return "0";
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return "0";
    }
}

public class ValveSystem
{
    public Dictionary<string, Valve> AllValves { get; } = new();

    public ValveSystem(IEnumerable<string> input)
    {
        foreach (var line in input)
        {
            var valve = Valve.FromInput(line);
            AllValves.Add(valve.Name, valve);
        }
    }
}

public record Valve(string Name, int FlowRate, ISet<string> LeadsTo)
{
    public static Valve FromInput(string input)
    {
        var names = input
            .Split(" ")
            .Select(word => word.Trim(','))
            .Where(word => word.Length == 2 && word.All(char.IsUpper))
            .ToArray();

        var flowRate = input.Ints().Single();

        var valve = new Valve(names[0], flowRate, new HashSet<string>(names[1..]));

        return valve;
    }
}

public static class ValveDijkstra
{
    public static IEnumerable<ValveLink> GetQuickestConnections(ValveSystem system)
    {
        var connectionsToFind = system.AllValves.Keys.SelectMany(node =>
            system.AllValves.Keys.Where(n => n != node).Select(otherNode => (Start: node, End: otherNode)));

        foreach (var connection in connectionsToFind)
        {
            var unvisited = system.AllValves.Select(v => new DijkstraNode
            {
                Name = v.Key
            }).ToDictionary(n => n.Name);

            var visited = new Dictionary<string, DijkstraNode>();

            var end = unvisited[connection.End];
            var start = unvisited[connection.Start];
            start.Distance = 0;
            start.Path = new[] { start.Name };

            var current = start;
            while (current is not null && unvisited.ContainsKey(end.Name) && unvisited.Values.Any(node => node.Distance != int.MaxValue))
            {
                var reachable = system.AllValves[current.Name].LeadsTo
                    .Where(con => unvisited.ContainsKey(con))
                    .Select(con => unvisited[con]);
                foreach (var unvisitedNeighbour in reachable)
                {
                    if (unvisitedNeighbour.Distance > current.Distance + 1)
                    {
                        unvisitedNeighbour.Distance = current.Distance + 1;
                        unvisitedNeighbour.Path = current.Path.Concat(new[] { unvisitedNeighbour.Name }).ToArray();
                    }
                }

                unvisited.Remove(current.Name);
                visited.Add(current.Name, current);
                current = unvisited.Values.OrderBy(node => node.Distance).FirstOrDefault();
            }

            yield return new ValveLink(connection.Start, connection.End, visited[connection.End].Distance,
                visited[connection.End].Path);
        }
    }
}

public class DijkstraNode
{
    public required string Name { get; init; }
    public int Distance { get; set; } = int.MaxValue;
    public string[] Path { get; set; } = Array.Empty<string>();
}

public record struct ValveLink(string Start, string End, int Distance, string[] Path);

internal static class EnumerableExtensions
{
    public static IEnumerable<int> Ints(this IEnumerable<char> source)
    {
        var currentNumber = string.Empty;
        foreach (var c in source)
        {
            if (char.IsNumber(c) || c == '-')
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