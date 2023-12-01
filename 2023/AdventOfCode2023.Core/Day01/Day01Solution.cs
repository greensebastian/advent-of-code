using System.Globalization;

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
        var elves = GetSum2();
        yield return elves.ToString();
    }

    private int GetSum()
    {
        var numbers = Input.Select(line => $"{line.First(c => int.TryParse(c.ToString(), out _))}{line.Last(c => int.TryParse(c.ToString(), out _))}");
        return numbers.Select(int.Parse).Sum();
    }

    private int GetSum2()
    {
        var ints = new List<string>
        {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"
        };
        
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
        
        var numbers = new List<string>();

        foreach (var line in Input)
        {
            var nbrs = SpelledOutInText(line, ints).ToList();
            var spelled = SpelledOutInText(line, strs).ToList();
            
            var res = "";

            var nbrBeforeSpelled = nbrs.First().Index < spelled.First().Index;
            res += nbrBeforeSpelled ? nbrs.First().Value : spelled.First().Value;
            var nbrAfterSpelled = nbrs.Last().Index > spelled.Last().Index;
            res += nbrAfterSpelled ? nbrs.Last().Value : spelled.Last().Value;
            numbers.Add(res);
        }
        
        return numbers.Select(int.Parse).Sum();
    }

    private static IEnumerable<(int Index, int Value)> SpelledOutInText(string text, IList<string> searchWords)
    {
        for (var index = 0; index < text.Length; index++)
        {
            for (var value = 0; value < searchWords.Count; value++)
            {
                var st = searchWords[value];
                if (text.Substring(index, st.Length) == st) yield return (index, value);
            }
        }
    }
}