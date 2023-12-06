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
        yield return 0.ToString();
    }
}

public record RaceSet(IReadOnlyList<RaceInformation> Races)
{
    public int HoldTimesToWinProduct()
    {
        return Races.Select(r => r.HoldTimeOptionsToWin()).Aggregate(1, (prev, curr) => prev * curr);
    }
    
    public static RaceSet FromInput(IList<string> input)
    {
        var times = input[0].Ints().ToArray();
        var distances = input[1].Ints().ToArray();

        var races = new List<RaceInformation>();
        
        for (var i = 0; i < times.Length; i++)
        {
            races.Add(new RaceInformation(times[i], distances[i]));
        }

        return new RaceSet(races);
    }
}

public record RaceInformation(int Time, int Distance)
{
    public int HoldTimeOptionsToWin()
    {
        return MaximumHoldTimeToBeat() - MinimumHoldTimeToBeat() + 1;
    }

    private int MinimumHoldTimeToBeat()
    {
        for (var holdTime = 0; holdTime < Time; holdTime++)
        {
            var dist = holdTime * (Time - holdTime);
            if (dist > Distance) return holdTime;
        }

        return -1;
    }

    private int MaximumHoldTimeToBeat()
    {
        for (var holdTime = Time; holdTime > 0; holdTime--)
        {
            var dist = holdTime * (Time - holdTime);
            if (dist > Distance) return holdTime;
        }

        return -1;
    }
}