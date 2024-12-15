using System.Text.RegularExpressions;
using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day05 : ISolution
{
    private const string Example = """
                                   47|53
                                   97|13
                                   97|61
                                   97|47
                                   75|29
                                   61|13
                                   75|53
                                   29|13
                                   97|29
                                   53|29
                                   61|53
                                   97|53
                                   61|29
                                   47|13
                                   75|47
                                   97|75
                                   47|61
                                   75|61
                                   47|29
                                   75|13
                                   53|13
                                   
                                   75,47,61,53,29
                                   97,61,53,29,13
                                   75,29,13
                                   75,97,47,61,53
                                   61,13,29
                                   97,13,75,29,47
                                   """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day05");

        var answer = PrintingDirectives.CorrectPrintingMediansSum(input);
        answer.Should().Be(5374);
    }

    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day04");

        
    }

    private record PrintingDirectives
    {
        public static int CorrectPrintingMediansSum(string[] lines)
        {
            var rules = lines.TakeWhile(l => !string.IsNullOrWhiteSpace(l)).Select(l => new { Prerequisite = int.Parse(l[..2]), Page = int.Parse(l[3..]) }).ToArray();
            var requirements = rules.SelectMany(r => new[] { r.Prerequisite, r.Page }).Distinct()
                .ToDictionary(r => r, _ => new HashSet<int>());
            foreach (var rule in rules)
            {
                requirements[rule.Page].Add(rule.Prerequisite);
            }
            var updates = lines.Skip(rules.Length + 1).Select(l => l.Split(',').Select(int.Parse).ToArray()).ToArray();
            var valid = updates.Where(update =>
            {
                var seen = new HashSet<int>();
                foreach (var page in update)
                {
                    if (!requirements.ContainsKey(page)) requirements[page] = new HashSet<int>();
                    if (requirements[page].Any(req => update.Contains(req) && !seen.Contains(req))) return false;
                    seen.Add(page);
                }

                return true;
            });
            return valid.Sum(v => v[v.Length / 2]);
        }
    }
}