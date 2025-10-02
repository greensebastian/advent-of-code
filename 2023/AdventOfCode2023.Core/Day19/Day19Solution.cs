namespace AdventOfCode2023.Core.Day19;

public record Day19Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var system = WorkflowSystem.FromInput(Input.ToArray());
        var res = system.GetSumOfAcceptedParts();
        yield return res.ToString();
    }

    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var wfs = new WorkflowSolver(Input.ToArray());
        yield return wfs.AcceptedCombinations().ToString();
    }
}

public record WorkflowSolver(string[] Input)
{
    public IReadOnlyDictionary<string, Workflow> Workflows { get; } = Input
        .TakeWhile(l => !string.IsNullOrWhiteSpace(l)).Select(l => new Workflow(l)).ToDictionary(wf => wf.Name);
    
    public long AcceptedCombinations()
    {
        var range = new RatingRange(new(1, 4000), new(1, 4000), new(1, 4000), new(1, 4000));
        var ratingResults = new List<RatingRangeResult>() { new RatingRangeResult(range, "in") };
        while (ratingResults.Any(r => r.Result != "A" && r.Result != "R"))
        {
            var toChange = ratingResults.First(r => r.Result != "A" && r.Result != "R");
            ratingResults.Remove(toChange);
            ratingResults.AddRange(Workflows[toChange.Result!].Apply(toChange.Range));
        }
        
        return ratingResults.Sum(r => r.Score());
    }
}

public record Workflow(string Definition)
{
    public string Name { get; } = Definition.Split('{')[0];
    
    private WorkflowRule[] Rules { get; } = Definition.Split("{")[1].Split("}")[0].Split(",").SkipLast(1).Select(r =>
        new WorkflowRule(r.Split('<', '>')[0], r.Contains("<"), int.Parse(r.Split('<', '>')[1].Split(':')[0]),
            r.Split(':')[1])).ToArray();

    private string Fallback { get; } = Definition.Split(',').Last().Split('}')[0];

    public IEnumerable<RatingRangeResult> Apply(RatingRange range)
    {
        var resolved = new Dictionary<RatingRange, string>();
        var unresolved = new [] { range };
        foreach (var rule in Rules)
        {
            var next = unresolved.SelectMany(r => rule.Apply(r)).ToList();
            var nextUnresolved = next.Where(r => r.Result is null).Select(r => r.Range).ToArray();
            foreach (var ratingRangeResult in next)
            {
                if (ratingRangeResult.Result is not null)
                {
                    resolved[ratingRangeResult.Range] = ratingRangeResult.Result;
                }
            }

            unresolved = nextUnresolved;
        }

        return resolved.Select(r => new RatingRangeResult(r.Key, r.Value))
            .Concat(unresolved.Select(ur => new RatingRangeResult(ur, Fallback)));
    }
}

public record WorkflowRule(string RatingKey, bool IsLessThan, int Rating, string ResultKey)
{
    private IEnumerable<(Range Range, string? ResultKey)> Apply(Range range)
    {
        if (IsLessThan)
        {
            if (range.High < Rating)
            {
                yield return (range, ResultKey);
            }
            else if (range.Low >= Rating)
            {
                yield return (range, null);
            }
            else
            {
                yield return (range with { High = Rating - 1 }, ResultKey);
                yield return (range with { Low = Rating }, null);
            }
        }
        else
        {
            if (range.High <= Rating)
            {
                yield return (range, null);
            }
            else if (range.Low > Rating)
            {
                yield return (range, ResultKey);
            }
            else
            {
                yield return (range with { High = Rating }, null);
                yield return (range with { Low = Rating + 1 }, ResultKey);
            }
        }
    }
    
    public IEnumerable<RatingRangeResult> Apply(RatingRange range)
    {
        switch (RatingKey)
        {
            case "x":
            {
                foreach (var result in Apply(range.X))
                {
                    yield return new RatingRangeResult(range with { X = result.Range }, result.ResultKey);
                }
                yield break;
            }
            case "m":
            {
                foreach (var result in Apply(range.M))
                {
                    yield return new RatingRangeResult(range with { M = result.Range }, result.ResultKey);
                }
                yield break;
            }
            case "a":
            {
                foreach (var result in Apply(range.A))
                {
                    yield return new RatingRangeResult(range with { A = result.Range }, result.ResultKey);
                }
                yield break;
            }
            case "s":
            {
                foreach (var result in Apply(range.S))
                {
                    yield return new RatingRangeResult(range with { S = result.Range }, result.ResultKey);
                }
                yield break;
            }
        }
    }
}

public record Range(int Low, int High)
{
    public long Size() => High - Low + 1;
}

public record RatingRange(Range X, Range M, Range A, Range S)
{
    public long Size() => X.Size() * M.Size() * A.Size() * S.Size();
}

public record RatingRangeResult(RatingRange Range, string? Result)
{
    public long Score() => Result == "A" ? Range.Size() : 0;
}

public record RatingSplit(
    IReadOnlyList<RatingRange> Accepted,
    IReadOnlyList<RatingRange> Rejected,
    IReadOnlyList<RatingRange> Unknown);

public record WorkflowSystem(List<Part> Parts, Dictionary<string, FuncWorkflow> Workflows)
{
    public int GetSumOfAcceptedParts()
    {
        var acceptedParts = new List<Part>();
        foreach (var part in Parts)
        {
            var res = (string?)null;
            var workflowName = "in";
            while (res != "A" && res != "R")
            {
                var workflow = Workflows[workflowName];
                res = workflow.Run(part);
                workflowName = res;
            }
            
            if (res == "A") acceptedParts.Add(part);
        }

        var sum = acceptedParts.Sum(p => p.Score);
        return sum;
    }
    
    public static WorkflowSystem FromInput(IList<string> lines)
    {
        var i = 0;
        var workflows = new List<FuncWorkflow>();
        while (!string.IsNullOrWhiteSpace(lines[i]))
        {
            workflows.Add(FuncWorkflow.FromInput(lines[i]));
            i++;
        }

        i++;
        var parts = new List<Part>();
        while (i < lines.Count)
        {
            parts.Add(Part.FromInput(lines[i]));
            i++;
        }

        return new WorkflowSystem(parts, workflows.ToDictionary(w => w.Name));
    }
}

public record Part(int X, int M, int A, int S)
{
    public int Score => X + M + A + S;
    
    public static Part FromInput(string line)
    {
        var ratings = line
            .Replace("{", "")
            .Replace("}", "")
            .Split(',', StringSplitOptions.TrimEntries)
            .Select(p => new { Ident = p.Split('=')[0], Value = int.Parse(p.Split('=')[1])})
            .ToDictionary(p => p.Ident, p => p.Value);
        return new Part(ratings["x"], ratings["m"], ratings["a"], ratings["s"]);
    }
}

public record FuncWorkflow(string Name, Func<Part, string?>[] Rules)
{
    public string Run(Part part)
    {
        foreach (var rule in Rules)
        {
            var res = rule(part);
            if (res is not null) return res;
        }

        throw new Exception("Impossible");
    }
    
    public static FuncWorkflow FromInput(string line)
    {
        var name = line.Split('{')[0];
        var rules = line.Split('{')[1].Split('}')[0].Split(',').Select(ParseRule);
        return new FuncWorkflow(name, rules.ToArray());
    }

    private static Func<Part, string?> ParseRule(string rule)
    {
        if (rule.IndexOf(':') < 0) return _ => rule;
        var ratingName = rule[0];
        var comparator = rule[1];
        var limit = int.Parse(rule[2..rule.IndexOf(':')]);
        var outcome = rule[(rule.IndexOf(':') + 1)..];

        return (ratingName, comparator) switch
        {
            ('x', '>') => part => part.X > limit ? outcome : null,
            ('x', '<') => part => part.X < limit ? outcome : null,
            ('m', '>') => part => part.M > limit ? outcome : null,
            ('m', '<') => part => part.M < limit ? outcome : null,
            ('a', '>') => part => part.A > limit ? outcome : null,
            ('a', '<') => part => part.A < limit ? outcome : null,
            ('s', '>') => part => part.S > limit ? outcome : null,
            ('s', '<') => part => part.S < limit ? outcome : null,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}