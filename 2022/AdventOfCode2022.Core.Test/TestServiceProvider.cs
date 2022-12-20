using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AdventOfCode2022.Core.Test;

public class TestServiceProvider
{
    public IServiceProvider Services { get; }
    
    public TestServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddEnvironmentVariables()
            .Build();

        foreach (var pair in configuration.AsEnumerable())
        {
            Console.WriteLine($"Key: [{pair.Key}] ({pair.Value?.Length})");
        }

        var azureConf = configuration.GetSection("Azure").Get<AzureConfig>()!;

        var builder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddAzureClients(x =>
                {
                    x.AddBlobServiceClient(new Uri($"https://{azureConf.StorageAccountName}.blob.core.windows.net"));
                    x.UseCredential(
                        new ClientSecretCredential(azureConf.TenantId, azureConf.ClientId, azureConf.ClientSecret));
                });
                services.AddSingleton<AppDataCachedFileReader>();
            });

        Services = builder.Build().Services;
    }
}