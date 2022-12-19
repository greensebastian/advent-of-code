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

        Parallel.ForEach(Input, line =>
        {
            //SimBlueprint(line, minutesToRun, filterAmount, scores);
        });
        
        foreach (var line in Input)
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
        var statesToCheck = new HashSet<GeodeSimulationState>
        {
            new(0, 0, 0, 0, 0, 1, 0, 0, 0, "Ore(0)")
        };
        var done = new HashSet<GeodeSimulationState>();
        while (statesToCheck.Any())
        {
            var newStates = new HashSet<GeodeSimulationState>();
            var maxValue = 0;
            foreach (var state in statesToCheck)
            {
                foreach (var newState in state.GetNextInterestingStates(factory))
                {
                    maxValue = newState.Value > maxValue ? newState.Value : maxValue;
                    if (newState.MinutesPassed == minutesToRun)
                    {
                        done.Add(newState);
                    }
                    else
                    {
                        newStates.Add(newState);
                    }
                }
            }

            statesToCheck = newStates.OrderByDescending(s => s.Value)
                .Take(filterAmount).ToHashSet();
        }

        var orderedStates = done.OrderByDescending(s => s.Value).ToArray();
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

public record struct GeodeSimulationState(int MinutesPassed, int Ore, int Clay, int Obsidian, int Geodes, int OreBots,
    int ClayBots, int ObsidianBots, int GeodeBots, string BuildHistory)
{
    public IEnumerable<GeodeSimulationState> GetNextRound(RobotFactory factory)
    {
        var nextMinutesPassed = MinutesPassed + 1;

        var buildOptions = factory.GetBuildOptions(this).ToHashSet();

        foreach (var state in buildOptions.Prepend(this))
        {
            yield return state with
            {
                MinutesPassed = nextMinutesPassed,
                Ore = state.Ore + OreBots,
                Clay = state.Clay + ClayBots,
                Obsidian = state.Obsidian + ObsidianBots,
                Geodes = state.Geodes + GeodeBots
            };
        }
    }

    public IEnumerable<GeodeSimulationState> GetNextInterestingStates(RobotFactory factory)
    {
        var minutesLeft = factory.MaxMinutes - MinutesPassed;
        yield return this with
        {
            MinutesPassed = MinutesPassed + minutesLeft,
            Ore = Ore + minutesLeft * OreBots,
            Clay = Clay + minutesLeft * ClayBots,
            Obsidian = Obsidian + minutesLeft * ObsidianBots,
            Geodes = Geodes + minutesLeft * GeodeBots
        };
        
        foreach (var (res, blueprint) in factory.Blueprints)
        {
            if (OreBots == 0 && blueprint.OreCost > 0) continue;
            if (ClayBots == 0 && blueprint.ClayCost > 0) continue;
            if (ObsidianBots == 0 && blueprint.ObsidianCost > 0) continue;
            
            // Can afford, find out when
            var maxTime = int.MinValue;
            if (blueprint.OreCost > 0)
            {
                var time = (blueprint.OreCost - Ore) / OreBots;
                maxTime = time > maxTime ? time : maxTime;
            }
            if (blueprint.ClayCost > 0)
            {
                var time = (blueprint.ClayCost - Clay) / ClayBots;
                maxTime = time > maxTime ? time : maxTime;
            }
            if (blueprint.ObsidianCost > 0)
            {
                var time = (blueprint.ObsidianCost - Obsidian) / ObsidianBots;
                maxTime = time > maxTime ? time : maxTime;
            }

            var newMinutesPassed = MinutesPassed + maxTime;
            if (newMinutesPassed > factory.MaxMinutes)
                continue;

            var newOre = Ore + OreBots * maxTime - blueprint.OreCost;
            var newClay = Clay + ClayBots * maxTime - blueprint.ClayCost;
            var newObsidian = Obsidian + ObsidianBots * maxTime - blueprint.ObsidianCost;
            var newGeodes = Geodes + GeodeBots * maxTime;

            yield return this with
            {
                OreBots = OreBots + (res == Resource.Ore ? 1 : 0),
                ClayBots = ClayBots + (res == Resource.Clay ? 1 : 0),
                ObsidianBots = ObsidianBots + (res == Resource.Obsidian ? 1 : 0),
                GeodeBots = GeodeBots + (res == Resource.Geode ? 1 : 0),
                MinutesPassed = newMinutesPassed,
                Ore = newOre,
                Clay = newClay,
                Obsidian = newObsidian,
                Geodes = newGeodes,
                BuildHistory = BuildHistory + $",{res.ToString()}({newMinutesPassed})"
            };
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
                        BuildHistory = BuildHistory + $",Ore({c.MinutesPassed})"
                    },
                    Resource.Clay => c with
                    {
                        Ore = newOre,
                        Clay = newClay,
                        Obsidian = newObsidian,
                        ClayBots = c.ClayBots + 1,
                        BuildHistory = BuildHistory + $",Clay({c.MinutesPassed})"
                    },Resource.Obsidian => c with
                    {
                        Ore = newOre,
                        Clay = newClay,
                        Obsidian = newObsidian,
                        ObsidianBots = c.ObsidianBots + 1,
                        BuildHistory = BuildHistory + $",Obsidian({c.MinutesPassed})"
                    },Resource.Geode => c with
                    {
                        Ore = newOre,
                        Clay = newClay,
                        Obsidian = newObsidian,
                        GeodeBots = c.GeodeBots + 1,
                        BuildHistory = BuildHistory + $",Geode({c.MinutesPassed})"
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

    public override string ToString() =>
        $"{MinutesPassed}: Or,C,Ob,G [{OreBots}:{Ore},{ClayBots}:{Clay},{ObsidianBots}:{Obsidian},{GeodeBots}:{Geodes}], {BuildHistory}";
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