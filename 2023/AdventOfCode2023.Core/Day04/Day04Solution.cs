namespace AdventOfCode2023.Core.Day04;

public record Day04Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var score = Input.Select(l => ScratchCard.FromInput(l).Score()).Sum();
        yield return score.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var cards = RecursiveScratchCardSet.FromInput(Input);
        var result = cards.TotalScratchCardCount();
        yield return result.ToString();
    }
}

public record RecursiveScratchCardSet(IList<ScratchCard> OriginalCards)
{
    public int TotalScratchCardCount()
    {
        var counts = new int[OriginalCards.Count].Select(_ => 1).ToArray();

        for (var i = 0; i < counts.Length; i++)
        {
            var winCount = OriginalCards[i].Wins.Count();
            for (var j = 1; j <= winCount; j++)
            {
                var k = i + j;
                if (k >= counts.Length) break;

                counts[k] += counts[i];
            }
        }

        return counts.Sum();
    }
    
    public static RecursiveScratchCardSet FromInput(IEnumerable<string> lines)
    {
        return new RecursiveScratchCardSet(lines.Select(ScratchCard.FromInput).ToArray());
    }
}

public record ScratchCard(int Id, IList<int> Numbers, ISet<int> WinningNumbers)
{
    public IEnumerable<int> Wins => Numbers.Where(n => WinningNumbers.Contains(n));
    
    public int Score()
    {
        var wins = Wins.Count();
        var score = wins > 0 ? Math.Pow(2, wins - 1) : 0;
        return Convert.ToInt32(score);
    }
    
    public static ScratchCard FromInput(string line)
    {
        var id = int.Parse(line[4..].Split(":")[0]);
        var winning = line.Split(":")[1].Split("|")[0].Split(" ").Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToHashSet();
        var mine = line.Split(":")[1].Split("|")[1].Split(" ").Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToArray();
        return new ScratchCard(id, mine, winning);
    }
}