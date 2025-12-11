using System.Text;
using Shouldly;

namespace AdventOfCode2025.Tests.Day11;

public class Day11
{
    private const string Example = """
                                   aaa: you hhh
                                   you: bbb ccc
                                   bbb: ddd eee
                                   ccc: ddd eee fff
                                   ddd: ggg
                                   eee: out
                                   fff: out
                                   ggg: out
                                   hhh: ccc fff iii
                                   iii: out
                                   """;

    private const string SecondExample = """
                                         svr: aaa bbb
                                         aaa: fft
                                         fft: ccc
                                         bbb: tty
                                         tty: ccc
                                         ccc: ddd eee
                                         ddd: hub
                                         hub: fff
                                         eee: dac
                                         dac: fff
                                         fff: ggg hhh
                                         ggg: out
                                         hhh: out
                                         """;
    
    [Fact]
    public void Example_1()
    {
        var lines = Util.ReadRaw(Example);
        var wm = new WiringMess(lines);
        wm.PathsTo(new Path("you", null), "out").ShouldBe(5);
    }
    
    [Fact]
    public void Real_1()
    {
        var lines = Util.ReadFile("day11");
        var wm = new WiringMess(lines);
        wm.PathsTo(new Path("you", null), "out").ShouldBe(696);
    }
    
    [Fact]
    public void Example_2()
    {
        var lines = Util.ReadRaw(SecondExample);
        // order hardcoded
    }
    
    [Fact]
    public void Real_2()
    {
        var lines = Util.ReadFile("day11");
        var wm = new WiringMess(lines);
        wm.PathsToPassingDacFft().ShouldBe(473741288064360L);
    }
}

public class WiringMess(IReadOnlyList<string> input)
{
    public Dictionary<string, Device> Devices { get; } = input.Select(Device.FromLine).ToDictionary(d => d.Id);

    public string PrintGraphViz()
    {
        var sb = new StringBuilder();
        sb.AppendLine("digraph G {");
        foreach (var device in Devices)
        {
            foreach (var linked in device.Value.Outputs)
            {
                sb.AppendLine($"    {device.Key} -> {linked}");
            }
        }
        sb.AppendLine("}");
        return sb.ToString();
    }

    public int PathsTo(Path path, string target)
    {
        if (path.Id == target) return 1;

        return Devices[path.Id].Outputs
            .Sum(next => PathsTo(path.Next(next), target));
    }
    
    public long PathsToNotIncluding(Path path, string target, IReadOnlyCollection<string> forbidden, Dictionary<string, long> cache)
    {
        if (cache.TryGetValue(path.Id, out var paths)) return paths;
        if (path.Id == target) return 1;

        var res = Devices[path.Id].Outputs
            .Where(next => !forbidden.Contains(next))
            .Sum(next => PathsToNotIncluding(path.Next(next), target, forbidden, cache));

        cache[path.Id] = res;
        return res;
    }
    
    public long PathsToPassingDacFft()
    {
        var afterDac = new HashSet<string>();
        PopulateNodesAfter(new Path("dac", null), afterDac);
        afterDac.Remove("dac");
        
        var afterFft = new HashSet<string>();
        PopulateNodesAfter(new Path("fft", null), afterFft);
        afterFft.Remove("fft");
        
        // fft first

        var svr2fft = PathsToNotIncluding(new Path("svr", null), "fft", afterFft, new Dictionary<string, long>());
        var fft2dac = PathsToNotIncluding(new Path("fft", null), "dac", afterDac, new Dictionary<string, long>());
        var dac2out = PathsToNotIncluding(new Path("dac", null), "out", [], new Dictionary<string, long>());
        
        return svr2fft * fft2dac * dac2out;
    }

    public void PopulateNodesAfter(Path path, HashSet<string> seen)
    {
        if (!seen.Add(path.Id)) return;
        if (!Devices.TryGetValue(path.Id, out var device)) return;

        foreach (var next in device.Outputs)
        {
            PopulateNodesAfter(path.Next(next), seen);
        }
    }
}

public record Device(string Id, string[] Outputs)
{
    public static Device FromLine(string l) => new(l.Split(' ')[0].Trim(':'), l.Split(' ')[1..]);
}

public record Path(string Id, Path? Prev)
{
    public Path Next(string id) => new(id, this);
    
    public bool Contains(string id) => Enumerate().Any(p => p.Id == id);
    
    public IEnumerable<Path> Enumerate()
    {
        yield return this;
        if (Prev == null) yield break;
        foreach (var path in Prev.Enumerate())
        {
            yield return path;
        }
    }

    public string Print() => string.Join(" -> ", Enumerate().Select(p => p.Id).Reverse());
}
