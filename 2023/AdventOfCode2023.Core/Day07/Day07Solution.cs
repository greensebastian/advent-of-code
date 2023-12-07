namespace AdventOfCode2023.Core.Day07;

public record Day07Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var hands = SetOfHands.FromInput(Input);
        var score = hands.Score();
        yield return score.ToString();
    }

    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var hands = SetOfHands.FromInput(Input);
        var score = hands.Score(true);
        yield return score.ToString();
    }
}

public record SetOfHands(IReadOnlyList<Hand> Hands)
{
    public long Score(bool jokers = false)
    {
        var ordered = (jokers ? JokerOrdered() : Ordered()).Reverse().ToArray();
        long sum = 0;
        for (var i = 0; i < ordered.Length; i++)
        {
            sum += (i + 1) * ordered[i].Bid;
        }

        return sum;
    }
    
    private IReadOnlyList<Hand> Ordered()
    {
        var orderedHands = Hands.ToList();
        orderedHands.Sort((a, b) => a.CompareTo(b));
        return orderedHands.AsReadOnly();
    }
    
    private IReadOnlyList<Hand> JokerOrdered()
    {
        var orderedHands = Hands.ToList();
        orderedHands.Sort((a, b) => a.JokerCompareTo(b));
        return orderedHands.AsReadOnly();
    }
    
    public static SetOfHands FromInput(IEnumerable<string> input)
    {
        var hands = input.Select(Hand.FromInput).ToArray();
        return new SetOfHands(hands);
    }
}

public record Hand(string Cards, int Bid)
{
    public int JokerCompareTo(Hand other)
    {
        if (JokerTypeRank < other.JokerTypeRank) return -1;
        if (JokerTypeRank > other.JokerTypeRank) return 1;
        for (var i = 0; i < Cards.Length; i++)
        {
            var diff = JokerCardValue[Cards[i]] - JokerCardValue[other.Cards[i]];
            if (diff != 0) return diff;
        }

        return 0;
    }
    
    public int CompareTo(Hand other)
    {
        if (TypeRank < other.TypeRank) return -1;
        if (TypeRank > other.TypeRank) return 1;
        for (var i = 0; i < Cards.Length; i++)
        {
            var diff = CardValue[Cards[i]] - CardValue[other.Cards[i]];
            if (diff != 0) return diff;
        }

        return 0;
    }

    private static IReadOnlyDictionary<char, int> CardValue { get; } = new Dictionary<char, int>
    {
        { 'A', 0 },
        { 'K', 1 },
        { 'Q', 2 },
        { 'J', 3 },
        { 'T', 4 },
        { '9', 5 },
        { '8', 6 },
        { '7', 7 },
        { '6', 8 },
        { '5', 9 },
        { '4', 10 },
        { '3', 11 },
        { '2', 12 }
    };

    private static IReadOnlyDictionary<char, int> JokerCardValue { get; } = GetJokerCardValue();

    private static IReadOnlyDictionary<char, int> GetJokerCardValue()
    {
        var jokerCardValue = CardValue.ToDictionary();
        jokerCardValue['J'] = jokerCardValue.Values.Max() + 1;
        return jokerCardValue;
    }

    private int TypeRank { get; } = ComputeTypeRank(Cards);
    
    private int JokerTypeRank { get; } = ComputeTypeRankWithJokers(Cards);

    private static int ComputeTypeRank(string cards)
    {
        if (cards.All(c => c == cards[0])) return 0;
        
        for (var i = 0; i < 2; i++)
        {
            var toCheck = cards[i];
            if (cards.Count(c => c == toCheck) == 4) return 1;
        }

        var distinctCount = cards.Distinct().Count();

        if (distinctCount == 2) return 2;
        
        for (var i = 0; i < 3; i++)
        {
            var toCheck = cards[i];
            if (cards.Count(c => c == toCheck) == 3) return 3;
        }
        
        if (distinctCount == 3) return 4;
        
        if (distinctCount == 4) return 5;

        return 6;
    }
    
    private static int ComputeTypeRankWithJokers(string cardsWithJokers)
    {
        var jokers = cardsWithJokers.Count(c => c == 'J');
        if (jokers == 0) return ComputeTypeRank(cardsWithJokers);
        if (jokers == cardsWithJokers.Length) return 0;
        
        var cards = cardsWithJokers.Replace("J", "");

        var byCount = cards
            .Distinct()
            .OrderByDescending(c => cards.Count(oc => oc == c))
            .ToArray();

        return ComputeTypeRank(cards + new string(byCount[0], jokers));
    }
    
    public static Hand FromInput(string input)
    {
        return new Hand(input.Split(" ")[0], int.Parse(input.Split(" ")[1]));
    }
}