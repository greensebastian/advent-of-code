using System.Text;
using Shouldly;

namespace AdventOfCode2021.Core.Test;

public class Day13
{
    private const string Example = """
                                   6,10
                                   0,14
                                   9,10
                                   0,3
                                   10,4
                                   4,11
                                   6,0
                                   6,12
                                   4,1
                                   0,13
                                   10,12
                                   3,4
                                   3,0
                                   8,4
                                   1,10
                                   2,14
                                   8,10
                                   9,0
                                   
                                   fold along y=7
                                   fold along x=5
                                   """;
    
    [Fact]
    public void Day13_1_Example()
    {
        var input = Example.Split('\n');
        var p = new FoldingPaper(input);
        var ans = p.PointsAfterFolds();
        ans.ShouldBe(17);
    }
    
    [Fact]
    public void Day13_1_Real()
    {
        var input = Util.ReadFile("day13");
        var p = new FoldingPaper(input);
        var ans = p.PointsAfterFolds();
        ans.ShouldBe(827);
        // 982 too high
        // 981 too high
    }
    
    [Fact]
    public void Day13_2_Real()
    {
        var input = Util.ReadFile("day13");
        var p = new FoldingPaper(input);
        var ans = p.PointsAfterAllFolds();
        ans.ShouldBe(0);
        //EAHKRECP
    }
}

public class FoldingPaper(IReadOnlyList<string> input)
{
    public IReadOnlySet<Vector> UnfoldedPoints { get; } =
        input.TakeWhile(l => !string.IsNullOrWhiteSpace(l)).Select(Vector.FromInput).ToHashSet();

    public int PointsAfterFolds()
    {
        var fold = input.SkipWhile(l => !string.IsNullOrWhiteSpace(l)).Skip(1).Select(Fold.FromInput).First();
        var newPoints = new HashSet<Vector>();
        foreach (var point in UnfoldedPoints)
        {
            if (fold.FoldUp)
            {
                if (point.Row == fold.N)
                {
                    continue;
                }
                var newRow = point.Row > fold.N ? fold.N - (point.Row - fold.N) : point.Row;
                newPoints.Add(point with { Row = newRow });
            }
            else
            {
                if (point.Col == fold.N)
                {
                    continue;
                }
                var newCol = point.Col > fold.N ? fold.N - (point.Col - fold.N) : point.Col;
                newPoints.Add(point with { Col = newCol });
            }
        }
        return newPoints.Count;
    }
    
    public int PointsAfterAllFolds()
    {
        var folds = input.SkipWhile(l => !string.IsNullOrWhiteSpace(l)).Skip(1).Select(Fold.FromInput);
        var points = UnfoldedPoints.ToHashSet();
        foreach (var fold in folds)
        {
            var newPoints = new HashSet<Vector>();
            foreach (var point in points)
            {
                if (fold.FoldUp)
                {
                    if (point.Row == fold.N)
                    {
                        continue;
                    }
                    var newRow = point.Row > fold.N ? fold.N - (point.Row - fold.N) : point.Row;
                    newPoints.Add(point with { Row = newRow });
                }
                else
                {
                    if (point.Col == fold.N)
                    {
                        continue;
                    }
                    var newCol = point.Col > fold.N ? fold.N - (point.Col - fold.N) : point.Col;
                    newPoints.Add(point with { Col = newCol });
                }
            }

            points = newPoints;
        }

        return 0;
    }

    private string Print(IReadOnlySet<Vector> points)
    {
        var sb = new StringBuilder();
        var minRow = points.Min(p => p.Row);
        var maxRow = points.Max(p => p.Row);
        var minCol = points.Min(p => p.Col);
        var maxCol = points.Max(p => p.Col);
        for (var row = minRow; row <= maxRow; row++)
        {
            for (var col = minCol; col <= maxCol; col++)
            {
                sb.Append(points.Contains(new Vector(row, col)) ? '#' : '.');
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}

public record Vector(int Row, int Col)
{
    public static Vector FromInput(string line) => new(int.Parse(line.Split(',')[1]), int.Parse(line.Split(',')[0]));
}

public record Fold(bool FoldUp, int N)
{
    public static Fold FromInput(string line) => new(line[11] == 'y', int.Parse(line[13..]));
}