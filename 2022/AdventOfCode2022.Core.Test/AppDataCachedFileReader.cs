using Azure.Storage.Blobs;

namespace AdventOfCode2022.Core.Test;

public class AppDataCachedFileReader
{
    private readonly BlobContainerClient _client;

    public AppDataCachedFileReader(BlobServiceClient client)
    {
        _client = client.GetBlobContainerClient("aoc2022");
    }

    public async Task<string[]> GetLines(string filePath)
    {
        var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var fileName = Path.GetFileName(filePath);
        var localFilePath = Path.Combine(localAppDataFolder, "Advent of Code 2022 Test Runner", fileName);
        var localFileDirectory = Path.GetDirectoryName(localFilePath);

        if (File.Exists(localFilePath))
        {
            return await File.ReadAllLinesAsync(localFilePath);
        }

        var linesFromBlob = await GetLinesFromStorage(fileName);

        if (!Directory.Exists(localFileDirectory))
        {
            Directory.CreateDirectory(localFileDirectory!);
        }
        
        await File.WriteAllLinesAsync(localFilePath, linesFromBlob);
        return linesFromBlob;
    }

    private async Task<string[]> GetLinesFromStorage(string fileName)
    {
        var blobClient = _client.GetBlobClient(fileName);
        if (!await blobClient.ExistsAsync()) return Array.Empty<string>();
        
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