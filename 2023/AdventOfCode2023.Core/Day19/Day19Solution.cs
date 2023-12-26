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
        yield return 0.ToString();
    }
}

public record WorkflowSystem(List<Part> Parts, Dictionary<string, Workflow> Workflows)
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
        var workflows = new List<Workflow>();
        while (!string.IsNullOrWhiteSpace(lines[i]))
        {
            workflows.Add(Workflow.FromInput(lines[i]));
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

public record Workflow(string Name, Func<Part, string?>[] Rules)
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
    
    public static Workflow FromInput(string line)
    {
        var name = line.Split('{')[0];
        var rules = line.Split('{')[1].Split('}')[0].Split(',').Select(ParseRule);
        return new Workflow(name, rules.ToArray());
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