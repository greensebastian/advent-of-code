using System.Globalization;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;

namespace AdventOfCode2023.Core.Day18;

public record Day18Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var site = DigSite.FromInstruction(Input);
        yield return site.GetL1TrenchSize().ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var site = DigSite.FromInstruction(Input, true);
        yield return site.GetL1TrenchSize().ToString();
    }
}

public record DigSite(Instruction[] Instructions)
{
    public long GetL1TrenchSize()
    {
        var zero = new Point(0, 0);
        var path = new List<Point> { zero };
        var steps = 1;
        foreach (var instruction in Instructions)
        {
            var step = instruction.Dir switch
            {
                Direction.Up => zero.North,
                Direction.Down => zero.South,
                Direction.Left => zero.West,
                Direction.Right => zero.East,
                _ => throw new ArgumentOutOfRangeException()
            };
            steps += instruction.Len;
            path.Add(path[^1].Add(step.Mul(instruction.Len)));
        }
        return CoveredSquares(path, steps);
    }

    private static long CoveredSquares(List<Point> path, long steps)
    {
        var s = 0d;
        for (var i = 1; i < path.Count; i++)
        {
            var curr = path[i];
            var prev = path[i - 1];
            var mat = DenseMatrix.OfRows(new[] { new[] { (double)prev.Col, curr.Col }, new[] { (double)prev.Row, curr.Row } });
            s += mat.Determinant();
        }
        
        var l = (long)((s + steps) / 2).Round(0);

        return l;
    }
    
    public static DigSite FromInstruction(IEnumerable<string> lines, bool useHex = false)
    {
        return new DigSite(lines.Select(l => useHex ? Instruction.FromHexInput(l) : Instruction.FromInput(l)).ToArray());
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public record Instruction(Direction Dir, int Len)
{
    public static Instruction FromInput(string line)
    {
        var dir = line[0] switch
        {
            'U' => Direction.Up,
            'R' => Direction.Right,
            'D' => Direction.Down,
            'L' => Direction.Left,
            _ => throw new ArgumentOutOfRangeException()
        };
        var len = line.Ints().First();
        return new Instruction(dir, len);
    }
    
    public static Instruction FromHexInput(string line)
    {
        var rgb = line[^7..^1];
        var dir = rgb[^1] switch
        {
            '3' => Direction.Up,
            '0' => Direction.Right,
            '1' => Direction.Down,
            '2' => Direction.Left,
            _ => throw new ArgumentOutOfRangeException()
        };
        var len = int.Parse(rgb[..^1], NumberStyles.HexNumber);
        return new Instruction(dir, len);
    }
}


public record struct Point(long Row, long Col)
{
    public override string ToString() => $"{Row}, {Col}";

    public Point South => this with { Row = Row + 1 };
    public Point North => this with { Row = Row - 1 };
    public Point West => this with { Col = Col - 1 };
    public Point East => this with { Col = Col + 1 };
    public Point Add(Point other) => new(Row + other.Row, Col + other.Col);
    public Point Sub(Point other) => new(Row - other.Row, Col - other.Col);
    public Point Mul(int factor) => new(Row * factor, Col * factor);

    public Direction GetDir() => (Row, Col) switch
    {
        (>0, 0) => Direction.Down,
        (<0, 0) => Direction.Up,
        (0, >0) => Direction.Right,
        (0, <0) => Direction.Left,
        _ => throw new ArgumentOutOfRangeException()
    };
}