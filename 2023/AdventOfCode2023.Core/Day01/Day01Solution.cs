namespace AdventOfCode2023.Core.Day01;

public record Day01Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var elves = GetSum();
        yield return elves.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var elves = GetSumReplaced();
        yield return elves.ToString();
    }

    private int GetSum()
    {
        var numbers = Input.Select(line => $"{line.First(c => int.TryParse(c.ToString(), out _))}{line.Last(c => int.TryParse(c.ToString(), out _))}");
        return numbers.Select(int.Parse).Sum();
    }

    private int GetSumReplaced()
    {
        var strs = new List<string>
        {
            "zero",
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine"
        };

        var nbs = new List<string>();
        foreach (var rawLine in Input)
        {
            var line = rawLine;
            for (var index = 0; index <= line.Length; index++)
            {
                for (var i = 0; i < strs.Count; i++)
                {
                    var st = strs[i];
                    if ((index + st.Length) <= line.Length && line.Substring(index, st.Length) == st)
                    {
                        line = $"{line.Substring(0, index)}{i}{line.Substring(index + st.Length)}";
                    }
                }
            }
            
            nbs.Add($"{line.First(c => int.TryParse(c.ToString(), out _))}{line.Last(c => int.TryParse(c.ToString(), out _))}");
        }
        return nbs.Select(int.Parse).Sum();
    }
}