using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day25 : ISolution
{
    private const string Example = """
                                   #####
                                   .####
                                   .####
                                   .####
                                   .#.#.
                                   .#...
                                   .....
                                   
                                   #####
                                   ##.##
                                   .#.##
                                   ...##
                                   ...#.
                                   ...#.
                                   .....
                                   
                                   .....
                                   #....
                                   #....
                                   #...#
                                   #.#.#
                                   #.###
                                   #####
                                   
                                   .....
                                   .....
                                   #.#..
                                   ###..
                                   ###.#
                                   ###.#
                                   #####
                                   
                                   .....
                                   .....
                                   .....
                                   #....
                                   #.#..
                                   #.#.#
                                   #####
                                   """;

    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day25");

        var ans = new LockAndKeySet(input).MatchingCombinations();

        ans.Should().Be(3291);
    }

    [Fact]
    public void Solution2()
    {
        var input = Util.ReadRaw(Example);
        //var input = Util.ReadFile("day25");

        0.Should().Be(1);
    }

    public class LockAndKeySet(IReadOnlyList<string> input)
    {
        public const ushort StackHeight = 5;
        public const ushort PinCount = 5;
        
        private IReadOnlySet<Key> Keys => GetKeys().Keys.ToHashSet();
        private IReadOnlySet<Lock> Locks => GetKeys().Locks.ToHashSet();
        
        public int MatchingCombinations()
        {
            var sum = 0;
            foreach (var @lock in Locks)
            {
                foreach (var key in Keys)
                {
                    if (@lock.Fits(key)) sum++;
                }
            }
            return sum;
        }

        private record Key(int PinHeights)
        {
            public ushort[] Pins() => CalculatePins(PinHeights);
        }
        
        private record Lock(int PinHeights)
        {
            public ushort[] Pins() => CalculatePins(PinHeights);

            public bool Fits(Key key)
            {
                var keyPins = key.Pins();
                var allowedHeights = Pins().Select(p => StackHeight - p).ToArray();
                for (var i = 0; i < PinCount; i++)
                {
                    if (allowedHeights[i] < keyPins[i]) return false;
                }

                return true;
            }
        }

        private static ushort[] CalculatePins(int heights)
        {
            return Enumerable.Range(0, PinCount).Select(i =>
            {
                var divider = (int)Math.Pow(10, i);
                return (ushort) (heights / divider % 10);
            }).Reverse().ToArray();
        }
        
        private (Key[] Keys, Lock[] Locks) GetKeys()
        {
            var blocks = string.Join('\n', input).Split("\n\n").Select(b => b.Split('\n')).ToArray();
            
            var keys = blocks.Where(b => b[0][0] == '.').Select(b =>
            {
                var sum = 0;
                for (var c = 0; c < b[0].Length; c++)
                {
                    sum *= 10;
                    for (var r = 0; r < b.Length - 1; r++)
                    {
                        if (b[r][c] == '#') sum++;
                    }
                }

                return new Key(sum);
            }).ToArray();
            
            var locks = blocks.Where(b => b[0][0] == '#').Select(b =>
            {
                var sum = 0;
                for (var c = 0; c < b[0].Length; c++)
                {
                    sum *= 10;
                    for (var r = 1; r < b.Length; r++)
                    {
                        if (b[r][c] == '#') sum++;
                    }
                }

                return new Lock(sum);
            }).ToArray();

            return (keys, locks);
        }
    }
}