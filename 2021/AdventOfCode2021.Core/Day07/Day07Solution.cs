namespace AdventOfCode2021.Core.Day07;

public record Day07Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var crabPositions = Input.Single().Split(",").Select(int.Parse).Order().ToList();
        var max = crabPositions.Max();
        var min = crabPositions.Min();
        var minMove = int.MaxValue;
        for (var pos = min; pos <= max; pos++)
        {
            var toMove = crabPositions.Select(cPos => Math.Abs(cPos - pos)).Sum();
            minMove = toMove < minMove ? toMove : minMove;
        }
        yield return minMove.ToString();
    }

    public override IEnumerable<string> SecondSolution()
    {
        var crabPositions = Input.Single().Split(",").Select(int.Parse).Order().ToList();
        var max = crabPositions.Max();
        var min = crabPositions.Min();
        var minMove = int.MaxValue;
        for (var pos = min; pos <= max; pos++)
        {
            var toMove = crabPositions.Select(cPos => GetMoveCost(cPos, pos)).Sum();
            minMove = toMove < minMove ? toMove : minMove;
        }
        yield return minMove.ToString();
    }

    private static int GetMoveCost(int crabPos, int targetPos)
    {
        var cost = 0;
        for (var step = 1; step <= Math.Abs(targetPos - crabPos); step++)
        {
            cost += step;
        }

        return cost;
    }
}