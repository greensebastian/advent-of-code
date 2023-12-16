using System.Diagnostics;
using System.Text;

namespace AdventOfCode2023.Core.Day14;

public record Day14Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var tilt = new Point(0, 0).North;
        var platform = Platform.FromInput(Input.ToArray());
        platform.Tilt(tilt);
        var score = platform.Load();
        yield return score.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var platform = Platform.FromInput(Input.ToArray());
        var load = platform.LoadAt(1000000000 * 4L, Log);
        yield return load.ToString();
    }
}

public enum Block
{
    None, Solid, Moving
}

[DebuggerDisplay("{ToString()}")]
public record Platform(Block[][] Blocks)
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var row in Blocks)
        {
            sb.AppendLine(string.Join("", row.Select(c => c == Block.Solid ? '#' : c == Block.Moving ? 'O' : '.')));
        }

        return sb.ToString();
    }

    public int LoadAt(long target, Action<string> log)
    {
        var zeroPoint = new Point(0, 0);
        var cache = new Dictionary<string, int>();
        var i = 0;
        while (true)
        {
            var dir = (i % 4) switch
            {
                0 => zeroPoint.North,
                1 => zeroPoint.West,
                2 => zeroPoint.South,
                3 => zeroPoint.East
            };
            Tilt(dir);
            i++;
            log(i.ToString());
            log(ToString());
            var key = ToString();
            if (cache.ContainsKey(key)) break;
            cache[key] = i;
        }

        var start = cache[ToString()];
        var cycleLength = i - start;

        var cycleTarget = (target - start) % cycleLength;
        while (true)
        {
            var cycle = (i - start) % cycleLength;
            if (cycle == cycleTarget) break;
            var dir = (i % 4) switch
            {
                0 => zeroPoint.North,
                1 => zeroPoint.West,
                2 => zeroPoint.South,
                3 => zeroPoint.East
            };
            Tilt(dir);
            i++;
        }

        return Load();
    }

    public void Tilt(Point dir)
    {
        if (dir == new Point(0, 0).North)
        {
            for (var col = 0; col < Blocks[0].Length; col++)
            {
                var row = 0;
                var newFirstMoving = row;
                var movingCount = 0;
                while (row <= Blocks.Length)
                {
                    if (row == Blocks.Length || Blocks[row][col] == Block.Solid)
                    {
                        // Shift to prev solid
                        for (var rowToAdd = newFirstMoving; rowToAdd < newFirstMoving + movingCount; rowToAdd++)
                        {
                            Blocks[rowToAdd][col] = Block.Moving;
                        }

                        newFirstMoving = row + 1;
                        movingCount = 0;
                    }
                    else
                    {
                        if (Blocks[row][col] == Block.Moving) movingCount++;
                        Blocks[row][col] = Block.None;
                    }

                    row++;
                }
            }
        }
        else if (dir == new Point(0, 0).South)
        {
            for (var col = 0; col < Blocks[0].Length; col++)
            {
                var row = Blocks.Length - 1;
                var newFirstMoving = row;
                var movingCount = 0;
                while (row >= -1)
                {
                    if (row == -1 || Blocks[row][col] == Block.Solid)
                    {
                        // Shift to prev solid
                        for (var rowToAdd = newFirstMoving; rowToAdd > newFirstMoving - movingCount; rowToAdd--)
                        {
                            Blocks[rowToAdd][col] = Block.Moving;
                        }

                        newFirstMoving = row - 1;
                        movingCount = 0;
                    }
                    else
                    {
                        if (Blocks[row][col] == Block.Moving) movingCount++;
                        Blocks[row][col] = Block.None;
                    }

                    row--;
                }
            }
        }
        else if (dir == new Point(0, 0).West)
        {
            for (var row = 0; row < Blocks.Length; row++)
            {
                var col = 0;
                var newFirstMoving = col;
                var movingCount = 0;
                while (col <= Blocks[row].Length)
                {
                    if (col == Blocks[row].Length || Blocks[row][col] == Block.Solid)
                    {
                        // Shift to prev solid
                        for (var colToAdd = newFirstMoving; colToAdd < newFirstMoving + movingCount; colToAdd++)
                        {
                            Blocks[row][colToAdd] = Block.Moving;
                        }

                        newFirstMoving = col + 1;
                        movingCount = 0;
                    }
                    else
                    {
                        if (Blocks[row][col] == Block.Moving) movingCount++;
                        Blocks[row][col] = Block.None;
                    }

                    col++;
                }
            }
        }
        else if (dir == new Point(0, 0).East)
        {
            for (var row = 0; row < Blocks.Length; row++)
            {
                var col = Blocks[row].Length - 1;
                var newFirstMoving = col;
                var movingCount = 0;
                while (col >= -1)
                {
                    if (col == -1 || Blocks[row][col] == Block.Solid)
                    {
                        // Shift to prev solid
                        for (var colToAdd = newFirstMoving; colToAdd > newFirstMoving - movingCount; colToAdd--)
                        {
                            Blocks[row][colToAdd] = Block.Moving;
                        }

                        newFirstMoving = col - 1;
                        movingCount = 0;
                    }
                    else
                    {
                        if (Blocks[row][col] == Block.Moving) movingCount++;
                        Blocks[row][col] = Block.None;
                    }

                    col--;
                }
            }
        }
    }

    public int Load()
    {
        var sum = 0;
        for (var row = 0; row < Blocks.Length; row++)
        {
            for (var col = 0; col < Blocks[row].Length; col++)
            {
                if (Blocks[row][col] == Block.Moving)
                {
                    sum += Blocks.Length - row;
                }
            }
        }

        return sum;
    }
    
    public static Platform FromInput(IList<string> lines)
    {
        var blocks = new Block[lines.Count][];
        
        // Parse
        for (var row = 0; row < lines.Count; row++)
        {
            blocks[row] = new Block[lines[row].Length];
            for (var col = 0; col < lines[0].Length; col++)
            {
                blocks[row][col] = lines[row][col] switch
                {
                    '#' => Block.Solid,
                    'O' => Block.Moving,
                    _ => Block.None
                };
            }
        }

        return new Platform(blocks);
    }
}

public record Point(int Row, int Col)
{
    public Point South => this with { Row = Row + 1 };
    public Point North => this with { Row = Row - 1 };
    public Point West => this with { Col = Col - 1 };
    public Point East => this with { Col = Col + 1 };
    public Point Add(Point other) => new(Row + other.Row, Col + other.Col);
    public Point Sub(Point other) => new(Row - other.Row, Col - other.Col);
}