namespace AdventOfCode2022.Core.Day16;

public record Day16Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public const int NbrRounds = 30;
    
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var filterCount = int.Parse(args[0]);
        var valveSystem = new ValveSystem(Input);

        var connections = ValveDijkstra.GetQuickestConnections(valveSystem).ToList();

        var paths = new List<ValveSystemPath>();
        var done = new List<ValveSystemPath>();
        paths.Add(ValveSystemPath.New);
        while (paths.Count > 0)
        {
            Console.WriteLine($"PathCount: {paths.Count}, FirstTimeElapsed: {paths[0].TimeElapsed}");
            var newPaths = new List<ValveSystemPath>();
            foreach (var path in paths)
            {
                if (path.TimeElapsed >= NbrRounds)
                {
                    done.Add(path);
                    continue;
                }

                var scoreDelta = path.GetScoreForOneMinute(valveSystem);
                var valveLinksToCheck = connections
                    .Where(link => link.Value(NbrRounds - path.TimeElapsed, valveSystem) > 0)
                    .Where(link => link.Start == path.Current && !path.Opened.Contains(link.End))
                    .OrderByDescending(link => link.Value(NbrRounds - path.TimeElapsed, valveSystem))
                    .Take(filterCount)
                    .ToList();
                foreach (var link in valveLinksToCheck)
                {
                    if (path.TimeElapsed <= NbrRounds - 2)
                    {
                        var minutes = link.Distance + 1;
                        AddOrReplaceEquivalent(path with
                        {
                            Current = link.End,
                            Score = path.Score + minutes * scoreDelta,
                            TimeElapsed = path.TimeElapsed + minutes,
                            Opened = path.Opened.Concat(new []{ link.End }).ToArray(),
                            Route = path.Route + string.Join("", link.Path[1..]) + "_"
                        }, newPaths);
                    }
                    else
                    {
                        AddOrReplaceEquivalent(path with
                        {
                            Score = path.Score + scoreDelta,
                            TimeElapsed = path.TimeElapsed + 1
                        }, newPaths);
                    }
                }

                if (valveLinksToCheck.Count == 0)
                {
                    AddOrReplaceEquivalent(path with
                    {
                        Score = path.Score + scoreDelta,
                        TimeElapsed = path.TimeElapsed + 1
                    }, newPaths);
                }
            }

            paths = newPaths;
        }

        var best = done.OrderByDescending(p => p.Score).First();

        yield return done.Max(p => p.Score).ToString();
    }

    private static void AddOrReplaceEquivalent(ValveSystemPath toAdd, List<ValveSystemPath> allExisting)
    {
        allExisting.Add(toAdd);
        return;
        var exists = allExisting.Any(e => e.Equivalent(toAdd));
        if (!exists) allExisting.Add(toAdd);
        else
        {
            var existing = allExisting.Single(e => e.Equivalent(toAdd));
            if (existing.Score >= toAdd.Score) return;

            allExisting.Remove(existing);
            allExisting.Add(toAdd);
        }
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return "0";
    }
}

public record struct ValveLink(string Start, string End, int Distance, string[] Path)
{
    public int Value(int roundsLeft, ValveSystem system)
    {
        var availableRounds = roundsLeft - Distance - 1;
        if (availableRounds < 1) return 0;
        return system.AllValves[End].FlowRate * availableRounds;
    }
}

public record struct ValveSystemPath(int TimeElapsed, string[] Opened, string Current, int Score, string Route)
{
    public static ValveSystemPath New => new(0, Array.Empty<string>(), "AA", 0, "AA");
    
    public int GetScoreForOneMinute(ValveSystem system)
    {
        var roundPoints = 0;
        foreach (var open in Opened)
        {
            roundPoints += system.AllValves[open].FlowRate;
        }

        return roundPoints;
    }

    public bool Equivalent(ValveSystemPath other)
    {
        if (Current != other.Current) return false;
        if (TimeElapsed != other.TimeElapsed) return false;
        if (Opened.Length != other.Opened.Length) return false;
        var thisSet = string.Join("", Opened.Order());
        var otherSet = string.Join("", other.Opened.Order());
        return thisSet == otherSet;
    }

    public override string ToString()
    {
        return $"{Score} points after, {TimeElapsed} [{Route}]";
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