using System.Globalization;
using System.Text.RegularExpressions;

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
        var sum = 0;
        foreach (var line in Input)
        {
            var repeatInput = $"{line.Split(" ")[0].Repeat(5, "?")} {line.Split(" ")[1].Repeat(5, ",")}";
            var range = SpringRow.FromInput(repeatInput);
            var count = range.ValidArrangementCountDfs();
            sum += count;
        }
        yield return sum.ToString();
    }
}

public record SpringRow(string Springs, IList<int> BrokenRanges)
{
    private static Regex _brokenRegEx = new("(#+)[.]", RegexOptions.Compiled);
    
    private int BrokenCount { get; } = BrokenRanges.Sum();
    
    public static SpringRow FromInput(string line)
    {
        var brokenRanges = line.Split(' ')[1].Ints().ToArray();
        var springs = line.Split(' ')[0];
        return new SpringRow(springs, brokenRanges);
    }

    public int ValidArrangementCountDfs()
    {
        if (IsValid(Springs))
        {
            return 1;
        }
        var valid = 0;
        var unexplored = new Stack<string>();
        unexplored.Push(Springs);
        while (unexplored.Count > 0)
        {
            var curr = unexplored.Pop();
            var iOfNextOption = curr.IndexOf('?');
            if (iOfNextOption == -1)
            {
                if (IsValid(curr)) valid++;
                continue;
            }
            
            if (!CouldBeValid(curr)) continue;
            
            unexplored.Push(curr.ReplaceAt(iOfNextOption, 1, "#"));
            unexplored.Push(curr.ReplaceAt(iOfNextOption, 1, "."));
        }

        return valid;
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
            if (IsValid(option)) valid.Add(option);
        }

        return valid;
    }

    private bool CouldBeValid(string option)
    {
        var brokenCount = option.Count(c => c == '#');
        if (brokenCount > BrokenCount) return false;
        
        var wildcardCount = option.Count(c => c == '?');
        if (brokenCount + wildcardCount < BrokenCount) return false;

        var optionBeforeQuestion = option;
        var questionI = option.IndexOf('?') - 1;
        if (questionI >= 0) optionBeforeQuestion = option[..questionI];
        var brokenGroups = _brokenRegEx.Matches(optionBeforeQuestion).Select(m => m.Groups[1]).ToArray();
        var cap = Math.Min(brokenGroups.Length, BrokenRanges.Count);
        for (var i = 0; i < cap; i++)
        {
            if (i < cap - 1 && brokenGroups[i].Length < BrokenRanges[i]) return false;
            if (brokenGroups[i].Length > BrokenRanges[i]) return false;
        }
        
        return true;
    }

    private bool IsValid(string option)
    {
        var ranges = option.Split('.', StringSplitOptions.RemoveEmptyEntries).Select(r => r.Length).ToArray();
        if (ranges.Length != BrokenRanges.Count) return false;

        var optionValid = Enumerable.Range(0, BrokenRanges.Count).All(i => BrokenRanges[i] == ranges[i]);
        return optionValid;
    }
}