using FluentAssertions;

namespace AdventOfCode2024.Tests.Day01;

public class Day1
{
    [Fact]
    private void Solution1()
    {
        var input = Util.ReadFile("day1");

        var solution = new InputLists(input);
        solution.GetSolution().Should().Be(2164381L);
    }

    [Fact]
    private void Solution2()
    {
        var input = Util.ReadFile("day1");

        var solution = new InputLists(input);
        solution.GetCountSum().Should().Be(2164381L);
    }

    private record InputLists(string[] Lines)
    {
        public long GetSolution()
        {
            var list1 = new List<long>(Lines.Length);
            var list2 = new List<long>(Lines.Length);
            foreach (var line in Lines)
            {
                list1.Add(long.Parse(line.Split("  ")[0]));
                list2.Add(long.Parse(line.Split("  ")[1]));
            }
            list1.Sort();
            list2.Sort();

            var differences = new List<long>(list1.Count);
            for (var i = 0; i < list1.Count; i++)
            {
                differences.Add(list2[i] - list1[i]);
            }

            return differences.Sum(Math.Abs);
        }

        public long GetCountSum()
        {
            var numbers = Lines.Select(l => long.Parse(l.Split("  ")[0])).Distinct().ToDictionary(i => i, _ => 0L);
            foreach (var instance in Lines.Select(l => long.Parse(l.Split("  ")[1])))
            {
                if (!numbers.ContainsKey(instance)) continue;
                numbers[instance] += 1;
            }

            return numbers.Sum(n => n.Key * n.Value);
        }
    }

    private const string ExampleInput = """
                                        3   4
                                        4   3
                                        2   5
                                        1   3
                                        3   9
                                        3   3
                                        """;
}