using System.Globalization;

namespace AdventOfCode2021.Core.Day02;

public record Day02Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        long depth = 0;
        long forward = 0;
        foreach (var line in Input)
        {
            var action = line.Split(" ")[0];
            var amount = long.Parse(line.Split(" ")[1], CultureInfo.InvariantCulture);

            switch (action)
            {
                case "forward": 
                    forward += amount;
                    break;
                case "down":
                    depth += amount;
                    break;
                case "up":
                    depth -= amount;
                    break;
            }
        }

        yield return (depth * forward).ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        long depth = 0;
        long forward = 0;
        long aim = 0;
        foreach (var line in Input)
        {
            var action = line.Split(" ")[0];
            var amount = long.Parse(line.Split(" ")[1], CultureInfo.InvariantCulture);

            switch (action)
            {
                case "forward": 
                    forward += amount;
                    depth += aim * amount;
                    break;
                case "down":
                    aim += amount;
                    break;
                case "up":
                    aim -= amount;
                    break;
            }
        }

        yield return (depth * forward).ToString();
    }
}