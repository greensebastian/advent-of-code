using System.Globalization;

namespace AdventOfCode2021.Core.Day05;

public record Day05Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var lines = Input.Select(line => new Line(line)).ToList();

        var maxX = 0;
        var maxY = 0;
        foreach (var line in lines)
        {
            maxX = maxX > line.BigX ? maxX : line.BigX;

            maxY = maxY > line.BigY ? maxY : line.BigY;
        }

        var grid = new int[maxX + 1, maxY + 1];

        foreach (var line in lines)
        {
            var sameX = line.Start.X == line.End.X;
            var sameY = line.Start.Y == line.End.Y;

            var single = sameX && sameY;
            var lineOnX = sameY && !sameX;
            var lineOnY = sameX && !sameY;

            if (single)
            {
                grid[line.Start.X, line.Start.Y] += 1;
            }
            else if (lineOnX)
            {
                for (var y = line.SmallY; y <= line.BigY; y++)
                {
                    grid[line.Start.X, y] += 1;
                }
            }
            else if (lineOnY)
            {
                for (var x = line.SmallX; x <= line.BigX; x++)
                {
                    grid[x, line.Start.Y] += 1;
                }
            }
        }

        var moreThanTwo = 0;
        foreach (var count in grid)
        {
            if (count > 1) moreThanTwo++;
        }
        
        yield return moreThanTwo.ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        yield return "0";
    }
}

internal record Line(string Input)
{
    public Point Start { get; } = new Point(Input.Split(" -> ")[0]);
    public Point End { get; } = new Point(Input.Split(" -> ")[1]);
    public int SmallX => Math.Min(Start.X, End.X);
    public int BigX => Math.Max(Start.X, End.X);
    public int SmallY => Math.Min(Start.Y, End.Y);
    public int BigY => Math.Max(Start.Y, End.Y);
}

internal record Point(string Input)
{
    public int X { get; } = int.Parse(Input.Split(",")[0]);
    public int Y { get; } = int.Parse(Input.Split(",")[1]);
}