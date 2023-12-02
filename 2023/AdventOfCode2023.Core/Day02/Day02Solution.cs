namespace AdventOfCode2023.Core.Day02;

public record Day02Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var games = GameSet.FromInputLines(Input);
        var valid = new Showing(12, 13, 14);
        var validCount = games.Games.Where(game => game.IsValid(valid)).Select(game => game.Id).Sum();
        yield return validCount.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var games = GameSet.FromInputLines(Input);
        var powerSum = games.Games.Select(g => g.Power()).Sum();
        yield return powerSum.ToString();
    }
}

public record Showing(int Red, int Green, int Blue);

public record Game(int Id, IList<Showing> Showings)
{
    public bool IsValid(Showing max) => Showings.All(showing =>
        showing.Red <= max.Red && showing.Green <= max.Green && showing.Blue <= max.Blue);

    public int Power()
    {
        var minRed = 0;
        var minGreen = 0;
        var minBlue = 0;

        foreach (var showing in Showings)
        {
            minRed = Math.Max(minRed, showing.Red);
            minGreen = Math.Max(minGreen, showing.Green);
            minBlue = Math.Max(minBlue, showing.Blue);
        }

        return minRed * minGreen * minBlue;
    } 
    
    public static Game FromInput(string input)
    {
        var id = int.Parse(input.Split(":")[0].Split(" ")[1]);
        var sets = input.Split(":")[1].Split(";");
        var showings = new List<Showing>();
        foreach (var set in sets)
        {
            var red = 0;
            var green = 0;
            var blue = 0;
            foreach (var colorString in set.Split(",").Select(s => s.Trim()))
            {
                var count = int.Parse(colorString.Split(" ")[0]);
                var color = colorString.Split(" ")[1].ToLowerInvariant();
                switch (color)
                {
                    case "red":
                        red = count;
                        break;
                    case "green": 
                        green = count;
                        break;
                    case "blue":
                        blue = count;
                        break;
                }
            }
            showings.Add(new Showing(red, green, blue));
        }

        return new Game(id, showings);
    }
}

public record GameSet(IList<Game> Games)
{
    public static GameSet FromInputLines(IEnumerable<string> lines)
    {
        return new GameSet(lines.Select(Game.FromInput).ToList());
    }
}