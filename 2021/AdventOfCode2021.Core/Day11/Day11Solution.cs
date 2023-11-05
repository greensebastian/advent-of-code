using System.Text;

namespace AdventOfCode2021.Core.Day11;

public record Day11Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var squids = SquidGrid.FromInput(Input.ToList());

        for (var i = 0; i < 100; i++)
        {
            squids.Tick();
        }

        yield return squids.FlashCount.ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        var squids = SquidGrid.FromInput(Input.ToList());

        var i = 0;
        while (!squids.SyncedFlash)
        {
            squids.Tick();
            i++;
        }

        yield return i.ToString();
    }
}

public class SquidGrid
{
    private Dictionary<Point, int> SquidPowers { get; }
    private Point LowBound { get; }
    private Point HighBound { get; }
    public int FlashCount { get; private set; } = 0;
    public bool SyncedFlash { get; private set; } = false;

    private SquidGrid(Dictionary<Point, int> squidPowers)
    {
        LowBound = new Point(0, 0);
        HighBound = new Point(squidPowers.Select(pos => pos.Key.Row).Max(), squidPowers.Select(pos => pos.Key.Col).Max());
        SquidPowers = squidPowers;
    }

    public static SquidGrid FromInput(IList<string> lines)
    {
        var grid = new Dictionary<Point, int>();
        for (var row = 0; row < lines.Count; row++)
        {
            for (var col = 0; col < lines[row].Length; col++)
            {
                grid[new Point(row, col)] = int.Parse(lines[row][col].ToString());
            }
        }

        return new SquidGrid(grid);
    }

    public void Tick()
    {
        var toFlash = new Queue<Point>();
        foreach (var position in SquidPowers.Keys)
        {
            SquidPowers[position] += 1;
            if(SquidPowers[position] > 9) toFlash.Enqueue(position);
        }

        var flashed = new HashSet<Point>();
        while (toFlash.Count > 0)
        {
            var flashPosition = toFlash.Dequeue();
            Flash(flashPosition, flashed);
        }

        if (flashed.Count == SquidPowers.Count)
        {
            SyncedFlash = true;
        }

        foreach (var position in flashed)
        {
            SquidPowers[position] = 0;
        }
    }

    private void Flash(Point flashPosition, HashSet<Point> flashed)
    {
        if (flashed.Contains(flashPosition)) return;
        flashed.Add(flashPosition);
        FlashCount++;

        foreach (var neighbour in flashPosition.Neighbors(LowBound, HighBound))
        {
            SquidPowers[neighbour] += 1;
            if (SquidPowers[neighbour] > 9) Flash(neighbour, flashed);
        }
    }

    public string Print()
    {
        var sb = new StringBuilder();
        for (var row = LowBound.Row; row <= HighBound.Row; row++)
        {
            for (var col = LowBound.Col; col <= HighBound.Col; col++)
            {
                sb.Append(SquidPowers[new Point(row, col)]);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}

public record Point(int Row, int Col)
{
    public Point Up => this with { Row = Row - 1 };
    public Point Down => this with { Row = Row + 1 };
    public Point Left => this with { Col = Col - 1 };
    public Point Right => this with { Col = Col + 1 };

    public IEnumerable<Point> Neighbors(Point lowBound, Point highBound)
    {
        var neighbours = new[] { Up.Left, Up, Up.Right, Left, Right, Down.Left, Down, Down.Right };
        return neighbours.Where(pos => pos.Col >= lowBound.Col && pos.Row >= lowBound.Col && pos.Col <= highBound.Col && pos.Row <= highBound.Row);
    }

    public override string ToString() => $"[R:{Row}\tC{Col}:]";

}