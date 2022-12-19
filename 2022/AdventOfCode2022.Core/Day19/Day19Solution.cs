using System.Diagnostics;

namespace AdventOfCode2022.Core.Day19;

public record Day19Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var minutesToRun = int.Parse(args[0]);
        var filterAmount = args.Length > 1 ? int.Parse(args[1]) : int.MaxValue;
        var scores = new List<int>();
        var lines = Input.ToArray();

        foreach (var line in lines)
        {
            var (idx, score) = SimBlueprint(line, minutesToRun, filterAmount, false);
            scores.Add(idx * score);
        }

        yield return scores.Sum().ToString();
    }

    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var minutesToRun = int.Parse(args[0]);
        var filterAmount = args.Length > 1 ? int.Parse(args[1]) : int.MaxValue;
        var scores = new List<int>();
        var lines = Input.Take(3).ToArray();

        foreach (var line in lines)
        {
            var (idx, score) = SimBlueprint(line, minutesToRun, filterAmount, true);
            scores.Add(score);
        }

        var product = scores.Aggregate(1, (product, curr) => product * curr);
        
        yield return product.ToString();
    }
    
    private static (int Index, int GeodeCount) SimBlueprint(string line, int minutesToRun, int filterAmount, bool prioClayOverObsidian)
    {
        var idx = line.Ints().ToArray()[0];
        Console.WriteLine($"Starting blueprint {idx}");
        var sw = Stopwatch.StartNew();
        var factory = new RobotFactory(RobotBlueprint.From(line), minutesToRun, prioClayOverObsidian);
        var statesToCheck = new Dictionary<GeodeSimulationState, int>
        {
            { new GeodeSimulationState(1, 0, 0, 0, 1, 0, 0, 0), 1 }
        };
        var allStatesSeen = new Dictionary<GeodeSimulationState, int>();
        var done = new HashSet<GeodeSimulationState>();
        while (statesToCheck.Any())
        {
            var newStates = new Dictionary<GeodeSimulationState, int>();
            foreach (var state in statesToCheck.OrderBy(s => s.Value))
            {
                /*if (state.IsExampleState)
                {
                    //Console.WriteLine($"Saw example state after {state.MinutesPassed}");
                }*/
                foreach (var newState in state.Key.GetNextInterestingStates(factory, state.Value))
                {
                    if (newState.MinutesPassed == minutesToRun)
                    {
                        done.Add(newState.State);
                    }
                    else if (!allStatesSeen.TryGetValue(newState.State, out var bestMinutesPassed) || bestMinutesPassed > newState.MinutesPassed )
                    {
                        newStates[newState.State] = newState.MinutesPassed;
                        allStatesSeen[newState.State] = newState.MinutesPassed;
                    }
                }
            }
            
            Console.WriteLine($"Seen {allStatesSeen.Count} states");

            statesToCheck = newStates
                .OrderByDescending(s => s.Key.Value)
                .Take(filterAmount)
                .ToDictionary(s => s.Key, s => s.Value);
        }

        var orderedStates = done.OrderByDescending(s => s.Geodes).ToArray();
        var score = orderedStates.First().Geodes;
        var elapsed = sw.Elapsed;
        sw.Stop();
        Console.WriteLine($"Finished blueprint {idx} in {elapsed} with {score} points ({score * idx})");
        return (idx, score);
    }
}

public record struct GeodeSimulationState(int Ore, int Clay, int Obsidian, int Geodes, int OreBots,
    int ClayBots, int ObsidianBots, int GeodeBots)
{
    private int UpdateTimeUntilCreated(int oldMax, int current, int cost, int gain)
    {
        if (cost == 0) return oldMax;

        var time = Math.Ceiling(((decimal)cost - current) / gain) + 1;
        var roundTime = (int)Math.Round(time, MidpointRounding.AwayFromZero);
        return roundTime > oldMax ? roundTime : oldMax;
    }

    public IEnumerable<(GeodeSimulationState State, int MinutesPassed)> GetNextInterestingStates(RobotFactory factory, int minutesPassed)
    {
        var minutesLeft = factory.MaxMinutes - minutesPassed;
        yield return (this with
        {
            Ore = Ore + minutesLeft * OreBots,
            Clay = Clay + minutesLeft * ClayBots,
            Obsidian = Obsidian + minutesLeft * ObsidianBots,
            Geodes = Geodes + minutesLeft * GeodeBots
        }, minutesPassed + minutesLeft);

        foreach (var (res, blueprint) in factory.Blueprints)
        {
            if (OreBots == 0 && blueprint.OreCost > 0) continue;
            if (ClayBots == 0 && blueprint.ClayCost > 0) continue;
            if (ObsidianBots == 0 && blueprint.ObsidianCost > 0) continue;

            switch (res)
            {
                case Resource.Ore:
                    if (OreBots >= factory.MaxUsable[Resource.Ore]) continue;
                    if (ClayBots > 4) continue;
                    if (Ore > 4 * factory.MaxUsable[Resource.Ore]) continue;
                    break;
                case Resource.Clay:
                    if (ClayBots >= factory.MaxUsable[Resource.Clay]) continue;
                    if (ObsidianBots > 3) continue;
                    break;
                case Resource.Obsidian:
                    if (ObsidianBots >= factory.MaxUsable[Resource.Obsidian]) continue;
                    if (Obsidian > 1.5 * factory.MaxUsable[Resource.Obsidian]) continue;
                    break;
            }
            
            // Can afford, find out when
            var minUntilCreated = int.MinValue;
            minUntilCreated = UpdateTimeUntilCreated(minUntilCreated, Ore, blueprint.OreCost, OreBots);
            minUntilCreated = UpdateTimeUntilCreated(minUntilCreated, Clay, blueprint.ClayCost, ClayBots);
            minUntilCreated = UpdateTimeUntilCreated(minUntilCreated, Obsidian, blueprint.ObsidianCost, ObsidianBots);

            minUntilCreated = minUntilCreated <= 0 ? 1 : minUntilCreated;
            
            var newMinutesPassed = minutesPassed + minUntilCreated;
            if (newMinutesPassed > factory.MaxMinutes)
                continue;

            var newOre = Ore + OreBots * minUntilCreated - blueprint.OreCost;
            var newClay = Clay + ClayBots * minUntilCreated - blueprint.ClayCost;
            var newObsidian = Obsidian + ObsidianBots * minUntilCreated - blueprint.ObsidianCost;
            var newGeodes = Geodes + GeodeBots * minUntilCreated;

            yield return (this with
            {
                OreBots = OreBots + (res == Resource.Ore ? 1 : 0),
                ClayBots = ClayBots + (res == Resource.Clay ? 1 : 0),
                ObsidianBots = ObsidianBots + (res == Resource.Obsidian ? 1 : 0),
                GeodeBots = GeodeBots + (res == Resource.Geode ? 1 : 0),
                Ore = newOre,
                Clay = newClay,
                Obsidian = newObsidian,
                Geodes = newGeodes,
                //BuildHistory = BuildHistory + $",{res.ToString()}({newMinutesPassed})"
            }, newMinutesPassed);
        }
    }

    private static int[] Weights { get; } = { 1, 1, 10, 100, 1, 1, 10, 100 };

    public int Value => Ore * Weights[0] 
                        + Clay * Weights[1]
                        + Obsidian * Weights[2]
                        + Geodes * Weights[3]
                        + OreBots * Weights[4]
                        + ClayBots * Weights[5]
                        + ObsidianBots * Weights[6]
                        + GeodeBots * Weights[7];

    public override string ToString() =>
        $"Or,C,Ob,G [{OreBots}:{Ore},{ClayBots}:{Clay},{ObsidianBots}:{Obsidian},{GeodeBots}:{Geodes}]";
}

public record Priority(int Ore, int Clay, int Obsidian, int Geode);

public class RobotFactory
{
    public Dictionary<Resource, RobotBlueprint> Blueprints { get; } = new();

    public Dictionary<Resource, int> MaxUsable { get; } = new()
    {
        { Resource.Ore, 0 },
        { Resource.Clay, 0 },
        { Resource.Obsidian, 0 },
        { Resource.Geode, int.MaxValue }
    };
    private static IReadOnlyList<Resource[]> PriorityOptions { get; }
    public int MaxMinutes { get; }
    public bool PrioClayOverObsidian { get; }

    static RobotFactory()
    {
        //var set = GetAllPriorityPermutations();
        var set = GetEachPrioritizedOnce();

        PriorityOptions = set.Select(s =>
        {
            var order = new Resource[4];
            order[s.Ore] = Resource.Ore;
            order[s.Clay] = Resource.Clay;
            order[s.Obsidian] = Resource.Obsidian;
            order[s.Geode] = Resource.Geode;
            return order;
        }).ToList();
    }

    private static HashSet<Priority> GetEachPrioritizedOnce()
    {
        return new HashSet<Priority>
        {
            new(0, 1, 2, 3),
            new(1, 2, 3, 0),
            new(2, 3, 0, 1),
            new(3, 0, 1, 2)
        };
    }

    public RobotFactory(IEnumerable<RobotBlueprint> robotBlueprints, int maxMinutes, bool prioClayOverObsidian)
    {
        MaxMinutes = maxMinutes;
        PrioClayOverObsidian = prioClayOverObsidian;
        foreach (var blueprint in robotBlueprints)
        {
            Blueprints[blueprint.Extracts] = blueprint;
            MaxUsable[Resource.Ore] = Math.Max(MaxUsable[Resource.Ore], blueprint.OreCost);
            MaxUsable[Resource.Clay] = Math.Max(MaxUsable[Resource.Clay], blueprint.ClayCost);
            MaxUsable[Resource.Obsidian] = Math.Max(MaxUsable[Resource.Obsidian], blueprint.ObsidianCost);
        }
    }
}

public record RobotBlueprint(Resource Extracts, int OreCost, int ClayCost, int ObsidianCost)
{
    public static IEnumerable<RobotBlueprint> From(string input)
    {
        var ints = input.Ints().ToArray();
        yield return new RobotBlueprint(Resource.Ore, ints[1], 0, 0);
        yield return new RobotBlueprint(Resource.Clay, ints[2], 0, 0);
        yield return new RobotBlueprint(Resource.Obsidian, ints[3], ints[4], 0);
        yield return new RobotBlueprint(Resource.Geode, ints[5], 0, ints[6]);
    }
}

public enum Resource
{
    Ore,
    Clay,
    Obsidian,
    Geode
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