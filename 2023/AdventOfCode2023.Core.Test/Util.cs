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