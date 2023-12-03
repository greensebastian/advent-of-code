namespace AdventOfCode2023.Core.Day03;

public record Day03Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var schematic = EngineSchematic.FromInput(Input.ToList());
        
        yield return schematic.NumbersNextToSymbols().Sum().ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var schematic = EngineSchematic.FromInput(Input.ToList());
        
        yield return schematic.NumbersNextToGears().Sum(nbrs => nbrs.Aggregate(1, (prev, curr) => prev * curr)).ToString();
    }
}

public record EngineSchematic(IReadOnlyDictionary<Point, char> SymbolPositions, IList<(Point Position, int Number)> Numbers)
{
    public IEnumerable<int> NumbersNextToSymbols()
    {
        foreach (var (position, number) in Numbers)
        {
            if (HasSymbolNeighbour(position, number)) yield return number;
        }
    }

    private bool HasSymbolNeighbour(Point position, int number)
    {
        foreach (var neighbour in position.Neighbours(number.ToString().Length))
        {
            if (SymbolPositions.ContainsKey(neighbour)) return true;
        }

        return false;
    }
    
    private bool HasNeighbour(Point position, int number, Point potentialNeighbour)
    {
        foreach (var neighbour in position.Neighbours(number.ToString().Length))
        {
            if (neighbour == potentialNeighbour) return true;
        }

        return false;
    }
    
    public IEnumerable<IList<int>> NumbersNextToGears()
    {
        foreach (var gearPosition in SymbolPositions.Where(pair => pair.Value == '*').Select(pair => pair.Key))
        {
            var neighbouringNumbers = new List<int>();
            foreach (var (numberPosition, number) in Numbers)
            {
                if (Math.Abs(numberPosition.Row - gearPosition.Row) > 1) continue;
                if (HasNeighbour(numberPosition, number, gearPosition)) neighbouringNumbers.Add(number);
            }

            if (neighbouringNumbers.Count > 1) yield return neighbouringNumbers;
        }
    }

    public static EngineSchematic FromInput(IList<string> input)
    {
        var symbols = new Dictionary<Point, char>();
        var nums = new List<(Point Position, int NumberLength)>();
        for (var row = 0; row < input.Count; row++)
        {
            var curNum = "";
            for (var col = 0; col < input[row].Length; col++)
            {
                var sym = input[row][col];
                if (char.IsNumber(sym))
                {
                    curNum += sym;
                }
                else
                {
                    if (curNum != "")
                    {
                        nums.Add((new Point(row, col - curNum.Length), int.Parse(curNum)));
                        curNum = "";
                    }

                    if (sym != '.')
                    {
                        symbols.Add(new Point(row, col), sym);
                    }
                }
            }
            if (curNum != "")
            {
                nums.Add((new Point(row, input[row].Length - curNum.Length), int.Parse(curNum)));
            }
        }

        return new EngineSchematic(symbols, nums);
    }
}

public record Point(int Row, int Col)
{
    public override string ToString() => $"[{Row}, {Col}]";

    public Point Down => this with { Row = Row + 1 };
    public Point Up => this with { Row = Row - 1 };
    public Point Left => this with { Col = Col - 1 };
    public Point Right => this with { Col = Col + 1 };

    public IEnumerable<Point> Neighbours(int cols)
    {
        yield return Left;
        yield return Left.Up;
        yield return Left.Down;
        
        var pos = Left;
        for (var i = 0; i < cols; i++)
        {
            pos = pos.Right;
            yield return pos.Up;
            yield return pos.Down;
        }

        yield return pos.Right;
        yield return pos.Right.Up;
        yield return pos.Right.Down;
    }
}