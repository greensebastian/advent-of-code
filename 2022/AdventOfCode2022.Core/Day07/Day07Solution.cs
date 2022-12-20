namespace AdventOfCode2022.Core.Day07;

public record Day07Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var root = ParseToDirTree(Input);

        Print(root, Log);
        var totalSum = 0L;
        foreach (var dir in root.DirectoriesFlattened)
        {
            var size = dir.Size;
            if (size <= 100000) totalSum += size;
        }

        yield return totalSum.ToString();
    }

    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var root = ParseToDirTree(Input);

        var totalSpace = 70000000;
        var requiredFree = 30000000;
        var used = root.Size;

        var toRemove = used - (totalSpace - requiredFree);
        var removed = 0L;
        foreach (var dir in root.DirectoriesFlattened.OrderBy(dir => dir.Size))
        {
            var size = dir.Size;
            if (size >= toRemove)
            {
                removed = size;
                break;
            }
        }
        
        yield return removed.ToString();
    }
    
    private static Directory ParseToDirTree(IEnumerable<string> input)
    {
        var root = new Directory();
        var path = new Stack<Directory>(new[] { root });
        var lines = input.ToArray();

        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var line = lines[lineIndex];
            if (line[0] == '$')
            {
                var op = line[2..4];

                switch (op)
                {
                    case "ls":
                    {
                        while (lineIndex + 1 < lines.Length && lines[lineIndex + 1][0] != '$')
                        {
                            line = lines[++lineIndex];
                            if (line.StartsWith("dir"))
                                path.Peek().SubDirectories.TryAdd(line[4..], new Directory());
                            else
                                path.Peek().Files[line.Split(" ")[1]] = int.Parse(line.Split(" ")[0]);
                        }

                        break;
                    }
                    case "cd":
                        var arg = line[5..];
                        switch (arg)
                        {
                            case "/":
                                while (path.Count > 1)
                                {
                                    path.Pop();
                                }

                                break;
                            case "..":
                                path.Pop();
                                break;
                            default:
                                path.Push(path.Peek().SubDirectories[arg]);
                                break;
                        }

                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported operation '{op}'");
                }
            }
        }

        return root;
    }

    private static void Print(Directory dir, Action<string> log)
    {
        var lines = dir.ToStringLines();
        foreach (var line in lines)
        {
            log.Invoke(line);
        }
        log.Invoke(string.Empty);
    }
}

class Directory
{
    public Dictionary<string, long> Files { get; } = new();
    public Dictionary<string, Directory> SubDirectories { get; } = new();
    public long Size => DirectoriesFlattened.Sum(dir => dir.Files.Values.Sum());
    public IEnumerable<Directory> DirectoriesFlattened =>
        new[] { this }
        .Concat(SubDirectories.Values.SelectMany(dir => dir.DirectoriesFlattened));

    public IEnumerable<string> ToStringLines()
    {
        foreach (var file in Files.OrderBy(kv => kv.Key))
        {
            yield return $"{file.Key} ({file.Value})";
        }

        foreach (var dir in SubDirectories.OrderBy(kv => kv.Key))
        {
            yield return $"{dir.Key}";
            foreach (var line in dir.Value.ToStringLines())
            {
                yield return $"\t{line}";
            }
        }
    }
}