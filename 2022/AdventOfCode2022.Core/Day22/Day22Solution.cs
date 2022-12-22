using System.Text.RegularExpressions;

namespace AdventOfCode2022.Core.Day22;

public record Day22Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var lines = Input.ToArray();
        var map = new Map(lines);
        var player = new Player(map);
        player.Move(lines.Last());

        yield return player.Score.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return "0";
    }
}

public class Player
{
    private Map Map { get; }
    private static readonly Regex MoveSetRegex = new("([A-Z])|([0-9]+)", RegexOptions.Compiled);
    private Vector Position { get; set; }
    private Vector Direction { get; set; } = Vector.DirRight;
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
    public Player(Map map)
    {
        Map = map;
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
            if (!Map.Tiles.ContainsKey(newPos))
            {
                if (Direction == Vector.DirRight)
                    newPos = Map.FirstOnRow(newPos.Row);
                else if (Direction == Vector.DirLeft)
                    newPos = Map.LastOnRow(newPos.Row);
                else if (Direction == Vector.DirDown)
                    newPos = Map.FirstInCol(newPos.Col);
                else if (Direction == Vector.DirUp)
                    newPos = Map.LastInCol(newPos.Col);
                else
                    throw new ArgumentOutOfRangeException();
            }

            if (Map.Tiles[newPos] == Item.Wall)
                break;
            Position = newPos;
        }
    }
}

public class Map
{
    public IReadOnlyDictionary<Vector, Item> Tiles { get; }

    public Map(string[] lines)
    {
        var tiles = new Dictionary<Vector, Item>();
        for (var rowIndex = 0; rowIndex < lines.Length; rowIndex++)
        {
            var row = lines[rowIndex];
            if (string.IsNullOrWhiteSpace(row)) break;
            for (var colIndex = 0; colIndex < row.Length; colIndex++)
            {
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

public record struct Vector(long Col, long Row)
{
    public Vector Down => this with { Row = Row + 1 };
    public Vector Up => this with { Row = Row - 1 };
    public Vector Left => this with { Col = Col - 1 };
    public Vector Right => this with { Col = Col + 1 };
    public Vector To(Vector other) => new(other.Col - Col, other.Row - Row);
    public Vector Move(Vector other) => new(Col + other.Col, Row + other.Row);

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