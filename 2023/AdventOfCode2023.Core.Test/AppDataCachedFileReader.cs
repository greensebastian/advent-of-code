using Azure.Storage.Blobs;

namespace AdventOfCode2023.Core.Test;

public class AppDataCachedFileReader
{
    private readonly BlobContainerClient _client;

    public AppDataCachedFileReader(BlobServiceClient client)
    {
        _client = client.GetBlobContainerClient("aoc2023");
    }

    public async Task<string[]> GetLinesForPath(string filePath, bool skipFinalNewline = true)
    {
        var fileName = Path.GetFileName(filePath);

        return await GetLines(fileName, skipFinalNewline);
    }
    
    public async Task<string[]> GetLines(string fileName, bool skipFinalNewline = true)
    {
        var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var localFilePath = Path.Combine(localAppDataFolder, "Advent of Code 2023 Test Runner", fileName);
        var localFileDirectory = Path.GetDirectoryName(localFilePath);

        if (File.Exists(localFilePath))
        {
            var lines = await File.ReadAllLinesAsync(localFilePath);
            return TrimEmptyNewlineAtEnd(lines, skipFinalNewline);
        }

        var linesFromBlob = await GetLinesFromStorage(fileName);

        if (!Directory.Exists(localFileDirectory))
        {
            Directory.CreateDirectory(localFileDirectory!);
        }
        
        await File.WriteAllLinesAsync(localFilePath, linesFromBlob);
        return TrimEmptyNewlineAtEnd(linesFromBlob, skipFinalNewline);
    }

    private string[] TrimEmptyNewlineAtEnd(string[] lines, bool skipFinalNewline)
    {
        return skipFinalNewline && string.IsNullOrWhiteSpace(lines.Last()) ? lines[..^1] : lines;
    }

    private async Task<string[]> GetLinesFromStorage(string fileName)
    {
        var blobClient = _client.GetBlobClient(fileName);
        if (!await blobClient.ExistsAsync())
        {
            throw new ArgumentException($"File {fileName} does not exist in storage");
        }
        
        var content = await blobClient.DownloadContentAsync();
        using var reader = new StreamReader(content.Value.Content.ToStream());
        var output = new List<string>();
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            output.Add(line!);
        }

        return output.ToArray();
    }
}