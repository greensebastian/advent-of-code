using Shouldly;

namespace AdventOfCode2021.Core.Test;

public class Day12
{
    private const string SmallExample = """
                                        start-A
                                        start-b
                                        A-c
                                        A-b
                                        b-d
                                        A-end
                                        b-end
                                        """;
    
    private const string Example = """
                                   fs-end
                                   he-DX
                                   fs-he
                                   start-DX
                                   pj-DX
                                   end-zg
                                   zg-sl
                                   zg-pj
                                   pj-he
                                   RW-he
                                   fs-DX
                                   pj-RW
                                   zg-RW
                                   start-pj
                                   he-WI
                                   zg-he
                                   pj-fs
                                   start-RW
                                   """;
    
    [Fact]
    public void Day12_1_Example()
    {
        var input = Example.Split('\n');
        var cs = new CaveSystem(input);
        cs.PotentialPaths().ShouldBe(226);
    }
    
    [Fact]
    public void Day12_1_Real()
    {
        var input = Util.ReadFile("day12");
        var cs = new CaveSystem(input);
        cs.PotentialPaths().ShouldBe(5457);
    }
}

public class CaveSystem(IReadOnlyList<string> input)
{
    public IReadOnlyDictionary<string, string[]> Links { get; } = input.Aggregate(new Dictionary<string, string[]>(),
        (links, line) =>
        {
            var left = line.Split('-')[0];
            var right = line.Split('-')[1];
            links.TryAdd(left, []);
            links.TryAdd(right, []);
            if (!links[left].Contains(right)) links[left] = [..links[left], right];
            if (!links[right].Contains(left)) links[right] = [..links[right], left];
            return links;
        });

    public int PotentialPaths()
    {
        var solutions = new List<Route>();
        var queue = new Stack<Route>();
        queue.Push(new Route(null, "start"));
        while (queue.TryPop(out var path))
        {
            if (path.Current == "end")
            {
                solutions.Add(path);
                continue;
            }

            var options = path.Next(Links[path.Current]).ToArray();
            foreach (var next in options)
            {
                queue.Push(next);
            }
        }

        return solutions.Count;
    }

}

public record Route(Route? Prev, string Current)
{
    public Route Append(string next) => new(this, next);

    public IEnumerable<Route> Steps()
    {
        yield return this;
        if (Prev is null) yield break;
        foreach (var step in Prev.Steps())
        {
            yield return step;
        }
    }

    public IEnumerable<Route> Next(IEnumerable<string> links)
    {
        foreach (var link in links)
        {
            if (link[0] >= 'a' && link[0] <= 'z' && Steps().Any(s => s.Current == link)) continue;
            yield return Append(link);
        }
    }

    public override string ToString() => string.Join(", ", Steps().Reverse().Select(r => r.Current));
}