using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AdventOfCode2023.Core.Test;

public interface IInputSource
{
    Task<IList<string>?> GetLines(InputIdentifier id);
}

public interface IInputStore : IInputSource
{
    Task SetLines(InputIdentifier id, IList<string> lines);
}

public record InputIdentifier(int Year, int Day, string Suffix = "");

public class InputSource : IInputSource
{
    private readonly IInputStore _localStore;
    private readonly IInputStore _blobStore;
    private readonly IInputSource _aocClient;

    public InputSource(LocalStore localStore, BlobStore blobStore, AocClient aocClient)
    {
        _localStore = localStore;
        _blobStore = blobStore;
        _aocClient = aocClient;
    }
    
    public async Task<IList<string>?> GetLines(InputIdentifier id)
    {
        // Check Local
        var fromLocal = await _localStore.GetLines(id);
        if (fromLocal != null) return fromLocal;
        
        // Check Blob
        var fromBlob = await _blobStore.GetLines(id);
        if (fromBlob != null)
        {
            await _localStore.SetLines(id, fromBlob);
            return fromBlob;
        }
        
        // Check AoC
        var fromAoc = await _aocClient.GetLines(id);
        if (fromAoc != null)
        {
            await _blobStore.SetLines(id, fromAoc);
            await _localStore.SetLines(id, fromAoc);
            return fromAoc;
        }

        throw new Exception($"Could not get input for {id}");
    }
}

public class LocalStore : IInputStore
{
    private static string LocalAppDataFolder { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private static string LocalFilePath(InputIdentifier id) => Path.Combine(LocalAppDataFolder, "Advent of Code Test Runner", id.Year.ToString(), $"day{id.Day:00}{id.Suffix}.txt");
    
    public async Task<IList<string>?> GetLines(InputIdentifier id)
    {
        var localFilePath = LocalFilePath(id);
        if (!File.Exists(localFilePath)) return null;
        
        return await File.ReadAllLinesAsync(localFilePath);
    }

    public async Task SetLines(InputIdentifier id, IList<string> lines)
    {
        var localFilePath = LocalFilePath(id);
        var localFileDirectory = Path.GetDirectoryName(localFilePath);
        if (!Directory.Exists(localFileDirectory))
        {
            Directory.CreateDirectory(localFileDirectory!);
        }

        await File.WriteAllLinesAsync(localFilePath, lines);
    }
}

public class BlobStore : IInputStore
{
    private readonly BlobServiceClient _client;

    public BlobStore(BlobServiceClient client)
    {
        _client = client;
    }

    private static string ContainerName(InputIdentifier id) => $"aoc{id.Year}";
    private static string Filename(InputIdentifier id) => $"day{id.Day:00}{id.Suffix}.txt";
    
    public async Task<IList<string>?> GetLines(InputIdentifier id)
    {
        var containerClient = _client.GetBlobContainerClient(ContainerName(id));
        if (!await containerClient.ExistsAsync()) return null;
        var blobClient = containerClient.GetBlobClient(Filename(id));
        if (!await blobClient.ExistsAsync()) return null;
        
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

    public async Task SetLines(InputIdentifier id, IList<string> lines)
    {
        var containerClient = _client.GetBlobContainerClient(ContainerName(id));
        await containerClient.CreateIfNotExistsAsync();
        var blobClient = containerClient.GetBlobClient(Filename(id));

        var toWrite = System.Text.Encoding.UTF8.GetBytes(string.Join('\n', lines));
        await using var stream = await blobClient.OpenWriteAsync(true);
        await stream.WriteAsync(toWrite);
    }
}

public class AocClient : IInputSource
{
    private readonly HttpClient _client;

    public AocClient(HttpClient client)
    {
        _client = client;
        _client.BaseAddress = new Uri("https://adventofcode.com/");
    }
    
    public async Task<IList<string>?> GetLines(InputIdentifier id)
    {
        var cookie = GetSessionCookie();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{id.Year}/day/{id.Day}/input")
        {
            Headers =
            {
                { "Cookie", $"{cookie.Name}={cookie.Value}" }
            }
        };
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var stringContent = await response.Content.ReadAsStringAsync();
        return StoreUtil.TrimEmptyNewlineAtEnd(stringContent.Split("\n"));
    }

    private static ChromeManager.Cookie GetSessionCookie()
    {
        var cookies = ChromeManager.GetCookies("adventofcode.com");
        var sessionCookie = cookies.First(c => c.Name.Equals("session", StringComparison.InvariantCultureIgnoreCase));
        return sessionCookie;
    }
}

internal static class StoreUtil
{
    public static string[] TrimEmptyNewlineAtEnd(string[] lines) => string.IsNullOrWhiteSpace(lines.Last()) ? lines[..^1] : lines;
}