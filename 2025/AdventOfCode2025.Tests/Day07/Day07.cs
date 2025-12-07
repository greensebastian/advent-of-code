using Shouldly;

namespace AdventOfCode2025.Tests.Day07;

public class Day07
{
    private const string Example = """
                                   .......S.......
                                   ...............
                                   .......^.......
                                   ...............
                                   ......^.^......
                                   ...............
                                   .....^.^.^.....
                                   ...............
                                   ....^.^...^....
                                   ...............
                                   ...^.^...^.^...
                                   ...............
                                   ..^...^.....^..
                                   ...............
                                   .^.^.^.^.^...^.
                                   ...............
                                   """;
    
    [Fact]
    public void Example_1()
    {
        var lines = Util.ReadRaw(Example);
        var sys = new TachyonBeamSystem(lines);
        sys.SplitCounts().ShouldBe(21);
    }
    
    [Fact]
    public void Real_1()
    {
        var lines = Util.ReadFile("day07");
        var sys = new TachyonBeamSystem(lines);
        sys.SplitCounts().ShouldBe(1675);
    }
    
    [Fact]
    public void Example_2()
    {
        var lines = Util.ReadRaw(Example);
        var sys = new TachyonBeamSystem(lines);
        sys.ParallelOptions().ShouldBe(40);
    }
    
    [Fact]
    public void Real_2()
    {
        var lines = Util.ReadFile("day07");
        var sys = new TachyonBeamSystem(lines);
        sys.ParallelOptions().ShouldBe(187987920774390L);
    }
}

public class TachyonBeamSystem(IReadOnlyList<string> input)
{
    public int SplitCounts()
    {
        var beams = new HashSet<int>
        {
            input[0].IndexOf('S')
        };
        var splits = 0;
        for (var row = 1; row < input.Count; row++)
        {
            var nextBeams = new HashSet<int>();
            foreach (var beamCol in beams)
            {
                if (input[row][beamCol] == '^')
                {
                    splits++;
                    nextBeams.Add(beamCol - 1);
                    nextBeams.Add(beamCol + 1);
                }
                else nextBeams.Add(beamCol);
            }

            beams = nextBeams;
        }

        return splits;
    }

    public long ParallelOptions()
    {
        var col = input[0].IndexOf('S');
        return ParallelOptions(1, col);
    }

    private Dictionary<(int row, int col), long> OptionsCache { get; } = new();

    private long ParallelOptions(int row, int col)
    {
        if (row == input.Count - 1) return 1;
        if (OptionsCache.TryGetValue((row, col), out var cached)) return cached;
        if (input[row][col] != '^') return ParallelOptions(row + 1, col);
        
        var sum = 0L;
        sum += ParallelOptions(row, col - 1);
        sum += ParallelOptions(row, col + 1);
        OptionsCache[(row, col)] = sum;
        return sum;
    }
}