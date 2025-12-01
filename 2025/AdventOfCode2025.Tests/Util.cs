namespace AdventOfCode2025.Tests;

public static class Util
{
    public static string[] ReadFile(string name)
    {
        var filename = Directory.GetFiles(Environment.CurrentDirectory, $"{name}.input.txt", SearchOption.AllDirectories).Single();
        var lines = File.ReadAllLines(filename);
        return CleanInput(lines);
    }

    private static string[] CleanInput(string[] lines)
    {
        if (string.IsNullOrWhiteSpace(lines[0])) lines = lines[1..];
        if (string.IsNullOrWhiteSpace(lines[^1])) lines = lines[..^1];
        return lines.Select(l => l.Trim()).ToArray();
    }
    
    public static string[] ReadRaw(string lines) => CleanInput(lines.Split("\n"));
}