namespace AdventOfCode2022.Core;

public abstract record BaseSolution(IEnumerable<string> Input)
{
    public abstract IEnumerable<string> FirstSolution(params string[] args);
    public abstract IEnumerable<string> SecondSolution(params string[] args);
}