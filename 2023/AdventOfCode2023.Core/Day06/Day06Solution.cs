namespace AdventOfCode2023.Core.Day06;

public record Day06Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var races = RaceSet.FromInput(Input.ToArray());
        yield return races.HoldTimesToWinProduct().ToString();
    }

    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var races = RaceSet.FromSecondInput(Input.ToArray());
        yield return races.HoldTimesToWinProduct().ToString();
    }
}

public record RaceSet(IReadOnlyList<RaceInformation> Races)
{
    public long HoldTimesToWinProduct()
    {
        return Races.Select(r => r.HoldTimeOptionsToWin()).Aggregate(1L, (prev, curr) => prev * curr);
    }

    public static RaceSet FromSecondInput(IList<string> input)
    {
        var newInput = input.Select(l => string.Join("", l.Longs())).ToList();
        return FromInput(newInput);
    }
    
    public static RaceSet FromInput(IList<string> input)
    {
        var times = input[0].Longs().ToArray();
        var distances = input[1].Longs().ToArray();

        var races = new List<RaceInformation>();
        
        for (var i = 0; i < times.Length; i++)
        {
            races.Add(new RaceInformation(times[i], distances[i]));
        }

        return new RaceSet(races);
    }
}

public record RaceInformation(long Time, long Distance)
{
    public long HoldTimeOptionsToWin()
    {
        return MaximumHoldTimeToBeat() - MinimumHoldTimeToBeat() + 1;
    }

    private int MinimumHoldTimeToBeat()
    {
        for (var holdTime = 0; holdTime < Time; holdTime++)
        {
            long dist = holdTime * (Time - holdTime);
            if (dist > Distance) return holdTime;
        }

        return -1;
    }

    private long MaximumHoldTimeToBeat()
    {
        for (var holdTime = Time; holdTime > 0; holdTime--)
        {
            long dist = holdTime * (Time - holdTime);
            if (dist > Distance) return holdTime;
        }

        return -1;
    }
}