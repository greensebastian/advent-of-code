using System.Text.RegularExpressions;
using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day04 : ISolution
{
    private const string Example = """
                                   MMMSXXMASM
                                   MSAMXMSMSA
                                   AMXSXMAAMM
                                   MSAMASMSMX
                                   XMASAMXAMM
                                   XXAMMXXAMA
                                   SMSMSASXSS
                                   SAXAMASAAA
                                   MAMMMXMMMM
                                   MXMXAXMASX
                                   """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day04");

        var letters = new Dictionary<Point, char>();
        for (var row = 0; row < input.Length; row++)
        {
            for (var col = 0; col < input[row].Length; col++)
            {
                letters[new Point(row, col)] = input[row][col];
            }
        }

        var answer = XmasLine.ValidXmas(letters);
        answer.Should().Be(2468);
    }

    private record XmasLine(Point Start, Point End)
    {
        public static int ValidXmas(IReadOnlyDictionary<Point, char> letters)
        {
            var count = 0;
            foreach (var (xStart, _) in letters.Where(l => l.Value == 'X'))
            {
                foreach (var neighbour in xStart.ClockwiseNeighboursWithDiagonal())
                {
                    if (IsValidXmas(xStart, neighbour - xStart)) count++;
                }
            }

            return count;

            bool IsValidXmas(Point initial, Point dir)
            {
                var p = initial;
                foreach (var character in "XMAS")
                {
                    if (!letters.TryGetValue(p, out var c) || c != character) return false;
                    p += dir;
                }

                return true;
            }
        }
        
        public static int ValidXDashMas(IReadOnlyDictionary<Point, char> letters)
        {
            var count = 0;
            foreach (var (mid, _) in letters.Where(l => l.Value == 'A'))
            {
                var tlbr = IsValidXDashMas(mid, mid.Up.Left - mid);
                var trbl = IsValidXDashMas(mid, mid.Up.Right - mid);
                if (tlbr && trbl) count++;
            }

            return count;
            
            bool IsValidXDashMas(Point initial, Point dir)
            {
                if (!letters.ContainsKey(initial + dir) || !letters.ContainsKey(initial - dir)) return false;
                return (letters[initial + dir] == 'S' && letters[initial - dir] == 'M') ||
                       (letters[initial + dir] == 'M' && letters[initial - dir] == 'S');
            }
        }
    }
    
    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day04");

        var letters = new Dictionary<Point, char>();
        for (var row = 0; row < input.Length; row++)
        {
            for (var col = 0; col < input[row].Length; col++)
            {
                letters[new Point(row, col)] = input[row][col];
            }
        }

        var answer = XmasLine.ValidXDashMas(letters);
        answer.Should().Be(1864);
    }
}