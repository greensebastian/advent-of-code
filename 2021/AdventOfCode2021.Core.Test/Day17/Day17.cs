using System.Text.RegularExpressions;
using Shouldly;

namespace AdventOfCode2021.Core.Test.Day17;

public class Solution
{
    private const string Example = """
                                   target area: x=20..30, y=-10..-5
                                   """;
    
    [Fact]
    public void Part_1_Example()
    {
        var input = Example.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        BruteForceHighest(input[0]).highestPoint.ShouldBe(45);
    }
    
    [Fact]
    public void Part_1_Real()
    {
        var input = Util.ReadFile("day17");
        BruteForceHighest(input[0]).highestPoint.ShouldBe(11175L);
    }
    
    [Fact]
    public void Part_2_Example()
    {
        var input = Example.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        0.ShouldBe(0);
    }
    
    [Fact]
    public void Part_2_Real()
    {
        var input = Util.ReadFile("day17");
        BruteForceHighest(input[0]).nbrValidStarts.ShouldBe(11175L);
        // 3788L too high
    }

    public (long highestPoint, long nbrValidStarts) BruteForceHighest(string input)
    {
        var inputMatch = new Regex("(-?\\d+)");
        var matches = inputMatch.Matches(input).Select(m => int.Parse(m.Value)).ToArray();
        var targetBottom = new Vector(matches[0], matches[2]);
        var targetTop = new Vector(matches[1], matches[3]);

        bool InRange(Probe probe)
        {
            var x = probe.P.X;
            var y = probe.P.Y;
            return x >= targetBottom.X && x <= targetTop.X && y >= targetBottom.Y && y <= targetTop.Y;
        }
        
        // Find valid x
        var validX = new HashSet<long>();
        for (var dx = targetTop.X; dx > 0; dx--)
        {
            var p = new Probe(new(0, 0), new(dx, 0));
            while (p.P.X <= targetTop.X && p.dP.X > 0)
            {
                if (p.P.X >= targetBottom.X && p.P.X <= targetTop.X) validX.Add(dx);
                p = p.Move();
            }
        }

        var maxYStep = -targetBottom.Y + 1;

        var highestValid = long.MinValue;
        var validStarts = new HashSet<Vector>();
        foreach (var dx in validX)
        {
            var overshot = false;
            for (var dy = targetBottom.Y; !overshot && dy <= maxYStep; dy++)
            {
                var highest = long.MinValue;
                var p = new Probe(new(0, 0), new(dx, dy));
                while (p.P.X <= targetTop.X && p.P.Y >= targetBottom.Y)
                {
                    highest = Math.Max(highest, p.P.Y);
                    if (InRange(p))
                    {
                        highestValid = Math.Max(highestValid, highest);
                        validStarts.Add(new Vector(dx, dy));
                    }

                    if (p.P.X > targetTop.X && p.P.Y > targetTop.Y)
                    {
                        overshot = true;
                    }
                    p = p.Move();
                }
            }
        }
        return (highestValid, validStarts.Count);
    }
}

public record Vector(long X, long Y);

// ReSharper disable once InconsistentNaming
public record Probe(Vector P, Vector dP)
{
    public Probe Move() =>
        new(new(P.X + dP.X, P.Y + dP.Y), new(dP.X > 0 ? dP.X - 1 : dP.X < 0 ? dP.X + 1 : 0, dP.Y - 1));
}