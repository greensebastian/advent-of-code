using System.Collections;
using FluentAssertions;
// ReSharper disable InvalidXmlDocComment

namespace AdventOfCode2024.Tests.Solutions;

public class Day23 : ISolution
{
    private const string Example = """
                                   kh-tc
                                   qp-kh
                                   de-cg
                                   ka-co
                                   yn-aq
                                   qp-ub
                                   cg-tb
                                   vc-aq
                                   tb-ka
                                   wh-tc
                                   yn-cg
                                   kh-ub
                                   ta-co
                                   de-co
                                   tc-td
                                   tb-wq
                                   wh-td
                                   ta-ka
                                   td-qp
                                   aq-cg
                                   wq-ub
                                   ub-vc
                                   de-ta
                                   wq-aq
                                   wq-vc
                                   wh-yn
                                   ka-de
                                   kh-ta
                                   co-tc
                                   wh-qp
                                   tb-vc
                                   td-yn
                                   """;

    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day23");

        var sum = new LanParty(input).GroupsOfThreeContainingStartingWith("t");
        sum.Should().Be(1218);
    }

    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day23");

        var sum = new LanParty(input).Password();

        sum.Should().Be("co,de,ka,ta");
    }

    private class LanParty(string[] input)
    {
        private IReadOnlyList<string> Computers { get; } = input.SelectMany(l => l.Split('-')).Distinct().ToList();

        private IReadOnlyDictionary<string, HashSet<string>> LinksWithComputer { get; } = input.Aggregate(
            new Dictionary<string, HashSet<string>>(),
            (map, line) =>
            {
                var c0 = line.Split('-')[0];
                var c1 = line.Split('-')[1];
                map.TryAdd(c0, new HashSet<string>());
                map.TryAdd(c1, new HashSet<string>());
                map[c0].Add(c1);
                map[c0].Add(c0);
                map[c1].Add(c0);
                map[c1].Add(c1);
                return map;
            });
        
        public int GroupsOfThreeContainingStartingWith(string prefix)
        {
            var setsOfThree = new HashSet<SetOfThree>();
            foreach (var line in input)
            {
                var split = line.Split('-');
                var c0 = split[0];
                var c1 = split[1];
                foreach (var linkedComputer in LinksWithComputer[c0].Where(linked => LinksWithComputer[c1].Contains(linked)))
                {
                    if (linkedComputer == c0 || linkedComputer == c1) continue;
                    setsOfThree.Add(SetOfThree.From(c0, c1, linkedComputer));
                }
            }

            return setsOfThree.Count(s => s.AnyStartsWith(prefix));
        }

        public string Password()
        {
            var best = "";
            for (var i = 0; i < input.Length; i++)
            {
                var line = input[i];
                var c0 = line.Split('-')[0];
                var c1 = line.Split('-')[1];
                var password = BiggestSubgroup([c0, c1]);
                if (password.Length > best.Length) best = password;
            }
            
            return best;
        }

        private Dictionary<string, string> GroupsCache { get; } = new();

        private ulong ToNumber(params string[] computers)
        {
            var sum = 0ul;
            foreach (var computer in computers)
            {
                sum = AppendNumber(sum, computer);
            }

            return sum;
        }

        private ulong AppendNumber(ulong original, string next)
        {
            original *= 100;
            original += (uint)(next[0] - 'a');
            original *= 100;
            original += (uint)(next[1] - 'a');
            return original;
        }

        private string[] ToStrings(ulong number)
        {
            var numbers = new List<string>();
            while (number > 0)
            {
                var least = number % 100 + 'a';
                number /= 100;
                var biggest = number % 100 + 'a';
                number /= 100;
                numbers.Add($"{(char)biggest}{(char)least}");
            }

            return numbers.ToArray();
        }

        private string BiggestSubgroup(IEnumerable<string> groupEnumerable)
        {
            var group = groupEnumerable.ToHashSet();
            var password = GetPassword(group);
            
            var cached = GroupsCache.FirstOrDefault(cached => password.StartsWith(cached.Key));
            if (!string.IsNullOrWhiteSpace(cached.Key)) return cached.Value;
            
            var linkedToByAll = Computers
                .Where(potential => !group.Contains(potential))
                .Where(potential => group.All(inGroup => LinksWithComputer[inGroup].Contains(potential)))
                .ToArray();

            if (linkedToByAll.Length == 0)
            {
                GroupsCache[password] = password;
                return password;
            }

            var best = GetPassword(group);
            foreach (var next in linkedToByAll)
            {
                var subGroup = BiggestSubgroup(group.Append(next));
                if (subGroup.Length > best.Length) best = subGroup;
            }

            GroupsCache[password] = best;
            return best;
        }

        private string GetPassword(IEnumerable<string> computers) => string.Join(',', computers.OrderBy(c => c));

        private record SetOfThree(string A, string B, string C)
        {
            public static SetOfThree From(params string[] computers)
            {
                if (computers.Length != 3) throw new ArgumentException("wrong length", nameof(computers));
                var inOrder = computers.OrderBy(c => c).ToArray();
                return new SetOfThree(inOrder[0], inOrder[1], inOrder[2]);
            }

            public bool AnyStartsWith(string prefix) =>
                A.StartsWith(prefix) || B.StartsWith(prefix) || C.StartsWith(prefix);
        }
    }
}