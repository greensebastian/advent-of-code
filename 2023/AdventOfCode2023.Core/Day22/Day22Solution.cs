using System.Text.RegularExpressions;

namespace AdventOfCode2023.Core.Day22;

public record Day22Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var lines = Input.ToArray();
        var map = new Map(lines);
        var player = new Player(map, new LinearWrapper(map));
        player.Move(lines.Last());

        yield return player.Score.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var lines = Input.ToArray();
        var map = new Map(lines);
        var player = new Player(map, new CubeWrapper(map, args[0]));
        player.Move(lines.Last());

        yield return player.Score.ToString();
    }
}

public class Player
{
    private Map Map { get; }
    public int Side { get; private set; } = 1;
    private IWrapper Wrapper { get; }
    private static readonly Regex MoveSetRegex = new("([A-Z])|([0-9]+)", RegexOptions.Compiled);
    private Vector Position { get; set; }
    public Vector Direction { get; private set; } = Vector.DirRight;
    private long DirectionScore
    {
        get
        {
            if (Direction == Vector.DirRight) return 0;
            if (Direction == Vector.DirDown) return 1;
            if (Direction == Vector.DirLeft) return 2;
            if (Direction == Vector.DirUp) return 3;
            throw new ArgumentOutOfRangeException();
        }
    }
    public long Score => 1000 * Position.Row + 4 * Position.Col + DirectionScore;
    public Player(Map map, IWrapper wrapper)
    {
        Map = map;
        Wrapper = wrapper;
        Position = map.Tiles
            .Where(kv => kv is { Value: Item.Open, Key.Row: 1 })
            .MinBy(kv => kv.Key.Col)
            .Key;
    }

    public void Move(string path)
    {
        var matches = MoveSetRegex
            .Matches(path)
            .ToArray();
        var moves = matches
            .Select(m => m.Value)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToArray();
        foreach (var move in moves)
        {
            if (int.TryParse(move, out var moveLength))
            {
                MoveInCurrentDirection(moveLength);
            }
            else
            {
                Turn(move);
            }
        }
    }

    private void Turn(string direction)
    {
        if (direction == "L")
        {
            if (Direction == Vector.DirRight)
                Direction = Vector.DirUp;
            else if (Direction == Vector.DirUp)
                Direction = Vector.DirLeft;
            else if (Direction == Vector.DirLeft)
                Direction = Vector.DirDown;
            else if (Direction == Vector.DirDown)
                Direction = Vector.DirRight;
            else
                throw new ArgumentOutOfRangeException();
        }
        else if (direction == "R")
        {
            if (Direction == Vector.DirRight)
                Direction = Vector.DirDown;
            else if (Direction == Vector.DirUp)
                Direction = Vector.DirRight;
            else if (Direction == Vector.DirLeft)
                Direction = Vector.DirUp;
            else if (Direction == Vector.DirDown)
                Direction = Vector.DirLeft;
            else
                throw new ArgumentOutOfRangeException();
        }
        else
            throw new ArgumentOutOfRangeException();
    }

    private void MoveInCurrentDirection(int moveLength)
    {
        for (var i = 0; i < moveLength; i++)
        {
            var newPos = Position.Move(Direction);
            
            (newPos, var newDirection, var newSide) = Wrapper.Wrap(newPos, this);

            if (Map.Tiles.TryGetValue(newPos, out var item))
            {
                if (item == Item.Wall)
                    break;
            }
            else
            {
                throw new Exception("what");
            }
            Position = newPos;
            Direction = newDirection;
            Side = newSide;
        }
    }
}

public interface IWrapper
{
    PosDirSide Wrap(Vector newPos, Player player);
}

public class LinearWrapper : IWrapper
{
    private Map Map { get; }

    public LinearWrapper(Map map)
    {
        Map = map;
    }
    
    public PosDirSide Wrap(Vector newPos, Player player)
    {
        if (Map.Tiles.ContainsKey(newPos)) return new PosDirSide(newPos, player.Direction, player.Side);
        
        if (player.Direction == Vector.DirRight)
            newPos = Map.FirstOnRow(newPos.Row);
        else if (player.Direction == Vector.DirLeft)
            newPos = Map.LastOnRow(newPos.Row);
        else if (player.Direction == Vector.DirDown)
            newPos = Map.FirstInCol(newPos.Col);
        else if (player.Direction == Vector.DirUp)
            newPos = Map.LastInCol(newPos.Col);
        else
            throw new ArgumentOutOfRangeException();

        return new PosDirSide(newPos, player.Direction, 0);
    }
}

public record PosDirSide(Vector Position, Vector Direction, int Side);

public record Overflow(int From, Vector NewPosition);

public class CubeWrapper : IWrapper
{
    private Map Map { get; }
    private Dictionary<Overflow, PosDirSide> Links { get; }
    private long SideLength { get; }
    public CubeWrapper(Map map, string key)
    {
        Map = map;
        SideLength = GetSideLength();
        Links = GetLinks(key);
    }
    
    public PosDirSide Wrap(Vector newPos, Player player)
    {
        var overflow = new Overflow(player.Side, newPos);
        if (Links.TryGetValue(overflow, out var wrapped))
        {
            return wrapped;
        }

        return new PosDirSide(newPos, player.Direction, player.Side);
    }

    private Dictionary<Overflow, PosDirSide> GetLinks(string key)
    {
        var links = new Dictionary<Overflow, PosDirSide>();
        if (key == "example")
        {
            // 1 -> 2
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(2, 0),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirDown,
                    Side = 1
                },
                new OverflowLine
                {
                    Lanes = new Vector(2, 1),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirUp,
                    Side = 2
                },
                links);

            // 1 -> 3
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(2, 0),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirLeft,
                    Side = 1
                },
                new OverflowLine
                {
                    Lanes = new Vector(1, 1),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirUp,
                    Side = 3
                },
                links);

            // 1 -> 4
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(2, 0),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirRight,
                    Side = 1
                },
                new OverflowLine
                {
                    Lanes = new Vector(3, 2),
                    Delta = Vector.DirUp,
                    OverflowDir = Vector.DirRight,
                    Side = 4
                },
                links);

            // 1 -> 5
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(2, 0),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirUp,
                    Side = 1
                },
                new OverflowLine
                {
                    Lanes = new Vector(0, 1),
                    Delta = Vector.DirLeft,
                    OverflowDir = Vector.DirUp,
                    Side = 5
                },
                links);

            // 6 -> 2
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(2, 2),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirUp,
                    Side = 6
                },
                new OverflowLine
                {
                    Lanes = new Vector(2, 1),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirDown,
                    Side = 2
                },
                links);

            // 6 -> 3
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(2, 2),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirLeft,
                    Side = 6
                },
                new OverflowLine
                {
                    Lanes = new Vector(1, 1),
                    Delta = Vector.DirLeft,
                    OverflowDir = Vector.DirDown,
                    Side = 3
                },
                links);

            // 6 -> 4
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(2, 2),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirRight,
                    Side = 6
                },
                new OverflowLine
                {
                    Lanes = new Vector(3, 2),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirLeft,
                    Side = 4
                },
                links);

            // 6 -> 5
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(2, 2),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirDown,
                    Side = 6
                },
                new OverflowLine
                {
                    Lanes = new Vector(0, 1),
                    Delta = Vector.DirLeft,
                    OverflowDir = Vector.DirDown,
                    Side = 5
                },
                links);

            // 2 -> 4
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(2, 1),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirRight,
                    Side = 2
                },
                new OverflowLine
                {
                    Lanes = new Vector(3, 2),
                    Delta = Vector.DirLeft,
                    OverflowDir = Vector.DirUp,
                    Side = 4
                },
                links);

            // 4 -> 5
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(3, 2),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirDown,
                    Side = 4
                },
                new OverflowLine
                {
                    Lanes = new Vector(3, 2),
                    Delta = Vector.DirUp,
                    OverflowDir = Vector.DirLeft,
                    Side = 5
                },
                links);

            // 5 -> 3
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(0, 1),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirRight,
                    Side = 5
                },
                new OverflowLine
                {
                    Lanes = new Vector(1, 1),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirLeft,
                    Side = 3
                },
                links);

            // 3 => 2
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(1, 1),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirRight,
                    Side = 3
                },
                new OverflowLine
                {
                    Lanes = new Vector(2, 1),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirLeft,
                    Side = 2
                },
                links);
        }
        
        if (key == "real")
        {
            //     11114444
            //     11114444
            //     11114444
            //     11114444
            //     2222
            //     2222
            //     2222
            //     2222
            // 33336666
            // 33336666
            // 33336666
            // 33336666
            // 5555
            // 5555
            // 5555
            // 5555

            // 1 -> 2
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(1, 0),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirDown,
                    Side = 1
                },
                new OverflowLine
                {
                    Lanes = new Vector(1, 1),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirUp,
                    Side = 2
                },
                links);

            // 1 -> 3
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(1, 0),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirLeft,
                    Side = 1
                },
                new OverflowLine
                {
                    Lanes = new Vector(0, 2),
                    Delta = Vector.DirUp,
                    OverflowDir = Vector.DirLeft,
                    Side = 3
                },
                links);

            // 1 -> 4
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(1, 0),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirRight,
                    Side = 1
                },
                new OverflowLine
                {
                    Lanes = new Vector(2, 0),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirLeft,
                    Side = 4
                },
                links);

            // 1 -> 5
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(1, 0),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirUp,
                    Side = 1
                },
                new OverflowLine
                {
                    Lanes = new Vector(0, 3),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirLeft,
                    Side = 5
                },
                links);

            // 6 -> 2
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(1, 2),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirUp,
                    Side = 6
                },
                new OverflowLine
                {
                    Lanes = new Vector(1, 1),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirDown,
                    Side = 2
                },
                links);

            // 6 -> 3
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(1, 2),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirLeft,
                    Side = 6
                },
                new OverflowLine
                {
                    Lanes = new Vector(0, 2),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirRight,
                    Side = 3
                },
                links);

            // 6 -> 4
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(1, 2),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirRight,
                    Side = 6
                },
                new OverflowLine
                {
                    Lanes = new Vector(2, 0),
                    Delta = Vector.DirUp,
                    OverflowDir = Vector.DirRight,
                    Side = 4
                },
                links);

            // 6 -> 5
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(1, 2),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirDown,
                    Side = 6
                },
                new OverflowLine
                {
                    Lanes = new Vector(0, 3),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirRight,
                    Side = 5
                },
                links);

            // 2 -> 4
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(1, 1),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirRight,
                    Side = 2
                },
                new OverflowLine
                {
                    Lanes = new Vector(2, 0),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirDown,
                    Side = 4
                },
                links);

            // 4 -> 5
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(2, 0),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirUp,
                    Side = 4
                },
                new OverflowLine
                {
                    Lanes = new Vector(0, 3),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirDown,
                    Side = 5
                },
                links);

            // 5 -> 3
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(0, 3),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirUp,
                    Side = 5
                },
                new OverflowLine
                {
                    Lanes = new Vector(0, 2),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirDown,
                    Side = 3
                },
                links);

            // 3 => 2
            AddOverflow(new OverflowLine
                {
                    Lanes = new Vector(0, 2),
                    Delta = Vector.DirRight,
                    OverflowDir = Vector.DirUp,
                    Side = 3
                },
                new OverflowLine
                {
                    Lanes = new Vector(1, 1),
                    Delta = Vector.DirDown,
                    OverflowDir = Vector.DirLeft,
                    Side = 2
                },
                links);
        }
        return links;
    }

    private void AddOverflow(OverflowLine first, OverflowLine second, Dictionary<Overflow, PosDirSide> links)
    {
        var firstPos = FindStart(first);
        var secondPos = FindStart(second);
        
        for (var d = 0; d < SideLength; d++)
        {
            links[new Overflow(first.Side, firstPos.Move(first.OverflowDir))] =
                new PosDirSide(secondPos, second.OverflowDir.Inverse, second.Side);
            links[new Overflow(second.Side, secondPos.Move(second.OverflowDir))] =
                new PosDirSide(firstPos, first.OverflowDir.Inverse, first.Side);

            firstPos = firstPos.Move(first.Delta);
            secondPos = secondPos.Move(second.Delta);
        }
    }

    private Vector FindStart(OverflowLine face)
    {
        var width = SideLength - 1;
        var firstPos = new Vector(face.Lanes.Col * SideLength + 1, face.Lanes.Row * SideLength + 1);
        if (face.OverflowDir == Vector.DirUp)
        {
            if (face.Delta == Vector.DirLeft)
                firstPos = firstPos.Move(width, 0);
        }
        else if (face.OverflowDir == Vector.DirDown)
        {
            if (face.Delta == Vector.DirLeft)
                firstPos = firstPos.Move(width, width);
            else
                firstPos = firstPos.Move(0, width);
        }
        else if (face.OverflowDir == Vector.DirRight)
        {
            if (face.Delta == Vector.DirDown)
                firstPos = firstPos.Move(width, 0);
            else
                firstPos = firstPos.Move(width, width);
        }
        else if (face.OverflowDir == Vector.DirLeft)
        {
            if (face.Delta == Vector.DirUp)
                firstPos = firstPos.Move(0, width);
        }
        else
            throw new ArgumentOutOfRangeException();

        return firstPos;
    }

    private long GetSideLength()
    {
        var gcf = (long?)null;
        foreach (var rowGroup in Map.Tiles.Keys.GroupBy(v => v.Row))
        {
            var min = rowGroup.Min(v => v.Col);
            var max = rowGroup.Max(v => v.Col);
            var width = max - min + 1;
            
            gcf = gcf.HasValue ? GreatestCommonFactor(gcf.Value, width) : width;
        }

        return gcf!.Value;
    }
    
    private static long GreatestCommonFactor(long a, long b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}

public class OverflowLine
{
    public required Vector Lanes { get; init; }
    public required Vector OverflowDir { get; init; }
    public required Vector Delta { get; init; }
    public required int Side { get; init; }
}

public class Map
{
    public IReadOnlyDictionary<Vector, Item> Tiles { get; }
    public Vector GlobalMax { get; }
    public Vector GlobalMin { get; }

    public Map(string[] lines)
    {
        var maxRow = 1;
        var maxCol = 1;
        var tiles = new Dictionary<Vector, Item>();
        for (var rowIndex = 0; rowIndex < lines.Length; rowIndex++)
        {
            var row = lines[rowIndex];
            if (string.IsNullOrWhiteSpace(row)) break;
            for (var colIndex = 0; colIndex < row.Length; colIndex++)
            {
                maxRow = rowIndex + 1 > maxRow ? rowIndex + 1 : maxRow;
                maxCol = colIndex + 1 > maxCol ? colIndex + 1 : maxCol;
                
                var c = row[colIndex];
                switch (c)
                {
                    case '#':
                        tiles[new Vector(colIndex + 1, rowIndex + 1)] = Item.Wall;
                        break;
                    case '.':
                        tiles[new Vector(colIndex + 1, rowIndex + 1)] = Item.Open;
                        break;
                }
            }
        }

        GlobalMin = new Vector(1, 1);
        GlobalMax = new Vector(maxCol, maxRow);
        Tiles = tiles;
    }

    public Vector FirstOnRow(long row) => Tiles
        .Where(kv => kv.Key.Row == row)
        .MinBy(kv => kv.Key.Col)
        .Key;
    
    public Vector LastOnRow(long row) => Tiles
        .Where(kv => kv.Key.Row == row)
        .MaxBy(kv => kv.Key.Col)
        .Key;
    
    public Vector FirstInCol(long col) => Tiles
        .Where(kv => kv.Key.Col == col)
        .MinBy(kv => kv.Key.Row)
        .Key;
    
    public Vector LastInCol(long col) => Tiles
        .Where(kv => kv.Key.Col == col)
        .MaxBy(kv => kv.Key.Row)
        .Key;
}

public record Vector(long Col, long Row)
{
    public Vector Down => this with { Row = Row + 1 };
    public Vector Up => this with { Row = Row - 1 };
    public Vector Left => this with { Col = Col - 1 };
    public Vector Right => this with { Col = Col + 1 };
    public Vector To(Vector other) => new(other.Col - Col, other.Row - Row);
    public Vector Move(Vector other) => new(Col + other.Col, Row + other.Row);
    public Vector Move(long col, long row) => new(Col + col, Row + row);
    public Vector Inverse => new(-Col, -Row);

    public static readonly Vector DirRight = new(1, 0);
    public static readonly Vector DirLeft = new(-1, 0);
    public static readonly Vector DirUp = new(0, -1);
    public static readonly Vector DirDown = new(0, 1);

    public override string ToString() => $"Row: {Row}, Col: {Col}";
}

public enum Item
{
    Open,
    Wall
}