using System.Text;

namespace AdventOfCode2023.Core.Day21;

public record Day21Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var map = new GardenersMap(Input.ToArray());
        var ans = map.ReachablePointsIn(int.Parse(args[0]));
        yield return ans.ToString();
    }

    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var map = new GardenersMap(Input.ToArray());
        var ans = map.MathematicallySolve();
        yield return ans.ToString();
    }
}

public class GardenersMap
{
    private IReadOnlyList<string> Input { get; }
    public GardenersMap(IReadOnlyList<string> input)
    {
        Input = input;
        for (var r = 0; r < input.Count; r++)
        {
            for (var c = 0; c < input[r].Length; c++)
            {
                Map[new Point(r, c)] = input[r][c];
            }
        }

        Height = input.Count;
        Width = input[0].Length;
    }

    public string Print(IReadOnlySet<Point> current)
    {
        var minRow = Map.Keys.Min(p => p.Row);
        var maxRow = Map.Keys.Max(p => p.Row);
        var minCol = Map.Keys.Min(p => p.Col);
        var maxCol = Map.Keys.Max(p => p.Col);
        var sb = new StringBuilder();
        for (var row = minRow; row <= maxRow; row++)
        {
            for (var col = minCol; col <= maxCol; col++)
            {
                var p = new Point(row, col);
                var c = current.Contains(p) ? 'O' : C(p);
                sb.Append(c);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    public string Print(IReadOnlyDictionary<Point, List<bool>> history)
    {
        var sb = new StringBuilder();
        foreach (var kv in history.OrderBy(kv => kv.Key.Row * 1000 + kv.Key.Col))
        {
            var head = $"[{kv.Key.Row}, {kv.Key.Col}]";
            sb.AppendLine($"{head,-20}{string.Join("", kv.Value.Select(v => v ? "1" : "0"))}");
        }

        return sb.ToString();
    }

    private char C(long row, long col) => Map.GetValueOrDefault(new Point(row % Height, col % Width), '#');
    private char C(Point p) => C(p.Row, p.Col);

    private Dictionary<Point, char> Map { get; } = new();
    private int Height { get; }
    private int Width { get; }

    public int ReachablePointsIn(int steps)
    {
        var start = Map.Single(p => p.Value == 'S').Key;
        var current = new HashSet<Point> { start };
        for (var i = 0; i < steps; i++)
        {
            var nextCurrent = new HashSet<Point>();
            foreach (var currentPosition in current)
            {
                var next = new[]
                        { currentPosition.East, currentPosition.South, currentPosition.West, currentPosition.North }
                    .Where(p => Map.TryGetValue(p, out var nextChar) && nextChar != '#' && !current.Contains(p))
                    .ToArray();
                foreach (var point in next)
                {
                    nextCurrent.Add(point);
                }
            }
            
            current = nextCurrent;
            //Console.WriteLine($"After {i}");
            //Console.WriteLine(Print(current));
        }

        return current.Count;
    }

    public long MathematicallySolve()
    {
        var gridSize = Input.Count == Input[0].Length ? Input.Count : throw new ArgumentOutOfRangeException();

        var start = Enumerable.Range(0, gridSize)
            .SelectMany(i => Enumerable.Range(0, gridSize)
                .Where(j => Input[i][j] == 'S')
                .Select(j => new Point(i, j)))
            .Single();

        var grids = 26501365 / gridSize;
        var rem = 26501365 % gridSize;

        // By inspection, the grid is square and there are no barriers on the direct horizontal / vertical path from S
        // So, we'd expect the result to be quadratic in (rem + n * gridSize) steps, i.e. (rem), (rem + gridSize), (rem + 2 * gridSize), ...
        // Use the code from Part 1 to calculate the first three values of this sequence, which is enough to solve for ax^2 + bx + c
        var sequence = new List<int>();
        var work = new HashSet<Point> { start };
        var steps = 0;
        for (var n = 0; n < 3; n++)
        {
            for (; steps < n * gridSize + rem; steps++)
            {
                // Funky modulo arithmetic bc modulo of a negative number is negative, which isn't what we want here
                work = new HashSet<Point>(work
                    .SelectMany(it => new[] {it.East, it.South, it.West, it.North})
                    .Where(dest => Input[(int)((dest.Col % 131 + 131) % 131)][(int)((dest.Row % 131 + 131) % 131)] != '#'));
            }

            sequence.Add(work.Count);
        }

        // Solve for the quadratic coefficients
        var c = sequence[0];
        var aPlusB = sequence[1] - c;
        var fourAPlusTwoB = sequence[2] - c;
        var twoA = fourAPlusTwoB - 2 * aPlusB;
        var a = twoA / 2;
        var b = aPlusB - a;

        long F(long n)
        {
            return a * n * n + b * n + c;
        }

        for (var i = 0; i < sequence.Count; i++)
        {
            Console.WriteLine($"{sequence[i]} : {F(i)}");
        }

        return F(grids);
    }
}

public readonly record struct Point(long Row, long Col)
{
    public override string ToString() => $"{Row}, {Col}";

    public Point South => this with { Row = Row + 1 };
    public Point North => this with { Row = Row - 1 };
    public Point West => this with { Col = Col - 1 };
    public Point East => this with { Col = Col + 1 };
    public Point Add(Point other) => new(Row + other.Row, Col + other.Col);
    public Point Sub(Point other) => new(Row - other.Row, Col - other.Col);
    public Point Mul(int factor) => new(Row * factor, Col * factor);
}