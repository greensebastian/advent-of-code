using System.Globalization;

namespace AdventOfCode2022.Core.Day01;

public record Day01Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var elves = GetCaloriesByIndex();

        var maxElf = elves.MaxBy(pair => pair.Value);
        yield return maxElf.Key.ToString();
        yield return maxElf.Value.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var elves = GetCaloriesByIndex();
        
        var maxElves = elves.OrderByDescending(elf => elf.Value).Take(3).ToList();
        yield return string.Join(", ", maxElves.Select(elv => elv.Key));
        yield return maxElves.Select(elv => elv.Value).Sum().ToString();
    }

    private Dictionary<int, long> GetCaloriesByIndex()
    {
        var elves = new Dictionary<int, long>();
        var elfIndex = 1;
        foreach (var line in Input)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                elfIndex++;
            }
            else
            {
                if (!elves.ContainsKey(elfIndex))
                {
                    elves[elfIndex] = 0;
                }

                var calories = long.Parse(line, CultureInfo.InvariantCulture);
                elves[elfIndex] += calories;
            }
        }

        return elves;
    }
}