using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Polly;
using Weasel.Core;

namespace MartenApplyAutoCreateBeforeRun;

public static class HostBuilderExtension
{
    public static async Task RegisterStartup<THostRunner>(this IHost host)
    {
        await host
            .RunDatabaseSetup<THostRunner>();
    }

    private static async Task RunDatabaseSetup<THostRunner>(this IHost host, int retryDelay = 3)
    {
        var warehouseTaskDocumentStore = host.Services.GetRequiredService<IMyDocumentStore>();

        var retryPolicy = Policy.Handle<NpgsqlException>()
            .WaitAndRetryForeverAsync((_, _) => TimeSpan.FromSeconds(retryDelay),
                (exception, _, _) => { Console.WriteLine("Test"); });

        await retryPolicy.ExecuteAsync(
            async _ => { await ApplyAllConfigurationChangesToDatabase(warehouseTaskDocumentStore); }, CancellationToken.None);
    }

    private static async Task ApplyAllConfigurationChangesToDatabase(
        IMyDocumentStore myDocumentStore)
    {
        await myDocumentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync(AutoCreate.All);
    }
}