namespace AdventOfCode2022.Core.Day09;

public record Day09Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var rope = new LongRope(2, Log);
        foreach (var line in Input)
        {
            rope.Move(line[0], int.Parse(line[2..]));
        }

        yield return rope.Visited.Count.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var rope = new LongRope(10, Log);
        foreach (var line in Input)
        {
            rope.Move(line[0], int.Parse(line[2..]));
        }

        yield return rope.Visited.Count.ToString();
    }
}

public class LongRope
{
    public Action<string> Log { get; }

    public LongRope(int length, Action<string> log)
    {
        Log = log;
        for (var i = 0; i < length; i++)
        {
            Rope.Add(new Vector(0, 0));
        }
    }

    private Vector Head => Rope.First();
    private Vector Tail => Rope.Last();
    private List<Vector> Rope { get; } = new ();

    public readonly HashSet<Vector> Visited = new() { new Vector(0, 0) };

    private void MarkVisited(Vector v) => Visited.Add(v);

    public void Move(char dir, int length)
    {
        Log.Invoke($"Move: {dir} {length}");
        for (var count = 0; count < length; count++)
        {
            switch (dir)
            {
                case 'U':
                    Rope[0] = Rope[0] with { Y = Rope[0].Y + 1 };
                    break;
                case 'R':
                    Rope[0] = Rope[0] with { X = Rope[0].X + 1 };
                    break;
                case 'D':
                    Rope[0] = Rope[0] with { Y = Rope[0].Y - 1 };
                    break;
                case 'L':
                    Rope[0] = Rope[0] with { X = Rope[0].X - 1 };
                    break;
            }
            FollowWithTail();
        }
    }

    private void FollowWithTail()
    {
        for (var currentIndex = 1; currentIndex < Rope.Count; currentIndex++)
        {
            var current = Rope[currentIndex];
            var previous = Rope[currentIndex - 1];
            var diff = previous.Subtract(current);

            var moves = diff.MovesToZeroAdjacent().ToList();
            foreach (var move in moves)
            {
                Rope[currentIndex] = Rope[currentIndex].Move(move);
            }
        }
        
        MarkVisited(Rope.Last());
        
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
        for (var y = maxY; y >= minY; y--)
        {
            Console.Write($"{y}:\t");
            for (var x = minX; x <= maxX; x++)
            {
                var painted = false;
                if (Head.X == x && Head.Y == y)
                {
                    Console.Write("H");
                    continue;
                }

                for (var partIndex = 1; partIndex < Rope.Count - 1; partIndex++)
                {
                    if (Rope[partIndex].X == x && Rope[partIndex].Y == y)
                    {
                        Console.Write(partIndex.ToString());
                        painted = true;
                        break;
                    }
                }
                if (painted) continue;

                if (Tail.X == x && Tail.Y == y)
                    Console.Write("T");
                else if (Visited.Contains(new Vector(x, y)))
                    Console.Write("#");
                else
                    Console.Write(".");
            }
            Log.Invoke(string.Empty);
        }
        Log.Invoke(string.Empty);
    }
}

public record Vector(int X, int Y)
{
    private int AbsX => Math.Abs(X);
    private int AbsY => Math.Abs(Y);
    private int XSign => X >= 0 ? 1 : -1;
    private int YSign => Y >= 0 ? 1 : -1;
    private bool ZeroAdjacent => AbsX < 2 && AbsY < 2;
    public Vector Subtract(Vector other) => new Vector(other.X - X, other.Y - Y);

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
    
    public Vector Move(Vector v) => new(X + v.X, Y + v.Y);
}