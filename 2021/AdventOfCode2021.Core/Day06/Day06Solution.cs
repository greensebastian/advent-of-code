namespace AdventOfCode2021.Core.Day06;

public record Day06Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var fishCountByDay = CountFish(80);

        yield return fishCountByDay.Sum().ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        var fishCountByDay = CountFish(256);

        yield return fishCountByDay.Sum().ToString();
    }

    private long[] CountFish(int daysToSimulate)
    {
        var fishCountByDay = new long[10];
        foreach (var line in Input)
        {
            foreach (var days in line.Split(",").Select(byte.Parse))
            {
                fishCountByDay[days]++;
            }

            for (var day = 0; day < daysToSimulate; day++)
            {
                var fishToMultiply = fishCountByDay[0];
                for (var daysLeft = 1; daysLeft < fishCountByDay.Length - 1; daysLeft++)
                {
                    fishCountByDay[daysLeft - 1] = fishCountByDay[daysLeft];
                }

                fishCountByDay[6] += fishToMultiply;
                fishCountByDay[8] = fishToMultiply;

                Console.WriteLine($"Count day {day}:\t{fishCountByDay.Sum()}");
            }
        }

        return fishCountByDay;
    }
}