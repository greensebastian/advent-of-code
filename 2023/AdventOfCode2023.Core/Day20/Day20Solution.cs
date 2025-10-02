using System.Text;

namespace AdventOfCode2023.Core.Day20;

public record Day20Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var set = new ModuleSet(Input.ToArray());
        var ans = set.CountPulses(1000);
        
        yield return ans.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var set = new ModuleSet(Input.ToArray());
        var ans = set.CountPulsesToRx();
        
        yield return ans.ToString();
    }
}

public class ModuleSet(IReadOnlyList<string> input)
{
    public long CountPulses(int presses)
    {
        var lowPulses = 0L;
        var highPulses = 0L;
        //var pulseLog = new List<Pulse>();
        for (var i = 0; i < presses; i++)
        {
            var pulseQueue = new Queue<Pulse>();
            pulseQueue.Enqueue(new Pulse("", "button", false));
            lowPulses--;

            while (pulseQueue.TryDequeue(out var pulse))
            {
                //pulseLog.Add(pulse);
                if (pulse.IsHigh) highPulses++;
                else lowPulses++;
                if (!Modules.TryGetValue(pulse.Target, out var module)) continue;
                foreach (var newPulse in module.Pulse(pulse))
                {
                    pulseQueue.Enqueue(newPulse);
                }
            }
        }

        return lowPulses * highPulses;
    }
    
    public long CountPulsesToRx()
    {
        // LCM of cycles leading to high on xn
        return Util.LowestCommonMultiple(3769, Util.LowestCommonMultiple(3847, Util.LowestCommonMultiple(3877, 4057)));
        var graphViz = PrintGraphViz();
        var presses = 0;
        var log = new List<(int, Pulse)>();
        while (true)
        {
            var pulseQueue = new Queue<Pulse>();
            pulseQueue.Enqueue(new Pulse("", "button", false));
            presses++;
            var lOrdered = log.GroupBy(l => l.Item2.Source).Select(g => new {g.Key, Items = g.ToArray()});

            while (pulseQueue.TryDequeue(out var pulse))
            {
                if (pulse is { Target: "xn", IsHigh: true }) log.Add((presses, pulse));
                
                if (!Modules.TryGetValue(pulse.Target, out var module)) continue;
                foreach (var newPulse in module.Pulse(pulse))
                {
                    pulseQueue.Enqueue(newPulse);
                }
            }
        }
    }

    private static string PrintLog(IEnumerable<Pulse> log)
    {
        var low = new Dictionary<string, long>();
        var high = new Dictionary<string, long>();
        foreach (var pulse in log)
        {
            low.TryAdd(pulse.Target, 0);
            high.TryAdd(pulse.Target, 0);

            if (pulse.IsHigh) high[pulse.Target]++;
            else low[pulse.Target]++;
        }

        return string.Join('\n',
            low.OrderBy(l => l.Value).Select(kv => $"{kv.Key} Low: {low[kv.Key]} High: {high[kv.Key]}"));
    }

    private string PrintGraphViz()
    {
        var sb = new StringBuilder();
        sb.AppendLine("digraph G {");

        foreach (var module in Modules.OrderBy(m => m.Key))
        {
            foreach (var target in module.Value.Targets)
            {
                var op = Modules.TryGetValue(target, out var res) ? res.Definition[0] : '?';
                sb.AppendIndented($"{module.Key} -> {target} [headlabel=\"{op}\"]", 4);
            }
        }
        

        sb.AppendLine("}");
        return sb.ToString();
    }
    
    private Module Button => Modules["button"];
    
    private IReadOnlyDictionary<string, Module> Modules { get; } =
        Module.FromDefinitions(input).ToDictionary(m => m.Id);
}

public abstract class Module(string definition)
{
    public string Definition { get; } = definition;
    public string Id { get; } = GetId(definition);
    public IReadOnlyList<string> Targets { get; } = GetTargets(definition);
    
    public abstract IEnumerable<Pulse> Pulse(Pulse pulse);

    private static string GetId(string definition) => definition.Split("->")[0].Trim().Replace("%", "").Replace("&", "");
    private static IReadOnlyList<string> GetTargets(string definition) => definition.Split("->")[1].Replace(",", "").Trim().Split(" ");
    
    public static IEnumerable<Module> FromDefinitions(IReadOnlyList<string> definitions)
    {
        var remainingDefinitions = definitions.ToList();
        var modules = new List<Module>
        {
            new Button()
        };
        while (remainingDefinitions.Any())
        {
            foreach (var definition in remainingDefinitions.ToArray())
            {
                if (definition.StartsWith("%"))
                {
                    modules.Add(new FlipFlop(definition));
                    remainingDefinitions.Remove(definition);
                }

                if (definition.StartsWith("broadcaster"))
                {
                    modules.Add(new Broadcaster(definition));
                    remainingDefinitions.Remove(definition);
                }

                if (definition.StartsWith("&"))
                {
                    if (remainingDefinitions.Any(def => GetTargets(def).Contains(GetId(definition))))
                    {
                        continue;
                    }

                    var sources = modules.Where(m => m.Targets.Contains(GetId(definition))).Select(m => m.Id).ToArray();
                    modules.Add(new Conjunction(definition, sources));
                    remainingDefinitions.Remove(definition);
                }
            }
        }

        return modules;
    }

    public string Print() =>
        string.Join(" ", (this as Conjunction)!.Memory.OrderBy(kv => kv.Key).Select(kv => kv.Value));
}

public record struct Pulse(string Source, string Target, bool IsHigh)
{
    private string HighLowString() => IsHigh ? "high" : "low";
    public override string ToString() => $"{Source} -{HighLowString()}-> {Target}";
}

public class FlipFlop(string definition) : Module(definition)
{
    private bool On { get; set; } = false;
    public override IEnumerable<Pulse> Pulse(Pulse pulse)
    {
        if (pulse.IsHigh) yield break;
        On = !On;
        foreach (var target in Targets)
        {
            yield return new Pulse( Id, target, On);
        }
    }
}

public class Conjunction(string definition, IEnumerable<string> inputs) : Module(definition)
{
    public Dictionary<string, bool> Memory { get; } = inputs.ToDictionary(i => i, i => false);
    public override IEnumerable<Pulse> Pulse(Pulse pulse)
    {
        Memory[pulse.Source] = pulse.IsHigh;
        var sendHigh = Memory.Values.Any(v => !v);
        foreach (var target in Targets)
        {
            yield return new Pulse(Id, target, sendHigh);
        }
    }
}

public class Broadcaster(string definition) : Module(definition)
{
    public override IEnumerable<Pulse> Pulse(Pulse pulse)
    {
        foreach (var target in Targets)
        {
            yield return new Pulse(Id, target, pulse.IsHigh);
        }
    }
}

public class Button() : Module("button -> broadcaster")
{
    public override IEnumerable<Pulse> Pulse(Pulse pulse)
    {
        yield return new Pulse(Id, "broadcaster", false);
    }
}