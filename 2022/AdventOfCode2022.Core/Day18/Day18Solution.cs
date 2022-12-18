using System.Globalization;

namespace AdventOfCode2022.Core.Day18;

public record Day18Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var cloud = new LavaCloud();
        foreach (var line in Input)
        {
            cloud.AddDroplet(Vector3.From(line));
        }

        yield return cloud.ExposedArea.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var cloud = new LavaCloud();
        foreach (var line in Input)
        {
            cloud.AddDroplet(Vector3.From(line));
        }

        yield return cloud.GetExposedExternalArea().ToString();
    }
}

public class LavaCloud
{
    private HashSet<Vector3> Droplets { get; } = new();
    public int ExposedArea { get; private set; } = 0;

    public void AddDroplet(Vector3 position)
    {
        var areaDelta = 6;
        foreach (var neighbourPosition in position.GetNeighbours())
        {
            if (Droplets.Contains(neighbourPosition))
                areaDelta -= 2;
        }

        Droplets.Add(position);

        ExposedArea += areaDelta;
    }

    public int GetExposedExternalArea()
    {
        var enclosed = GetEnclosedCubes();
        return -1;
    }
    
    public IEnumerable<Vector3> GetEnclosedCubes()
    {
        var minX = Droplets.MinBy(d => d.X);
        var maxX = Droplets.MaxBy(d => d.X);
        var minY = Droplets.MinBy(d => d.Y);
        var maxY = Droplets.MaxBy(d => d.Y);
        var minZ = Droplets.MinBy(d => d.Z);
        var maxZ = Droplets.MaxBy(d => d.Z);
        
        yield break;
    }
}

public record struct Vector3(int X, int Y, int Z)
{
    private Vector3 XUp => this with { X = X + 1 };
    private Vector3 XDown => this with { X = X - 1 };
    private Vector3 YUp => this with { Y = Y + 1 };
    private Vector3 YDown => this with { Y = Y - 1 };
    private Vector3 ZUp => this with { Z = Z + 1 };
    private Vector3 ZDown => this with { Z = Z - 1 };

    public IEnumerable<Vector3> GetNeighbours()
    {
        yield return XUp;
        yield return XDown;
        yield return YUp;
        yield return YDown;
        yield return ZUp;
        yield return ZDown;
    }

    public static Vector3 From(string input)
    {
        var ints = input.Ints().ToArray();
        return new Vector3(ints[0], ints[1], ints[2]);
    }

    public override string ToString() => $"[{X}, {Y}, {Z}]";
}

internal static class EnumerableExtensions
{
    public static IEnumerable<int> Ints(this IEnumerable<char> source)
    {
        var currentNumber = string.Empty;
        foreach (var c in source)
        {
            if (char.IsNumber(c) || c == '-')
            {
                currentNumber += c;
            }
            else
            {
                if (string.IsNullOrEmpty(currentNumber)) continue;
                
                yield return int.Parse(currentNumber);
                currentNumber = string.Empty;
            }
        }

        if (currentNumber.Length > 0) yield return int.Parse(currentNumber);
    }
}