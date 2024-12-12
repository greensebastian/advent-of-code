using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day02 : ISolution
{
    [Fact]
    public void Solution1()
    {
        /*var input = Util.ReadRaw("""
                                 7 6 4 2 1
                                 1 2 7 8 9
                                 9 7 6 2 1
                                 1 3 2 4 5
                                 8 6 4 4 1
                                 1 3 6 7 9
                                 """);*/

        var input = Util.ReadFile("day02");

        var successful = input.Select(l => l.Split(" ").Select(int.Parse).ToArray()).Select(levels =>
        {
            var dir = levels[1] - levels[0];
            for (var i = 1; i < levels.Length; i++)
            {
                var diff = levels[i] - levels[i - 1];
                if (Math.Abs(diff) > 3 || Math.Abs(diff) < 1) return false;
                if (dir > 0 && diff < 0) return false;
                if (dir < 0 && diff > 0) return false;
            }

            return true;
        }).Count(s => s);

        successful.Should().Be(2);
    }

    [Fact]
    public void Solution2()
    {
        var input = Util.ReadFile("day02");

        var solution = new InputLists(input);
        solution.GetCountSum().Should().Be(20719933L);
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