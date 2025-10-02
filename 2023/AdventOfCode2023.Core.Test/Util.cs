using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace AdventOfCode2023.Core.Test;

public static class Util
{
    public static IEnumerable<string> ReadFromFile(string suffix, [CallerMemberName] string methodName = "",
        [CallerFilePath] string filePath = "", string extension = "txt")
    {
        var path = ChangeExtension(suffix, methodName, filePath, extension);
        return File.ReadLines(path);
    }

    public static async Task<string[]> GetInput(int year, int day)
    {
        using var host = new TestHost();
        var reader = host.Services.GetRequiredService<IInputSource>();
        var lines = await reader.GetLines(new InputIdentifier(year, day));
        if (lines is null) throw new Exception($"Could not get input for day {year}-{day:00}");
        return lines.ToArray();
    }
    
    public static async Task<string[]> ReadFromCachedFile(string fileName, string extension = "txt")
    {
        using var host = new TestHost();
        var reader = host.Services.GetRequiredService<AppDataCachedFileReader>();

        var fileNameWithExtension = Path.ChangeExtension(fileName, extension);
        return await reader.GetLines(fileNameWithExtension);
    }
    
    public static async Task<string[]> ReadFromCachedFileBySignature(string suffix, [CallerMemberName] string methodName = "",
        [CallerFilePath] string filePath = "", string extension = "txt")
    {
        using var host = new TestHost();
        var reader = host.Services.GetRequiredService<AppDataCachedFileReader>();
        
        var path = ChangeExtension(suffix, methodName, filePath, extension);
        return await reader.GetLinesForPath(path);
    }

    private static string ChangeExtension(string suffix, string methodName, string filePath, string extension) =>
        Path.ChangeExtension(filePath, $"{methodName}.{suffix}.{extension}");
        
    public static string[] ReadFile(string name)
    {
        var filename = Directory.GetFiles(Environment.CurrentDirectory, $"{name}.input.txt", SearchOption.AllDirectories).Single();
        var lines = File.ReadAllLines(filename);
        return CleanInput(lines);
    }

    public static string[] CleanInput(string[] lines)
    {
        if (string.IsNullOrWhiteSpace(lines[0])) lines = lines[1..];
        if (string.IsNullOrWhiteSpace(lines[^1])) lines = lines[..^1];
        return lines.Select(l => l.Trim()).ToArray();
    }
    
    public static string Repeat(this string input, int count)
    {
        if (count < 1) return "";
        if (string.IsNullOrEmpty(input) || count <= 1)
            return input;

        var builder = new StringBuilder(input.Length * count);

        for(var i = 0; i < count; i++) builder.Append(input);

        return builder.ToString();
    }

    public static string Repeat(this char input, int count) => input.ToString().Repeat(count);

    public static string Indent(this string text, int count) => " ".Repeat(count) + text;

    public static void AppendIndented(this StringBuilder sb, string text, int count)
    {
        foreach (var line in text.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)))
        {
            sb.AppendLine(Indent(line, count));
        }
    }
}