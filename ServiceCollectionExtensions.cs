using JasperFx.CodeGeneration;
using Marten;
using Marten.Services;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;

namespace MartenApplyAutoCreateBeforeRun;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMartenPersistence(this IServiceCollection services)
    {
        string domainEntityDatabaseConnectionString = "host=localhost;port=5432;database=MyDbo;password=servus;username=postgres";

        string maintenanceDatabaseConnectionString = "host=localhost;port=5432;database=postgres;password=servus;username=postgres";

        services.AddMartenStore<IMyDocumentStore>(options =>
        {
            options.Connection(domainEntityDatabaseConnectionString);
            options.Serializer(new SystemTextJsonSerializer());

            options.Schema.For<Dbo>()
                .DatabaseSchemaName("Dbo")
                .Identity(dbo => dbo.Id)
                .UseOptimisticConcurrency(true);

            options.AutoCreateSchemaObjects = AutoCreate.All;

            options.CreateDatabasesForTenants(expressions =>
            {
                expressions.MaintenanceDatabase(maintenanceDatabaseConnectionString);

                expressions.ForTenant()
                    .CheckAgainstPgDatabase()
                    .WithOwner("postgres")
                    .WithEncoding("UTF-8")
                    .ConnectionLimit(-1);
            });

            options.GeneratedCodeMode = TypeLoadMode.Auto;
        });

        return services;
    }
}