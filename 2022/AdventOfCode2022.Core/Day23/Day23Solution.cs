using System.Globalization;

namespace AdventOfCode2022.Core.Day23;

public record Day23Solution(IEnumerable<string> Input) : BaseSolution(Input)
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