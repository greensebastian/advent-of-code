using System.Globalization;

namespace AdventOfCode2023.Core.Day12;

public record Day12Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var sum = 0;
        foreach (var line in Input)
        {
            var range = SpringRow.FromInput(line);
            var count = range.ValidArrangementCount();
            sum += count;
        }
        yield return sum.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return 0.ToString();
    }
}

public record SpringRow(string Springs, IList<int> BrokenRanges)
{
    public static SpringRow FromInput(string line)
    {
        var brokenRanges = line.Split(' ')[1].Ints().ToArray();
        var springs = line.Split(' ')[0];
        return new SpringRow(springs, brokenRanges);

    }
    
    public int ValidArrangementCount()
    {
        return Permutations().Count();
    }

    private IEnumerable<string> Permutations()
    {
        var options = new[] { "" };
        for (var i = 0; i < Springs.Length; i++)
        {
            var newOptions = new List<string>();
            foreach (var option in options)
            {
                var c = Springs[i];
                switch (c)
                {
                    case '?':
                        newOptions.Add(option + '#');
                        newOptions.Add(option + '.');
                        break;
                    default:
                        newOptions.Add(option + c);
                        break;
                }
            }

            options = newOptions.ToArray();
        }

        var valid = new List<string>();
        foreach (var option in options)
        {
            var ranges = option.Split('.', StringSplitOptions.RemoveEmptyEntries).Select(r => r.Length).ToArray();
            if (ranges.Length != BrokenRanges.Count) continue;
            
            var optionValid = Enumerable.Range(0, BrokenRanges.Count).All(i => BrokenRanges[i] == ranges[i]);
            if (optionValid) valid.Add(option);
        }

        return valid;
    }
}