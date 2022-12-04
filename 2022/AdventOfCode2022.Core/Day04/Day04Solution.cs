using System.Globalization;

namespace AdventOfCode2022.Core.Day04;

public record Day04Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var count = 0;
        foreach (var line in Input)
        {
            var pair = new SectionPair(line.Trim());
            if (pair.Intersect is not null) count++;
        }

        yield return count.ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        var count = 0;
        foreach (var line in Input)
        {
            var pair = new SectionPair(line.Trim());
            if (pair.PartiallyIntersect) count++;
        }

        yield return count.ToString();
    }
}

class SectionPair
{
    private Section Left { get; }
    private Section Right { get; }
    
    public SectionPair(string input)
    {
        Left = new Section(input.Split(",")[0]);
        Right = new Section(input.Split(",")[1]);
    }

    public Section? Intersect => Left.FullyIntersects(Right);

    public bool PartiallyIntersect => Left.PartiallyIntersects(Right);
}

class Section
{
    private int Start { get; }
    private int End { get; }
    
    public Section(string input)
    {
        Start = int.Parse(input.Split("-")[0]);
        End = int.Parse(input.Split("-")[1]);
    }

    public Section? FullyIntersects(Section other)
    {
        if (Start == other.Start && End == other.End)
            return this;
        
        var leftMost = GetStartsLowest(other);
        var rightMost = this == leftMost ? other : this;
        
        return leftMost.End >= rightMost.End ? leftMost : null;
    }

    public bool PartiallyIntersects(Section other)
    {
        if (Start == other.Start && End == other.End)
            return true;

        var leftMost = GetStartsLowest(other);
        var rightMost = this == leftMost ? other : this;

        return rightMost.Start <= leftMost.End;
    }

    private Section GetStartsLowest(Section other)
    {
        if (Start == other.Start && End == other.End)
            return this;

        if (Start == other.Start)
            return End < other.End ? other : this;

        return Start < other.Start ? this : other;
    }
}