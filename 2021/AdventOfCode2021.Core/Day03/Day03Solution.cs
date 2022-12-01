namespace AdventOfCode2021.Core.Day03;

public record Day03Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var deltas = new int[Input.First().Length];
        foreach (var line in Input)
        {
            for (var i = 0; i < deltas.Length; i++)
            {
                deltas[i] += line[i] == '1' ? 1 : -1;
            }
        }

        var gamma = 0;
        var epsilon = 0;
        for (var i = 0; i < deltas.Length; i++)
        {
            var one = deltas[i] >= 1;
            var power = deltas.Length - 1 - i;
            gamma += one ? (int)Math.Pow(2, power) : 0;
            epsilon += one ? 0 : (int)Math.Pow(2, power);
        }

        yield return (gamma*epsilon).ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        yield return "0";
    }
}