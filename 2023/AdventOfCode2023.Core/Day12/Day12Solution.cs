namespace AdventOfCode2023.Core.Day12;

public record Day12Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var sum = 0L;
        foreach (var line in Input)
        {
            var range = SpringRow.FromInput(line);
            var count = range.ValidArrangementCountRecursive();
            sum += count;
        }
        yield return sum.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var sum = 0L;
        foreach (var line in Input)
        {
            var repeatInput = $"{line.Split(" ")[0].Repeat(5, "?")} {line.Split(" ")[1].Repeat(5, ",")}";
            var range = SpringRow.FromInput(repeatInput);
            var count = range.ValidArrangementCountRecursive();
            sum += count;
        }
        yield return sum.ToString();
    }
}

public record Cache(int LeftCutoff, int LeftSeen, int WildLeft);

public record SpringRow(string Springs, IList<int> BrokenRanges)
{
    private int BrokenCount { get; } = BrokenRanges.Sum();
    
    public static SpringRow FromInput(string line)
    {
        var brokenRanges = line.Split(' ')[1].Ints().ToArray();
        var springs = line.Split(' ')[0];
        return new SpringRow(springs, brokenRanges);
    }

    public long ValidArrangementCountRecursive()
    {
        return SolutionCount(Springs, new Dictionary<Cache, long>());
    }

    private long SolutionCount(string input, Dictionary<Cache, long> cache)
    {
        if (!CouldBeValid(input)) return 0;
        var iOfNextOption = input.IndexOf('?');
        var iOfNextOptionOr0 = Math.Max(iOfNextOption, 0);
        int leftCutoff;
        if (iOfNextOption >= 0)
        {
            leftCutoff = Math.Max(input[..iOfNextOptionOr0].LastIndexOf('#') + 1, 0);
        }
        else
        {
            leftCutoff = input.LastIndexOf('#') + 1;
        }
        var left = input[..leftCutoff];
        var leftSeen = input[..leftCutoff].Split('.', StringSplitOptions.RemoveEmptyEntries); 
        var state = new Cache(leftCutoff, leftSeen.Length, left.Count(c => c == '#'));

        if (!string.IsNullOrEmpty(left) && cache.TryGetValue(state, out var res)) return res;
        if (iOfNextOption >= 0)
        {
            res = SolutionCount(input.ReplaceAt(iOfNextOption, 1, "#"), cache) +
                   SolutionCount(input.ReplaceAt(iOfNextOption, 1, "."), cache);
        }
        else
        {
            res = IsValid(input) ? 1 : 0;
        }
        
        cache[state] = res;
        
        return res;
    }

    private bool CouldBeValid(string option)
    {
        var brokenCount = option.Count(c => c == '#');
        if (brokenCount > BrokenCount) return false;
        
        var wildcardCount = option.Count(c => c == '?');
        if (brokenCount + wildcardCount < BrokenCount) return false;

        var optionBeforeQuestion = option;
        var questionI = option.IndexOf('?');
        if (questionI >= 0) optionBeforeQuestion = option[..questionI];

        var brokenI = 0;
        for (var i = 0; i < optionBeforeQuestion.Length; i++)
        {
            if (optionBeforeQuestion[i] == '.') continue;
            var toFind = BrokenRanges[brokenI];
            brokenI++;
            for (var j = 0; j < toFind; j++)
            {
                if (i >= optionBeforeQuestion.Length) break;
                if (optionBeforeQuestion[i] != '#') return false;
                i++;
            }
            if (i < optionBeforeQuestion.Length && optionBeforeQuestion[i] != '.') return false;
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