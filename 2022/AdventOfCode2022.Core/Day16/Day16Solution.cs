using System.Globalization;

namespace AdventOfCode2022.Core.Day16;

public record Day16Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        foreach (var line in Input)
        {
            var nbr = long.Parse(line, CultureInfo.InvariantCulture);
        }

        yield return "0";
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return "0";
    }
}