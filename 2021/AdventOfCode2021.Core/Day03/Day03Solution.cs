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
        var inDecimal = Input.Select(line => Convert.ToUInt32(line, 2)).ToList();
        
        var butSums = new int[Input.First().Length];

        // Oxygen generator
        uint oxygenGeneratorRating = 0;
        var active = new List<uint>(inDecimal);
        for (var i = 0; i < butSums.Length; i++)
        {
            RecomputeCounts(butSums, active);
            
            var power = butSums.Length - 1 - i;
            var dec = 1 << power;
            if (butSums[i] >= 0)
            {
                // Most common is one
                if (active.Count > 1)
                    active.RemoveAll(val => (val & dec) == 0);
            }
            else
            {
                // Most common is zero
                if (active.Count > 1)
                    active.RemoveAll(val => (val & dec) == dec);
            }
            
            if (active.Count < 2)
            {
                oxygenGeneratorRating = active.Single();
                break;
            }
        }
        
        // CO2 Scrubber
        uint co2Scrubber = 0;
        active = new List<uint>(inDecimal);
        for (var i = 0; i < butSums.Length; i++)
        {
            RecomputeCounts(butSums, active);
            
            var power = butSums.Length - 1 - i;
            var dec = 1 << power;
            if (butSums[i] < 0)
            {
                // Most common is zero
                if (active.Count > 1)
                    active.RemoveAll(val => (val & dec) == 0);
            }
            else
            {
                // Most common is one
                if (active.Count > 1)
                    active.RemoveAll(val => (val & dec) == dec);
            }
            
            if (active.Count < 2)
            {
                co2Scrubber = active.Single();
                break;
            }
        }

        yield return (oxygenGeneratorRating * co2Scrubber).ToString();
    }

    private static void RecomputeCounts(int[] deltas, List<uint> inDecimal)
    {
        for (var i = 0; i < deltas.Length; i++)
        {
            deltas[i] = 0;
        }

        foreach (var nbr in inDecimal)
        {
            for (var i = 0; i < deltas.Length; i++)
            {
                var shift = deltas.Length - 1 - i;
                deltas[i] += (nbr & (1u << shift)) == 0 ? -1 : 1;
            }
        }
    }
}