using System.Globalization;

namespace AdventOfCode2022.Core.Day02;

public record Day02Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var totalScore = 0;
        foreach (var line in Input)
        {
            var opponentMove = line.Split(" ")[0];
            var recommendedMove = line.Split(" ")[1];

            var resultScore = (opponentMove, recommendedMove) switch
            {
                ("A", "X") => 3,
                ("A", "Y") => 6,
                ("A", "Z") => 0,
                ("B", "X") => 0,
                ("B", "Y") => 3,
                ("B", "Z") => 6,
                ("C", "X") => 6,
                ("C", "Y") => 0,
                ("C", "Z") => 3
            };

            var moveScore = recommendedMove switch
            {
                "X" => 1,
                "Y" => 2,
                "Z" => 3
            };

            totalScore += moveScore + resultScore;
        }

        yield return totalScore.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var totalScore = 0;
        foreach (var line in Input)
        {
            var opponentMove = line.Split(" ")[0];
            var wantedOutcome = line.Split(" ")[1];

            var recommendedMove = (opponentMove, wantedOutcome) switch
            {
                ("A", "X") => "C",
                ("A", "Y") => "A",
                ("A", "Z") => "B",
                ("B", "X") => "A",
                ("B", "Y") => "B",
                ("B", "Z") => "C",
                ("C", "X") => "B",
                ("C", "Y") => "C",
                ("C", "Z") => "A"
            };
            
            var resultScore = (opponentMove, recommendedMove) switch
            {
                ("A", "A") => 3,
                ("A", "B") => 6,
                ("A", "C") => 0,
                ("B", "A") => 0,
                ("B", "B") => 3,
                ("B", "C") => 6,
                ("C", "A") => 6,
                ("C", "B") => 0,
                ("C", "C") => 3
            };

            var moveScore = recommendedMove switch
            {
                "A" => 1,
                "B" => 2,
                "C" => 3
            };

            totalScore += moveScore + resultScore;
        }

        yield return totalScore.ToString();
    }
}