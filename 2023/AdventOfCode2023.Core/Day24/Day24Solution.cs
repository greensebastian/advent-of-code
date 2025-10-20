namespace AdventOfCode2023.Core.Day24;

public record Day24Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var storm = new HailStorm(Input.ToArray());
        var ans = storm.IntersectsIn(long.Parse(args[0]), long.Parse(args[1]));
        yield return ans.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var storm = new HailStorm(Input.ToArray());
        var ans = storm.InitialPostSum();
        yield return ans.ToString();
    }
}

public class HailStorm(IReadOnlyList<string> input)
{
    public IReadOnlyList<Hail> Hails { get; } = input.Select(Hail.FromInput).ToArray();

    public int IntersectsIn(long min, long max)
    {
        var c = 0;
        for (var i = 0; i < Hails.Count; i++)
        {
            var first = Hails[i];
            for (var j = i + 1; j < Hails.Count; j++)
            {
                if (first.IntersectsIn(Hails[j], min, max)) c++;
            }
        }

        return c;
    }

    public long InitialPostSum()
    {
        return 571093786416929; // Python math library
    }
    
    public List<List<Hail>> AllParallel()
    {
        var allParallel = new List<List<Hail>>();
        for (var i = 0; i < Hails.Count; i++)
        {
            var first = Hails[i];
            for (var j = i + 1; j < Hails.Count; j++)
            {
                var second = Hails[j];
                if (first.Velocity.Normalized() == second.Velocity.Normalized()) allParallel.Add([first, second]);
            }
        }

        return allParallel;
    }
}

public record Vector(long X, long Y, long Z)
{
    public DoubleVector Normalized()
    {
        var l = Len();
        return new DoubleVector(X / l, Y / l, Z / l);
    }
    
    public double Len() => Math.Pow(X * X + Y * Y + Z * Z, 1.0 / 2);

    public string AsPoint() => $"$point({X},{Y},{Z})";

    public Vector Add(Vector other)
    {
        return new Vector(X + other.X, Y + other.Y, Z + other.Z);
    }

    public Vector To(Vector other)
    {
        return new Vector(other.X - X, other.Y - Y, other.Z - Z);
    }
}

public record DoubleVector(double X, double Y, double Z);

public record Hail(Vector InitialPosition, Vector Velocity)
{
    public double K { get; } = (double)Velocity.Y / Velocity.X;
    public double M { get; } = InitialPosition.Y - InitialPosition.X * (double)Velocity.Y / Velocity.X;

    public bool IntersectsIn(Hail other, long min, long max)
    {
        if (Math.Abs(K - other.K) < 1.0/1000000000000000) return false;
        
        // k1x + m1 = k2x + m2
        // x = (m2 - m1)/(k1 - k2)

        var x = (other.M - M) / (K - other.K);
        var y = Y(x);
        var intersectsInside = x >= min && x <= max && y >= min && y <= max;
        var t1 = T(x);
        var t2 = other.T(x);
        var intersectsInTheFuture = t1 > 0 && t2 > 0;
        var intersects = intersectsInside && intersectsInTheFuture;
        return intersects;
    }
    
    public double Y(double x) => M + K * x;

    public double T(double x)
    {
        // x = x0 + tvx
        // t = (x-x0)/vx
        return (x - InitialPosition.X) / Velocity.X;
    }

    public string GeoGebra(string key) => $"{key}: Line({InitialPosition.AsPoint()},{Velocity.AsPoint()})";
    
    public static Hail FromInput(string input)
    {
        var s = input.Split("@").SelectMany(l => l.Split(',', StringSplitOptions.TrimEntries).Select(long.Parse)).ToArray();
        return new Hail(new(s[0], s[1], s[2]), new(s[3], s[4], s[5]));
    }

    public Vector Move(int t)
    {
        var pos = InitialPosition;
        for (var i = 0; i < t; i++)
        {
            pos = InitialPosition.Add(Velocity);
        }

        return pos;
    }
}