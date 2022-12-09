namespace AdventOfCode2022.Core.Day09;

public record Day09Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var rope = new Rope();
        foreach (var line in Input)
        {
            rope.Move(line[0], int.Parse(line[2..]));
        }

        yield return rope.Visited.Count.ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        yield return "0";
    }
}

public class Rope
{
    public Vector Head { get; private set; } = new Vector(0, 0);
    public Vector Tail { get; private set; } = new Vector(0, 0);
    public Vector PrevHead { get; private set; } = new Vector(0, 0);

    public HashSet<Vector> Visited = new() { new Vector(0, 0) };

    private void MarkVisited(Vector v) => Visited.Add(v);

    public void Move(char dir, int length)
    {
        Console.WriteLine($"Move: {dir} {length}");
        for (var count = 0; count < length; count++)
        {
            PrevHead = Head;
            switch (dir)
            {
                case 'U':
                    Head = Head with { Y = Head.Y + 1 };
                    break;
                case 'R':
                    Head = Head with { X = Head.X + 1 };
                    break;
                case 'D':
                    Head = Head with { Y = Head.Y - 1 };
                    break;
                case 'L':
                    Head = Head with { X = Head.X - 1 };
                    break;
            }
            FollowWithTail();
        }
    }

    public double MaxDiff { get; private set; } = 0;

    private void FollowWithTail()
    {
        /*var diff = Head.Subtract(Tail);
        MaxDiff = diff.Magnitude > MaxDiff ? diff.Magnitude : MaxDiff;
        var moves = diff.MovesToZeroAdjacent().ToList();
        foreach (var move in moves)
        {
            Print();
            Tail = Tail.Move(move);
            MarkVisited(Tail);
        }*/

        var diff = Head.Subtract(Tail);
        if (!diff.ZeroAdjacent)
        {
            Tail = PrevHead;
            MarkVisited(Tail);
        }

        Print();
    }

    void Print()
    {
        return;
        var printMargin = 5;
        var minX = Math.Min(Head.X, Head.X) - printMargin;
        var maxX = Math.Max(Head.X, Tail.X) + printMargin;
        var minY = Math.Min(Head.Y, Tail.Y) - printMargin;
        var maxY = Math.Max(Head.Y, Tail.Y) + printMargin;
        for (int y = maxY; y >= minY; y--)
        {
            Console.Write($"{y}:\t");
            for (int x = minX; x <= maxX; x++)
            {
                if (Head.X == x && Head.Y == y)
                    Console.Write("H");
                else if (Tail.X == x && Tail.Y == y)
                    Console.Write("T");
                else if (Visited.Contains(new Vector(x, y)))
                    Console.Write("#");
                else
                    Console.Write(".");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}

public record Vector(int X, int Y)
{
    public int AbsX => Math.Abs(X);
    public int AbsY => Math.Abs(Y);
    public int XSign => X >= 0 ? 1 : -1;
    public int YSign => Y >= 0 ? 1 : -1;
    
    public Vector Subtract(Vector other) => new Vector(other.X - X, other.Y - Y);

    public double Magnitude => Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));

    public IEnumerable<Vector> MovesToZeroAdjacent()
    {
        var cur = this;
        while (!cur.ZeroAdjacent)
        {
            var move = new Vector(cur.AbsX > 0 ? -cur.XSign : 0, cur.AbsY > 0 ? -cur.YSign : 0);
            yield return move;
            cur = cur.Move(move);
        }
    }

    public bool ZeroAdjacent => AbsX < 2 && AbsY < 2;

    public Vector Move(Vector v) => new(X + v.X, Y + v.Y);
}