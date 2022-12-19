﻿namespace AdventOfCode2022.Core.Day19;

public record Day19Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var minutesToRun = int.Parse(args[0]);
        var scores = new List<int>();
        foreach (var line in Input)
        {
            var idx = line.Ints().ToArray()[0];
            var factory = new RobotFactory(RobotBlueprint.From(line));
            var statesToCheck = new List<GeodeSimulationState>
            {
                new(0, 0, 0, 0, 0, 1, 0, 0, 0)
            };
            while (statesToCheck.First().MinutesPassed < minutesToRun)
            {
                var newStates = new HashSet<GeodeSimulationState>();
                var maxValue = 0;
                foreach (var state in statesToCheck)
                {
                    foreach (var newState in state.GetNextRound(factory))
                    {
                        maxValue = newState.Value > maxValue ? newState.Value : maxValue;
                        newStates.Add(newState);
                    }
                }

                statesToCheck = newStates.OrderBy(s => s.Value).Take(2000).ToList();
            }

            var score = statesToCheck.Select(s => s.Geodes).Max();
            scores.Add(score * idx);
        }

        yield return scores.Max().ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return "0";
    }
}

public record struct GeodeSimulationState(int MinutesPassed, int Ore, int Clay, int Obsidian, int Geodes, int OreBots,
    int ClayBots, int ObsidianBots, int GeodeBots)
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

    public int Value => Obsidian + Geodes * 10;
    
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
                    Resource.Ore => this with
                    {
                        Ore = newOre,
                        Clay = newClay,
                        Obsidian = newObsidian,
                        OreBots = c.OreBots + 1
                    },
                    Resource.Clay => this with
                    {
                        Ore = newOre,
                        Clay = newClay,
                        Obsidian = newObsidian,
                        ClayBots = c.ClayBots + 1
                    },Resource.Obsidian => this with
                    {
                        Ore = newOre,
                        Clay = newClay,
                        Obsidian = newObsidian,
                        ObsidianBots = c.ObsidianBots + 1
                    },Resource.Geode => this with
                    {
                        Ore = newOre,
                        Clay = newClay,
                        Obsidian = newObsidian,
                        GeodeBots = c.GeodeBots + 1
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
}

public record Priority(int Ore, int Clay, int Obsidian, int Geode);

public class RobotFactory
{
    private Dictionary<Resource, RobotBlueprint> Blueprints { get; } = new();
    private static IReadOnlyList<Resource[]> PriorityOptions { get; }

    static RobotFactory()
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
    
    public RobotFactory(IEnumerable<RobotBlueprint> robotBlueprints)
    {
        foreach (var blueprint in robotBlueprints)
        {
            Blueprints[blueprint.Extracts] = blueprint;
        }
    }

    public IEnumerable<GeodeSimulationState> GetBuildOptions(GeodeSimulationState state)
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