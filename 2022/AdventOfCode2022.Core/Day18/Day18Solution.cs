using System.Globalization;

namespace AdventOfCode2022.Core.Day18;

public record Day18Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var cloud = new LavaCloud();
        foreach (var line in Input)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            cloud.AddDroplet(Vector3.From(line));
        }

        yield return cloud.ExposedArea.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var cloud = new LavaCloud();
        foreach (var line in Input)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            cloud.AddDroplet(Vector3.From(line));
        }
        
        cloud.Print();

        yield return cloud.GetExposedExternalArea().ToString();
    }
}

public class LavaCloud
{
    private HashSet<Vector3> Droplets { get; } = new();
    public int ExposedArea { get; private set; } = 0;
    private CloudLimits Limits { get; } = new();

    public void AddDroplet(Vector3 position)
    {
        var areaDelta = 6;
        foreach (var neighbourPosition in position.GetNeighbours())
        {
            if (Droplets.Contains(neighbourPosition))
                areaDelta -= 2;
        }

        Limits.UpdateLimitsOnXY(position.XY, position.Z);
        Limits.UpdateLimitsOnXZ(position.XZ, position.Y);
        Limits.UpdateLimitsOnZY(position.ZY, position.X);

        Droplets.Add(position);

        ExposedArea += areaDelta;
    }

    public int GetExposedExternalArea()
    {
        var enclosedCubes = GetEnclosedCubes().ToList();
        var area = ExposedArea;

        foreach (var droplet in enclosedCubes)
        {
            foreach (var neighbour in droplet.GetNeighbours())
            {
                if (Droplets.Contains(neighbour)) area--;
            }
        }
        return area;
    }

    private IEnumerable<Vector3> GetEnclosedCubes()
    {
        var potentialEmpty = new HashSet<Vector3>();
        var knownOpen = new HashSet<Vector3>();
        
        var (min, max) = Limits.GetGlobalLimits();

        for (var x = min.X - 1; x <= max.X + 1; x++)
        {
            for (var y = min.Y - 1; y <= max.Y + 1; y++)
            {
                var tentativeMissing = new List<Vector3>();
                var seenSolid = false;
                for (var z = min.Z - 1; z <= max.Z + 1; z++)
                {
                    var cur = new Vector3(x, y, z);

                    if (cur.X == min.X) knownOpen.Add(cur.XDown);
                    if (cur.X == max.X) knownOpen.Add(cur.XUp);
                    if (cur.Y == min.Y) knownOpen.Add(cur.YDown);
                    if (cur.Y == max.Y) knownOpen.Add(cur.YUp);
                    if (cur.Z == min.Z) knownOpen.Add(cur.ZDown);
                    if (cur.Z == max.Z) knownOpen.Add(cur.ZUp);

                    if (!Droplets.Contains(cur))
                    {
                        if (!seenSolid) continue;
                        if (!Limits.WithinLimits(cur)) continue;
                        tentativeMissing.Add(cur);
                    }
                    else if (seenSolid)
                    {
                        // Add tentative to actual void
                        foreach (var droplet in tentativeMissing)
                        {
                            potentialEmpty.Add(droplet);
                        }

                        tentativeMissing = new List<Vector3>();
                    }
                    else
                    {
                        seenSolid = true;
                    }
                }
            }
        }

        var knownClosed = new HashSet<Vector3>();
        // BFS to see if any connect to air
        foreach (var droplet in potentialEmpty)
        {
            BfsDroplets(knownOpen, droplet, knownClosed);
        }

        return knownClosed;
    }

    private void BfsDroplets(ISet<Vector3> knownOpen, Vector3 rootDroplet, ISet<Vector3> knownClosed)
    {
        if (knownOpen.Contains(rootDroplet)) return;
        if (knownClosed.Contains(rootDroplet)) return;

        var toExplore = new Queue<Vector3>();
        toExplore.Enqueue(rootDroplet);
        var visitedDroplets = new HashSet<Vector3>();

        while (toExplore.Count > 0)
        {
            var cur = toExplore.Dequeue();
            if (visitedDroplets.Contains(cur)) continue;
            visitedDroplets.Add(cur);
            
            if (knownOpen.Contains(cur))
            {
                foreach (var visited in visitedDroplets)
                {
                    knownOpen.Add(visited);
                }

                return;
            }

            var neighboursToVisit =
                cur.GetNeighbours().Where(n => !visitedDroplets.Contains(n) && !Droplets.Contains(n));
            foreach (var unvisitedNeighbour in neighboursToVisit)
            {
                toExplore.Enqueue(unvisitedNeighbour);
            }
        }

        foreach (var visited in visitedDroplets)
        {
            knownClosed.Add(visited);
        }
    }

    public void Print()
    {
        foreach (var line in GetLines())
        {
            Console.WriteLine(line);
        }
    }

    private IEnumerable<string> GetLines()
    {
        var (min, max) = Limits.GetGlobalLimits();
        for (var z = max.Z; z >= min.Z; z--)
        {
            for (var y = max.Y; y >= min.Y; y--)
            {
                var line = $"Y = {y}\t";
                for (var x = min.X; x <= max.X; x++)
                {
                    line += Droplets.Contains(new Vector3(x, y, z)) ? "#" : ".";
                }

                yield return line;
            }

            yield return $"Z = {z}";
        }
    }
}

public class CloudLimits
{
    private Dictionary<Vector2, Vector2> LimitsByXY { get; } = new();
    private Dictionary<Vector2, Vector2> LimitsByXZ { get; } = new();
    private Dictionary<Vector2, Vector2> LimitsByZY { get; } = new();

    public (Vector3 Min, Vector3 Max) GetGlobalLimits()
    {
        var minX = LimitsByZY.Values.Select(l => l.First).Min();
        var maxX = LimitsByZY.Values.Select(l => l.Second).Max();
        var minY = LimitsByXZ.Values.Select(l => l.First).Min();
        var maxY = LimitsByXZ.Values.Select(l => l.Second).Max();
        var minZ = LimitsByXY.Values.Select(l => l.First).Min();
        var maxZ = LimitsByXY.Values.Select(l => l.Second).Max();

        return (new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
    }

    public bool WithinLimits(Vector3 position)
    {
        if (!WithinLimits(LimitsByXY, position.XY, position.Z))
            return false;

        if (!WithinLimits(LimitsByXZ, position.XZ, position.Y))
            return false;

        if (!WithinLimits(LimitsByZY, position.ZY, position.X))
            return false;

        return true;
    }

    private static bool WithinLimits(IReadOnlyDictionary<Vector2, Vector2> planeLimits, Vector2 positionOnPlane, int height)
    {
        if (!planeLimits.ContainsKey(positionOnPlane))
            return false;
        
        var limits = planeLimits[positionOnPlane];
        return height >= limits.First && height <= limits.Second;
    }

    public void UpdateLimitsOnXY(Vector2 positionOnPlane, int height) =>
        UpdateLimitsOn(LimitsByXY, positionOnPlane, height);
    public void UpdateLimitsOnXZ(Vector2 positionOnPlane, int height) =>
        UpdateLimitsOn(LimitsByXZ, positionOnPlane, height);
    public void UpdateLimitsOnZY(Vector2 positionOnPlane, int height) =>
        UpdateLimitsOn(LimitsByZY, positionOnPlane, height);
    
    private static void UpdateLimitsOn(Dictionary<Vector2, Vector2> planeLimits, Vector2 positionOnPlane, int height)
    {
        if (!planeLimits.ContainsKey(positionOnPlane))
            planeLimits[positionOnPlane] = new Vector2(height, height);
        else
        {
            if (planeLimits[positionOnPlane].First > height)
                planeLimits[positionOnPlane] = planeLimits[positionOnPlane] with { First = height };
            if (planeLimits[positionOnPlane].Second < height)
                planeLimits[positionOnPlane] = planeLimits[positionOnPlane] with { Second = height };
        }
    }
}

public record struct Vector2(int First, int Second);

public record struct Vector3(int X, int Y, int Z)
{
    public Vector3 XUp => this with { X = X + 1 };
    public Vector3 XDown => this with { X = X - 1 };
    public Vector3 YUp => this with { Y = Y + 1 };
    public Vector3 YDown => this with { Y = Y - 1 };
    public Vector3 ZUp => this with { Z = Z + 1 };
    public Vector3 ZDown => this with { Z = Z - 1 };

    public Vector2 XY => new(X, Y);
    public Vector2 XZ => new(X, Z);
    public Vector2 ZY => new(Z, Y);
    
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