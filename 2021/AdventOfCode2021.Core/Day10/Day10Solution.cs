using System.Globalization;

namespace AdventOfCode2021.Core.Day10;

public record Day10Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        foreach (var line in Input)
        {
            var nbr = long.Parse(line, CultureInfo.InvariantCulture);
        }

        yield return "0";
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        yield return "0";
    }
}