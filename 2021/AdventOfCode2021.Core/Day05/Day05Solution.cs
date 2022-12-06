using System.Text;

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
        
        foreach (var point in lines.Where(line => line.AlongX || line.AlongY).SelectMany(line => line.PointsOnPath()))
        {
            grid[point.X, point.Y] += 1;
        }

        var moreThanTwo = grid.Cast<int>().Count(count => count > 1);

        yield return moreThanTwo.ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
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
            var points = line.PointsOnPath().ToArray();
            foreach (var point in points)
            {
                grid[point.X, point.Y] += 1;
            }
        }

        var moreThanTwo = grid.Cast<int>().Count(count => count > 1);

        yield return moreThanTwo.ToString();
    }

    private void Print(int[,] grid)
    {
        var sb = new StringBuilder();
        for (var rowIndex = 0; rowIndex <= grid.GetUpperBound(0); rowIndex++)
        {
            var row = new List<int>();
            for (var colIndex = 0; colIndex <= grid.GetUpperBound(1); colIndex++)
            {
                row.Add(grid[rowIndex, colIndex]);
            }

            var formatString = string.Join("|", row.Select(cell => $"{cell,2} "));
            sb.AppendLine(formatString);
        }

        sb.AppendLine("____________");
        
        Console.WriteLine(sb);
    }
}

internal record Line(string Input)
{
    private Point Start { get; } = new (Input.Split(" -> ")[0]);
    private Point End { get; } = new (Input.Split(" -> ")[1]);
    private int SmallX => Math.Min(Start.X, End.X);
    public int BigX => Math.Max(Start.X, End.X);
    private int SmallY => Math.Min(Start.Y, End.Y);
    public int BigY => Math.Max(Start.Y, End.Y);
    public bool AlongX => Start.Y == End.Y;
    public bool AlongY => Start.X == End.X;
    private bool SinglePoint => AlongX && AlongY;

    public IEnumerable<Point> PointsOnPath()
    {
        if (SinglePoint)
            yield return Start;
        else if (AlongX)
        {
            for (var x = SmallX; x <= BigX; x++)
            {
                yield return new Point(x, SmallY);
            }
        }
        else if (AlongY)
        {
            for (var y = SmallY; y <= BigY; y++)
            {
                yield return new Point(SmallX, y);
            }
        }
        else
        {
            var nbrPoints = BigX - SmallX + 1;
            var deltaX = (End.X - Start.X) / (nbrPoints - 1);
            var deltaY = (End.Y - Start.Y) / (nbrPoints - 1);
            var x = Start.X;
            var y = Start.Y;
            for (var delta = 0; delta < nbrPoints; delta++)
            {
                yield return new Point(x, y);
                x += deltaX;
                y += deltaY;
            }
        }
    }
}

internal class Point
{
    public Point(string input)
    {
        X = int.Parse(input.Split(",")[0]);
        Y = int.Parse(input.Split(",")[1]);
    }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
    
    public int X { get; }
    public int Y { get; }
}