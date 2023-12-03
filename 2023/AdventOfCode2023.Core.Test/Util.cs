using System.Runtime.CompilerServices;
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
}

public static class EnumerableExtensions
{
    public static IEnumerable<T[]> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
        var batch = new List<T>();
        foreach (var item in source)
        {
            batch.Add(item);
            if (batch.Count >= batchSize)
            {
                yield return batch.ToArray();
                batch.Clear();
            }
        }

        if (batch.Count > 0)
            yield return batch.ToArray();
    }

    public static IEnumerable<int> Ints(this IEnumerable<char> source)
    {
        var currentNumber = string.Empty;
        foreach (var c in source)
        {
            if (char.IsNumber(c))
            {
                currentNumber += c;
            }
            else
            {
                if (string.IsNullOrEmpty(currentNumber)) continue;
                
                yield return int.Parse(currentNumber);
                currentNumber = string.Empty;
            }
        }

        if (currentNumber.Length > 0) yield return int.Parse(currentNumber);
    }
}