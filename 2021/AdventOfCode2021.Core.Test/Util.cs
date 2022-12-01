using System.Runtime.CompilerServices;

namespace AdventOfCode2021.Core.Test;

public static class Util
{
    public static IEnumerable<string> ReadFromFile(string suffix, [CallerMemberName] string methodName = "",
        [CallerFilePath] string filePath = "", string extension = "txt")
    {
        var path = Path.ChangeExtension(filePath, $"{methodName}.{suffix}.{extension}");
        return File.ReadLines(path);
    }
}