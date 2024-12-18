using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day08 : ISolution
{
    private const string Example = """
                                   ............
                                   ........0...
                                   .....0......
                                   .......0....
                                   ....0.......
                                   ......A.....
                                   ............
                                   ............
                                   ........A...
                                   .........A..
                                   ............
                                   ............
                                   """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day08");
        
        var answer = UniqueAntiNodeLocations(input);
        answer.Should().Be(254);
    }

    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day08");

        var answer = UniqueRepeatingAntiNodeLocations(input);
        answer.Should().Be(951);
    }

    private int UniqueAntiNodeLocations(string[] lines)
    {
        var nodeLocations = new HashSet<Point>();
        var map = Point.GetMap(lines, c => c);

        foreach (var antennaGrouping in map.Where(kv => kv.Value != '.').GroupBy(kv => kv.Value))
        {
            foreach (var antennaPair in antennaGrouping.Combinations().Where(c => c[0].Key != c[1].Key))
            {
                var a1 = antennaPair[0].Key;
                var a2 = antennaPair[1].Key;
                var a1Ext = a1 + (a1 - a2);
                var a2Ext = a2 + (a2 - a1);
                if (map.ContainsKey(a1Ext)) nodeLocations.Add(a1Ext);
                if (map.ContainsKey(a2Ext)) nodeLocations.Add(a2Ext);
            }
        }

        return nodeLocations.Count;
    }
    
    private int UniqueRepeatingAntiNodeLocations(string[] lines)
    {
        var nodeLocations = new HashSet<Point>();
        var map = Point.GetMap(lines, c => c);

        foreach (var antennaGrouping in map.Where(kv => kv.Value != '.').GroupBy(kv => kv.Value))
        {
            foreach (var antennaPair in antennaGrouping.Combinations().Where(c => c[0].Key != c[1].Key))
            {
                var a1 = antennaPair[0].Key;
                var a2 = antennaPair[1].Key;

                var p = a1;
                var dir = a1 - a2;
                while (map.ContainsKey(p))
                {
                    nodeLocations.Add(p);
                    p += dir;
                }
                
                p = a2;
                dir = a2 - a1;
                while (map.ContainsKey(p))
                {
                    nodeLocations.Add(p);
                    p += dir;
                }
            }
        }

        return nodeLocations.Count;
    }
}