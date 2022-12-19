using System.Collections.Concurrent;
using System.Diagnostics;

namespace AdventOfCode2022.Core.Day19;

public record Day19Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var minutesToRun = int.Parse(args[0]);
        var filterAmount = int.Parse(args[1]);
        var scores = new ConcurrentQueue<int>();
        var lines = Input.ToArray();

        Parallel.ForEach(lines, new ParallelOptions { MaxDegreeOfParallelism = 4 }, line =>
        {
            //SimBlueprint(line, minutesToRun, filterAmount, scores);
        });
        
        foreach (var line in lines)
        {
            SimBlueprint(line, minutesToRun, filterAmount, scores);
        }

        yield return scores.Sum().ToString();
    }

    private static void SimBlueprint(string line, int minutesToRun, int filterAmount, ConcurrentQueue<int> scores)
    {
        var idx = line.Ints().ToArray()[0];
        Console.WriteLine($"Starting blueprint {idx}");
        var sw = Stopwatch.StartNew();
        var factory = new RobotFactory(RobotBlueprint.From(line), minutesToRun);
        var statesToCheck = new Dictionary<GeodeSimulationState, int>
        {
            { new GeodeSimulationState(0, 0, 0, 0, 1, 0, 0, 0), 0 }
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

            statesToCheck = newStates.OrderByDescending(s => s.Key.Value)
                .Take(filterAmount).ToDictionary(s => s.Key, s => s.Value);
        }

        var orderedStates = done.OrderByDescending(s => s.Geodes).ToArray();
        var score = orderedStates.First().Geodes;
        scores.Enqueue(score * idx);
        var elapsed = sw.Elapsed;
        sw.Stop();
        Console.WriteLine($"Finished blueprint {idx} in {elapsed} with {score} points ({score * idx})");
    }

    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return "0";
    }
}

public record struct GeodeSimulationState(int Ore, int Clay, int Obsidian, int Geodes, int OreBots,
    int ClayBots, int ObsidianBots, int GeodeBots)
{
    public IEnumerable<(GeodeSimulationState, int)> GetNextRound(RobotFactory factory, int minutesPassed)
    {
        var nextMinutesPassed = minutesPassed + 1;

        var buildOptions = factory.GetBuildOptions(this).ToHashSet();

        foreach (var state in buildOptions.Prepend(this))
        {
            yield return (state with
            {
                Ore = state.Ore + OreBots,
                Clay = state.Clay + ClayBots,
                Obsidian = state.Obsidian + ObsidianBots,
                Geodes = state.Geodes + GeodeBots
            }, nextMinutesPassed);
        }
    }

    private int UpdateTimeUntilCanCreate(int oldMax, int current, int cost, int gain)
    {
        if (cost == 0) return oldMax;

        var time = Math.Ceiling(((decimal)cost - current) / gain) + 1;
        return time > oldMax ? (int)time : oldMax;
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
            
            // Can afford, find out when
            var minUntilCanCreate = int.MinValue;
            minUntilCanCreate = UpdateTimeUntilCanCreate(minUntilCanCreate, Ore, blueprint.OreCost, OreBots);
            minUntilCanCreate = UpdateTimeUntilCanCreate(minUntilCanCreate, Clay, blueprint.ClayCost, ClayBots);
            minUntilCanCreate = UpdateTimeUntilCanCreate(minUntilCanCreate, Obsidian, blueprint.ObsidianCost, ObsidianBots);

            var newMinutesPassed = minutesPassed + minUntilCanCreate;
            if (newMinutesPassed > factory.MaxMinutes)
                continue;

            var newOre = Ore + OreBots * minUntilCanCreate - blueprint.OreCost;
            var newClay = Clay + ClayBots * minUntilCanCreate - blueprint.ClayCost;
            var newObsidian = Obsidian + ObsidianBots * minUntilCanCreate - blueprint.ObsidianCost;
            var newGeodes = Geodes + GeodeBots * minUntilCanCreate;

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
    
    public IEnumerable<GeodeSimulationState> TryBuild(RobotBlueprint blueprint)
    {
        var c = this;
        while (true)
        {
            var newOre = c.Ore - blueprint.OreCost;
            var newClay = c.Clay - blueprint.ClayCost;
            var newObsidian = c.Obsidian - blueprint.ObsidianCost;

            if (newOre >= 0 && newClay >= 0 && newObsidian >= 0)
            {
                var next = blueprint.Extracts switch
                {
                    Resource.Ore => c with
                    {
                        Ore = newOre,
                        Clay = newClay,
                        Obsidian = newObsidian,
                        OreBots = c.OreBots + 1,
                        //BuildHistory = BuildHistory + $",Ore({c.MinutesPassed})"
                    },
                    Resource.Clay => c with
                    {
                        Ore = newOre,
                        Clay = newClay,
                        Obsidian = newObsidian,
                        ClayBots = c.ClayBots + 1,
                        //BuildHistory = BuildHistory + $",Clay({c.MinutesPassed})"
                    },Resource.Obsidian => c with
                    {
                        Ore = newOre,
                        Clay = newClay,
                        Obsidian = newObsidian,
                        ObsidianBots = c.ObsidianBots + 1,
                        //BuildHistory = BuildHistory + $",Obsidian({c.MinutesPassed})"
                    },Resource.Geode => c with
                    {
                        Ore = newOre,
                        Clay = newClay,
                        Obsidian = newObsidian,
                        GeodeBots = c.GeodeBots + 1,
                        //BuildHistory = BuildHistory + $",Geode({c.MinutesPassed})"
                    },
                    _ => throw new ArgumentOutOfRangeException()
                };

                yield return next;
                c = next;
            }
            else
            {
                yield break;
            }
        }
    }

    /*public bool IsExampleState =>
        "Ore(0),Clay(3),Clay(5),Clay(7),Obsidian(11),Clay(12),Obsidian(15),Geode(18),Geode(21),".StartsWith(
            BuildHistory);*/

    public override string ToString() =>
        $"Or,C,Ob,G [{OreBots}:{Ore},{ClayBots}:{Clay},{ObsidianBots}:{Obsidian},{GeodeBots}:{Geodes}]";
}

public record Priority(int Ore, int Clay, int Obsidian, int Geode);

public class RobotFactory
{
    public Dictionary<Resource, RobotBlueprint> Blueprints { get; } = new();
    private static IReadOnlyList<Resource[]> PriorityOptions { get; }
    public int MaxMinutes { get; }
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

    private static HashSet<Priority> GetAllPriorityPermutations()
    {
        var set = new HashSet<Priority>();
        const int resourceCount = 4;
        for (var ore = 0; ore < resourceCount; ore++)
        {
            for (var clay = 0; clay < resourceCount; clay++)
            {
                for (var obsidian = 0; obsidian < resourceCount; obsidian++)
                {
                    for (var geode = 0; geode < resourceCount; geode++)
                    {
                        set.Add(new Priority(ore, clay, obsidian, geode));
                    }
                }
            }
        }

        return set;
    }

    public RobotFactory(IEnumerable<RobotBlueprint> robotBlueprints, int maxMinutes)
    {
        MaxMinutes = maxMinutes;
        foreach (var blueprint in robotBlueprints)
        {
            Blueprints[blueprint.Extracts] = blueprint;
        }
    }

    public IEnumerable<GeodeSimulationState> GetBuildOptions(GeodeSimulationState state)
    {
        return GetBuildOne(state);
        //return GetBuildAll(state);
    }

    private IEnumerable<GeodeSimulationState> GetBuildOne(GeodeSimulationState state)
    {
        foreach (var order in PriorityOptions)
        {
            var newState = state.TryBuild(Blueprints[order[0]]).FirstOrDefault();
            if (newState != default)
                yield return newState;
        }
    }

    private IEnumerable<GeodeSimulationState> GetBuildAll(GeodeSimulationState state)
    {
        foreach (var order in PriorityOptions)
        {
            var knownGood = state;
            for (var i = 0; i < 4; i++)
            {
                var s = knownGood.TryBuild(Blueprints[order[i]]).LastOrDefault();
                if (s != default)
                {
                    knownGood = s;
                }
            }

            if (knownGood != state)
                yield return knownGood;
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