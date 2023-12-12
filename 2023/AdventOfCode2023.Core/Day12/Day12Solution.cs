using System.Numerics;

namespace AdventOfCode2023.Core.Day12;

public record Day12Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var sum = 0;
        foreach (var line in Input)
        {
            var range = SpringRow.FromInput(line);
            var count = range.ValidArrangementCountBinary();
            sum += count;
        }
        yield return sum.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var sum = 0;
        foreach (var line in Input)
        {
            var repeatInput = $"{line.Split(" ")[0].Repeat(5, "?")} {line.Split(" ")[1].Repeat(5, ",")}";
            var range = SpringRow.FromInput(repeatInput);
            var count = range.ValidArrangementCountDfs();
            sum += count;
        }
        yield return sum.ToString();
    }
}

public record SpringRow(string Springs, IList<int> BrokenRanges)
{
    private int BrokenCount { get; } = BrokenRanges.Sum();
    
    public static SpringRow FromInput(string line)
    {
        var brokenRanges = line.Split(' ')[1].Ints().ToArray();
        var springs = line.Split(' ')[0];
        return new SpringRow(springs, brokenRanges);
    }

    public int ValidArrangementCountBinary()
    {
        var springs = GetBigInteger(Springs, '#');

        var wildcards = GetBigInteger(Springs, '?');
        
        var valid = 0;
        var unexplored = new Stack<State>();
        unexplored.Push(new State(springs, wildcards));
        while (unexplored.Count > 0)
        {
            var curr = unexplored.Pop();
            if (curr.Wildcards == 0)
            {
                if (IsValid(curr)) valid++;
                continue;
            }
            
            if (curr.WildcardCount() == 0 || !CouldBeValid(curr)) continue;

            foreach (var next in curr.Next())
            {
                unexplored.Push(next);
            }
        }

        return valid;
    }

    private BigInteger GetBigInteger(string input, char flag)
    {
        var binString = string.Join("", input.Select(c => c == flag ? '1' : '0'));
        var result = BigInteger.Zero;
        foreach (var c in binString)
        {
            result <<= 1;
            if (c == '1') result |= 1;
        }

        return result;
    }

    public int ValidArrangementCountDfs()
    {
        if (IsValid(Springs))
        {
            return 1;
        }
        var valid = 0;
        var unexplored = new Stack<string>();
        unexplored.Push(Springs);
        while (unexplored.Count > 0)
        {
            var curr = unexplored.Pop();
            var iOfNextOption = curr.IndexOf('?');
            if (iOfNextOption == -1)
            {
                if (IsValid(curr)) valid++;
                continue;
            }
            
            if (iOfNextOption != 0 && !CouldBeValid(curr)) continue;
            
            unexplored.Push(curr.ReplaceAt(iOfNextOption, 1, "."));
            unexplored.Push(curr.ReplaceAt(iOfNextOption, 1, "#"));
        }

        return valid;
    }
    
    public int ValidArrangementCount()
    {
        return Permutations().Count();
    }

    private IEnumerable<string> Permutations()
    {
        var options = new[] { "" };
        for (var i = 0; i < Springs.Length; i++)
        {
            var newOptions = new List<string>();
            foreach (var option in options)
            {
                var c = Springs[i];
                switch (c)
                {
                    case '?':
                        newOptions.Add(option + '#');
                        newOptions.Add(option + '.');
                        break;
                    default:
                        newOptions.Add(option + c);
                        break;
                }
            }

            options = newOptions.ToArray();
        }

        var valid = new List<string>();
        foreach (var option in options)
        {
            if (IsValid(option)) valid.Add(option);
        }

        return valid;
    }

    private bool CouldBeValid(string option)
    {
        var brokenCount = option.Count(c => c == '#');
        if (brokenCount > BrokenCount) return false;
        
        var wildcardCount = option.Count(c => c == '?');
        if (brokenCount + wildcardCount < BrokenCount) return false;

        var optionBeforeQuestion = option;
        var questionI = option.IndexOf('?') - 1;
        if (questionI >= 0) optionBeforeQuestion = option[..questionI];

        var brokenI = 0;
        for (var i = 0; i < optionBeforeQuestion.Length; i++)
        {
            if (optionBeforeQuestion[i] == '.') continue;
            var toFind = BrokenRanges[brokenI];
            brokenI++;
            for (var j = 0; j < toFind; j++)
            {
                if (i >= optionBeforeQuestion.Length) break;
                if (optionBeforeQuestion[i] != '#') return false;
                i++;
            }
            if (i < optionBeforeQuestion.Length && optionBeforeQuestion[i] != '.') return false;
        }
        
        return true;
    }

    private bool IsValid(string option)
    {
        var ranges = option.Split('.', StringSplitOptions.RemoveEmptyEntries).Select(r => r.Length).ToArray();
        if (ranges.Length != BrokenRanges.Count) return false;

        var optionValid = Enumerable.Range(0, BrokenRanges.Count).All(i => BrokenRanges[i] == ranges[i]);
        return optionValid;
    }

    private bool IsValid(State state)
    {
        var brokenSections = state.BrokenSections().ToArray();
        if (brokenSections.Length != BrokenRanges.Count) return false;
        for (var i = 0; i < brokenSections.Length; i++)
        {
            if (brokenSections[i] != BrokenRanges[^(i+1)]) return false;
        }

        return true;
    }

    private bool CouldBeValid(State state)
    {
        if (state.Wildcards > state.Broken) return true;
        
        var brokenCount = state.BrokenCount();
        if (brokenCount > BrokenCount) return false;
        
        var wildcardCount = state.WildcardCount();
        if (brokenCount + wildcardCount < BrokenCount) return false;

        var brokenSections = state.BrokenSections(state.Wildcards).Reverse().ToArray();
        var toCheck = Math.Min(brokenSections.Length, BrokenRanges.Count);
        for (var i = 0; i < toCheck; i++)
        {
            if (i == toCheck - 1)
            {
                if (brokenSections[i] > BrokenRanges[i]) return false;
            }
            else
            {
                if (brokenSections[i] != BrokenRanges[i]) return false;
            }
        }
        
        return true;
    }
}

public readonly record struct State(BigInteger Broken, BigInteger Wildcards)
{
    public override string ToString() => $"{Broken.ToB128(true)}, {Wildcards.ToB128(true)}";

    public IEnumerable<State> Next()
    {
        var c = Wildcards;
        var n = BigInteger.One;
        while (c > 0)
        {
            n <<= 1;
            c >>= 1;
        }

        n >>= 1;
        yield return new State(Broken | n, Wildcards & ~n);
        yield return this with { Wildcards = Wildcards & ~n };
    }
    
    public IEnumerable<int> BrokenSections(BigInteger? start = null)
    {
        var c = Broken;
        var toShift = start ?? BigInteger.Zero;
        while (toShift > 0)
        {
            toShift >>= 1;
            c >>= 1;
        }
        while (c > 0)
        {
            while (c > 0 && (c & 1) == 0)
            {
                c >>= 1;
            }
            var n = 0;
            while ((c & 1) == 1)
            {
                c >>= 1;
                n++;
            }

            yield return n;
        }
    }

    public int WildcardCount()
    {
        var c = Wildcards;
        var n = 0;
        while (c > 0)
        {
            if ((c & 1) == 1) n++;
            c >>= 1;
        }

        return n;
    }
    
    public int BrokenCount()
    {
        var c = Broken;
        var n = 0;
        while (c > 0)
        {
            if ((c & 1) == 1) n++;
            c >>= 1;
        }

        return n;
    }
}

public static class BigIntegerExtensions
{
    public static string ToB128(this BigInteger n, bool trimStart = false)
    {
        const int chunkSize = 32;
        const int chunks = 4;
        IList<uint> parts = new List<uint>();
        for (var i = 0; i < chunks; i++)
        {
            var portion = (n >> i * chunkSize) & (((1uL << (chunkSize + 1)) - 1) >> 1);
            parts.Add((uint)portion);
        }

        var s = string.Join("", parts.Reverse().Select(p => p.ToString("b32")));
        return trimStart ? s.TrimStart('0') : s;
    }
}