namespace AdventOfCode2023.Core.Day11;

public record Day11Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var map = GalaxyMap.FromInput(Input.ToArray());
        yield return map.SumOfShortestDistances().ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return 0.ToString();
    }
}

public record GalaxyMap(IList<Vector> Galaxies)
{
    private Vector Min { get; } = new(Galaxies.MinBy(v => v.Row)!.Row, Galaxies.MinBy(v => v.Col)!.Col);
    private Vector Max { get; } = new(Galaxies.MaxBy(v => v.Row)!.Row, Galaxies.MaxBy(v => v.Col)!.Col);

    public int SumOfShortestDistances()
    {
        var pairs = Galaxies.SelectMany((g1, i) => Galaxies.Skip(i + 1).Select(g2 => (g1, g2))).ToArray();
        var sum = 0;
        foreach (var (g1, g2) in pairs)
        {
            sum += g1.VectorTo(g2).NumberSteps;
        }

        return sum;
    }
    
    public static GalaxyMap FromInput(IList<string> lines)
    {
        var rowsToAdd = Enumerable.Range(0, lines.Count).Where(row => lines[row].All(c => c == '.')).ToArray();
        var colsToAdd = Enumerable.Range(0, lines[0].Length).Where(col => lines.All(l => l[col] == '.')).ToArray();

        var galaxies = new List<Vector>();
        for (var row = 0; row < lines.Count; row++)
        {
            var rowOffset = rowsToAdd.Count(r => r <= row);
            for (var col = 0; col < lines[0].Length; col++)
            {
                if (lines[row][col] != '#') continue;
                
                var colOffset = colsToAdd.Count(c => c <= col);
                galaxies.Add(new Vector(row + rowOffset, col + colOffset));
            }
        }

        return new GalaxyMap(galaxies);
    }
}

public record Vector(int Row, int Col)
{
    public override string ToString() => $"[{Row}, {Col}]";

    public Vector Above => this with { Row = Row - 1 };
    public Vector Below => this with { Row = Row + 1 };
    public Vector Left => this with { Col = Col - 1 };
    public Vector Right => this with { Col = Col + 1 };
    
    public Vector VectorTo(Vector other) => new(other.Row - Row, other.Col - Col);

    public int NumberSteps { get; } = Math.Abs(Row) + Math.Abs(Col);
}