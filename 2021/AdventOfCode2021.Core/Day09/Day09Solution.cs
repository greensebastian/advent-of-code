using System.Globalization;

namespace AdventOfCode2021.Core.Day09;

public record Day09Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var lines = Input.ToList();
        var board = Board.FromInputLines(lines);

        yield return board.RiskScore.ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        yield return "0";
    }
}

public record Point(int Row, int Col)
{
    public Point Up => this with { Row = Row - 1 };
    public Point Down => this with { Row = Row + 1 };
    public Point Left => this with { Col = Col - 1 };
    public Point Right => this with { Col = Col + 1 };

    public IEnumerable<Point> Neighbors(Point lowBound, Point highBound) =>
        new[] { Up, Right, Down, Left }.Where(p =>
            p.Row >= lowBound.Row && p.Row <= highBound.Row && p.Col >= lowBound.Col && p.Col <= highBound.Col);

    public override string ToString() => $"[R:{Row}\tC{Col}:]";

}

public record Board(IReadOnlyDictionary<Point, int> Points, Point TopLeft, Point BottomRight)
{
    public static Board FromInputLines(IList<string> inputLines)
    {
        var map = new Dictionary<Point, int>();
        var topLeft = new Point(0, 0);
        var bottomRight = new Point(inputLines.Count - 1, inputLines[0].Length - 1);
        for (var row = 0; row < inputLines.Count; row++)
        {
            for (var col = 0; col < inputLines[row].Length; col++)
            {
                var point = new Point(row, col);
                map[point] = int.Parse(inputLines[row][col].ToString());
            }
        }
        return new Board(map, topLeft, bottomRight);
    }

    public bool IsLowPoint(Point point) => point.Neighbors(TopLeft, BottomRight).All(n => Points[n] > Points[point]);

    public IEnumerable<(Point point, int height)> GetLowPoints() => Points.Keys.Where(IsLowPoint).Select(p => (p, Points[p]));

    public int RiskScore => GetLowPoints().Select(p => p.height + 1).Sum();

    public IEnumerable<Point> EqualOrLowerNeighbors(Point p) => p.Neighbors(TopLeft, BottomRight).Where(n => Points[n] <= Points[p]);

    public IEnumerable<ISet<Point>> Basins
    {
        get
        {
            var basins = new List<HashSet<Point>>();
            foreach (var (lowPoint, height) in GetLowPoints())
            {
                var basin = new HashSet<Point> { lowPoint };
                var toInvestigate = new HashSet<Point>();
                foreach (var basinNeighbor in lowPoint.Neighbors(TopLeft, BottomRight)
                             .Where(n => EqualOrLowerNeighbors(n).Contains(lowPoint)))
                {
                    basin.Add(basinNeighbor);
                    toInvestigate.Add(basinNeighbor);
                }

                while (toInvestigate.Count > 0)
                {
                    var next = toInvestigate.
                }
            }
        }
    }
}