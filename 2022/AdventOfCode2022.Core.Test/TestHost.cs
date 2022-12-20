using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AdventOfCode2022.Core.Test;

public class TestHost : IDisposable
{
    private readonly IHost _host;
    
    public IServiceProvider Services { get; }
    
    public TestHost()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureHostConfiguration(conf =>
            {
                conf.AddJsonFile("appsettings.shared.json");
                conf.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddAzureClients(builder =>
                {
                    var azureConf = context.Configuration.GetSection("Azure").Get<AzureConfig>()!;
                    builder.AddBlobServiceClient(new Uri($"https://{azureConf.StorageAccountName}.blob.core.windows.net"));
                    builder.UseCredential(
                        new ClientSecretCredential(azureConf.TenantId, azureConf.ClientId, azureConf.ClientSecret));
                });
                services.AddSingleton<AppDataCachedFileReader>();
            });

        _host = builder.Build();
        Services = _host.Services;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _host.Dispose();
    }
}