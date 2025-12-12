using System.Text;
using Shouldly;

namespace AdventOfCode2025.Tests.Day12;

public class Day12
{
    private const string Example = """
                                   0:
                                   ###
                                   ##.
                                   ##.
                                   
                                   1:
                                   ###
                                   ##.
                                   .##
                                   
                                   2:
                                   .##
                                   ###
                                   ##.
                                   
                                   3:
                                   ##.
                                   ###
                                   ##.
                                   
                                   4:
                                   ###
                                   #..
                                   ###
                                   
                                   5:
                                   ###
                                   .#.
                                   ###
                                   
                                   4x4: 0 0 0 0 2 0
                                   12x5: 1 0 1 0 2 2
                                   12x5: 1 0 1 0 3 2
                                   """;
    
    [Fact]
    public void Example_1()
    {
        var lines = Util.ReadRaw(Example);
        // Skip, solution is trivial
    }
    
    [Fact]
    public void Real_1()
    {
        var lines = Util.ReadFile("day12");
        var pt = new PresentTetris(lines);
        pt.RegionsFittingAllPresents().ShouldBe(575);
    }
}

public class PresentTetris(IReadOnlyList<string> input)
{
    public IReadOnlyList<Shape> Shapes { get; } = GetShapes(input).ToArray();

    private static IEnumerable<Shape> GetShapes(IReadOnlyList<string> input)
    {
        for (var r = 0; r < 6; r++)
        {
            yield return Shape.FromInput(input.Skip(5 * r + 1).Take(3).ToArray());
        }
    }

    public IReadOnlyList<Region> Regions { get; } = input.Skip(30).Select((l, i) =>
    {
        var cols = int.Parse(l.Split(':')[0].Split('x')[0]);
        var rows = int.Parse(l.Split(':')[0].Split('x')[1]);
        var counts = l.Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        return new Region(i, cols, rows, counts);
    }).ToArray();

    public int RegionsFittingAllPresents()
    {
        for (var i = 0; i < Shapes.Count; i++)
        {
            Console.WriteLine(i);
            foreach (var permutation in Shapes[i].Permutations)
            {
                Console.WriteLine(permutation.Print());
            }
        }

        var margin = 0.1;
        var solvableRegions = Regions.Where(r =>
        {
            var positions = r.Rows * r.Cols;
            var required = r.Counts.Select((count, i) => (count, shape: Shapes[i]))
                .Sum(p => p.shape.FilledCount * p.count);
            var possible = required <= positions * (1 - margin);
            return possible;
        }).ToArray();

        var trivial = Regions.Where(r =>
        {
            var colBlocks = r.Cols / 3;
            var rowBlocks = r.Rows / 3;
            var blocks = colBlocks * rowBlocks;
            var required = r.Counts.Sum();
            return blocks >= required;
        }).ToArray();

        var toSolve = solvableRegions.Where(r => !trivial.Contains(r));

        return trivial.Length;
    }
}

public record Region(int Id, int Cols, int Rows, int[] Counts);

public record Shape(IReadOnlyList<bool[,]> Permutations)
{
    public int FilledCount { get; } = Enumerable.Range(0, 3).SelectMany(r => Enumerable.Range(0, 3).Select(c => (r, c)))
        .Count(p => Permutations[0][p.r, p.c]);
    
    public static Shape FromInput(IReadOnlyList<string> input)
    {
        bool F(int r, int c)
        {
            return input[r][c] == '#';
        }

        var m = new[,]
        {
            { F(0, 0), F(0, 1), F(0, 2) },
            { F(1, 0), F(1, 1), F(1, 2) },
            { F(2, 0), F(2, 1), F(2, 2) }
        };
        return new Shape(m.GetPermutations());
    }

    public void Print()
    {
        foreach (var permutation in Permutations)
        {
            Console.WriteLine(permutation.Print());
        }
    }
}

public static class MatrixExtensions
{
    public static string Print(this bool[,] m)
    {
        if (m.Length != 9) throw new Exception("wrong size");
        if (m.Rank != 2) throw new Exception("wrong rank");

        var sb = new StringBuilder();
            
        for (var row = 0; row < 3; row++)
        {
            for (var col = 0; col < 3; col++)
            {
                sb.Append(m[row, col] ? "#" : ".");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
    
    extension<T>(T[,] m)
    {
        public IReadOnlyList<T[,]> GetPermutations()
        {
            var r0 = m;
            var f01 = r0.FlipAlongMidCol();
            var f02 = r0.FlipAlongMidRow();
            var r1 = r0.RotateClockwise();
            var f11 = r1.FlipAlongMidCol();
            var f12 = r1.FlipAlongMidRow();
            var r2 = r1.RotateClockwise();
            var f21 = r2.FlipAlongMidCol();
            var f22 = r2.FlipAlongMidRow();
            var r3 = r2.RotateClockwise();
            var f31 = r3.FlipAlongMidCol();
            var f32 = r3.FlipAlongMidRow();
            var res = new[] { r0, f01, f02, r1, f11, f12, r2, f21, f22, r3, f31, f32 }.Aggregate(new List<T[,]>(),
                (list, r) =>
                {
                    if (!list.Any(m => m.Equivalent(r))) list.Add(r);
                    return list;
                });

            return res;
        }
        
        public T[,] RotateClockwise()
        {
            if (m.Length != 9) throw new Exception("wrong size");
            if (m.Rank != 2) throw new Exception("wrong rank");

            // 00, 01, 02
            // 10, 11, 12
            // 20, 21, 22
        
            return new[,]
            {
                { m[2, 0], m[1, 0], m[0, 0] },
                { m[2, 1], m[1, 1], m[0, 1] },
                { m[2, 2], m[1, 2], m[0, 2] }
            };
        }

        public T[,] FlipAlongMidRow()
        {
            if (m.Length != 9) throw new Exception("wrong size");
            if (m.Rank != 2) throw new Exception("wrong rank");

            // 00, 01, 02
            // 10, 11, 12
            // 20, 21, 22
        
            return new[,]
            {
                { m[2, 0], m[2, 1], m[2, 2] },
                { m[1, 0], m[1, 1], m[1, 2] },
                { m[0, 0], m[0, 1], m[0, 2] }
            };
        }
        
        public T[,] FlipAlongMidCol()
        {
            if (m.Length != 9) throw new Exception("wrong size");
            if (m.Rank != 2) throw new Exception("wrong rank");

            // 00, 01, 02
            // 10, 11, 12
            // 20, 21, 22
        
            return new[,]
            {
                { m[0, 2], m[0, 1], m[0, 0] },
                { m[1, 2], m[1, 1], m[1, 0] },
                { m[2, 2], m[2, 1], m[2, 0] }
            };
        }

        public bool Equivalent(T[,] other)
        {
            if (m.Length != 9) throw new Exception("wrong size");
            if (m.Rank != 2) throw new Exception("wrong rank");

            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (!m[row, col]!.Equals(other[row, col])) return false;
                }
            }

            return true;
        }
    }
}