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
        var lines = Util.ReadRaw(Example);
    }
    
    [Fact]
    public void Real_2()
    {
        var lines = Util.ReadFile("day11");
    }
}

public class WiringMess(IReadOnlyList<string> input)
{
    public Dictionary<string, Device> Devices { get; } = input.Select(Device.FromLine).ToDictionary(d => d.Id);

    public int PathsTo(Path path, string target)
    {
        if (path.Id == target) return 1;

        return Devices[path.Id].Outputs
            .Where(next => !path.Contains(next))
            .Sum(next => PathsTo(path.Next(next), target));
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
}
