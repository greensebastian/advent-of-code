namespace AdventOfCode2022.Core.Test;

public class AzureConfig
{
    public required string StorageAccountName { get; init; }
    public required string TenantId { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
}