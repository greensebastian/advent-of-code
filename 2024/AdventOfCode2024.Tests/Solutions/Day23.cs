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
        //var input = Util.ReadRaw(SecondExample);
        var input = Util.ReadFile("day22");

        0.Should().Be(0);
    }

    private class LanParty(string[] input)
    {
        private IReadOnlyList<string> Computers { get; } = input.SelectMany(l => l.Split('-')).ToList();

        private IReadOnlyDictionary<string, HashSet<string>> LinksWithComputer { get; } = input.Aggregate(
            new Dictionary<string, HashSet<string>>(),
            (map, line) =>
            {
                var c0 = line.Split('-')[0];
                var c1 = line.Split('-')[1];
                map.TryAdd(c0, new HashSet<string>());
                map.TryAdd(c1, new HashSet<string>());
                map[c0].Add(c1);
                map[c1].Add(c0);
                return map;
            });

        public IReadOnlyList<HashSet<string>> Groups()
        {
            var toCheck = new Queue<string>();
            var groups = new List<HashSet<string>>();
            foreach (var computer in Computers)
            {
                toCheck.Enqueue(computer);
            }

            while (toCheck.TryDequeue(out var computer))
            {
                if (groups.Any(g => g.Contains(computer))) continue;

                var group = new HashSet<string>();
                groups.Add(group);
                
                var groupQueue = new Queue<string>();
                groupQueue.Enqueue(computer);
                while (groupQueue.TryDequeue(out var connection))
                {
                    if (group.Add(connection))
                    {
                        foreach (var nestedConnection in LinksWithComputer[connection])
                        {
                            groupQueue.Enqueue(nestedConnection);
                        }
                    }
                }
            }

            return groups;
        }
        
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