namespace AdventOfCode2022.Core.Day15;

public record Day15Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var map = new SensorMap(Input);

        yield return map.CoveredOnRow(10).ToString();
        yield return map.CoveredOnRow(2000000).ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        var map = new SensorMap(Input);
        
        yield return "0";
    }
}

public class SensorMap
{
    private List<Sensor> Sensors { get; } = new();
    private HashSet<Vector> Beacons { get; } = new();
    private long MinX { get; }
    private long MaxX { get; }
    private long MinY { get; }
    private long MaxY { get; }
    public SensorMap(IEnumerable<string> input)
    {
        foreach (var line in input)
        {
            var sensor = Sensor.From(line);
            Sensors.Add(sensor);
            Beacons.Add(sensor.BeaconPosition);
            
            var distance = sensor.Position.StepsTo(sensor.BeaconPosition);
            
            var minX = Math.Min(sensor.Position.X - distance, sensor.BeaconPosition.X);
            var maxX = Math.Max(sensor.Position.X + distance, sensor.BeaconPosition.X);
            var minY = Math.Min(sensor.Position.Y - distance, sensor.BeaconPosition.Y);
            var maxY = Math.Max(sensor.Position.Y + distance, sensor.BeaconPosition.Y);

            MinX = minX < MinX ? minX : MinX;
            MaxX = maxX > MaxX ? maxX : MaxX;
            MinY = minY < MinY ? minY : MinY;
            MaxY = maxY > MaxY ? maxY : MaxY;
        }
    }

    public int CoveredOnRow(int row)
    {
        var count = 0;
        var coveredOnRow = Sensors.SelectMany(sensor => sensor.CoveredOnRow(row)).ToHashSet();
        foreach (var vector in coveredOnRow)
        {
            if (!Beacons.Contains(vector))
                count++;
        }

        return count;
    }

    public Vector? MissingOnRow(int row)
    {
        return null; // TODO
    }

    public void Print()
    {
        foreach (var line in GetPrintLines())
        {
            Console.WriteLine(line);
        }
    }

    private IEnumerable<string> GetPrintLines()
    {
        var headerLine = $"\t";
        for (var x = MinX; x <= MaxX; x++)
        {
            headerLine += (Math.Abs(x) % 10).ToString();
        }

        yield return headerLine;
        for (var y = MinY; y <= MaxY; y++)
        {
            var line = $"{y}:\t";
            for (var x = MinX; x <= MaxX; x++)
            {
                var v = new Vector(x, y);
                if (Sensors.Any(s => s.Position == v)) line += "S";
                else if (Beacons.Contains(v)) line += "B";
                else if (Sensors.Any(s => s.DistanceToBeacon < s.Position.StepsTo(v))) line += "#";
                else line += " ";
            }

            yield return line;
        }
    }
}

public record Sensor(Vector Position, Vector BeaconPosition)
{
    public long DistanceToBeacon { get; } = Position.StepsTo(BeaconPosition);

    public IEnumerable<Vector> CoveredOnRow(int row)
    {
        for (var x = Position.X - DistanceToBeacon; x <= Position.X + DistanceToBeacon; x++)
        {
            var v = new Vector(x, row);
            if (Position.StepsTo(v) <= DistanceToBeacon)
                yield return v;
        }
    }

    public Vector? MinOnRow(int row)
    {
        for (var x = Position.X - DistanceToBeacon; x <= Position.X + DistanceToBeacon; x++)
        {
            var v = new Vector(x, row);
            if (Position.StepsTo(v) <= DistanceToBeacon)
                return v;
        }

        return null;
    }
    
    public Vector? MaxOnRow(int row)
    {
        for (var x = Position.X + DistanceToBeacon; x >= Position.X - DistanceToBeacon; x--)
        {
            var v = new Vector(x, row);
            if (Position.StepsTo(v) <= DistanceToBeacon)
                return v;
        }

        return null;
    }

    public static Sensor From(string input)
    {
        var ints = input.Ints().ToArray();
        return new Sensor(new Vector(ints[0], ints[1]), new Vector(ints[2], ints[3]));
    }
}

public record Vector(long X, long Y)
{
    public long StepsTo(Vector other)
    {
        var x = Math.Abs(other.X - X);
        var y = Math.Abs(other.Y - Y);
        return x + y;
    }
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