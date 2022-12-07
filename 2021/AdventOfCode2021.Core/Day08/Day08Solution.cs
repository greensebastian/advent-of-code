using System.Globalization;

namespace AdventOfCode2021.Core.Day08;

public record Day08Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    /**
     *  aaaa
     * b    c
     * b    c
     *  dddd
     * e    f
     * e    f
     *  gggg
     */
    
    public override IEnumerable<string> FirstSolution()
    {
        foreach (var line in Input)
        {
            var decodeSets = line
                .Split(" | ")[0]
                .Split(" ")
                .GroupBy(word => word.Length)
                .ToDictionary(group => group.Key, group => group.ToList());
            var output = line.Split(" | ")[1].Split(" ");
            
            
        }

        yield return "0";
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        yield return "0";
    }
}

internal class Decoder
{
    private HashSet<string>[] Options;
}