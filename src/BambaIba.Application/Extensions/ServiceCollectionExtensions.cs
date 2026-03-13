using System.Data.Common;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Postgresql;


namespace BambaIba.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Enregistrement des validateurs
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        // 2. Configuration de Wolverine
        services.AddWolverine(opts =>
        {
            //opts.Discovery.DisableConventionalDiscovery();

            opts.Discovery.IncludeAssembly(typeof(ServiceCollectionExtensions).Assembly);

            // C. Configuration Postgres (PersistMessagesWithPostgresql est correct)
            opts.PersistMessagesWithPostgresql(
                configuration.GetConnectionString("Postgres"),
                schemaName: "wolverine");

            // D. Configuration de la durabilité
            opts.Durability.Mode = DurabilityMode.Solo;
            opts.Durability.EnableInboxPartitioning = true;

            // E. Politiques de transaction et retry
            opts.Policies.UseDurableInboxOnAllListeners();
            opts.Policies.UseDurableLocalQueues();
            opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
            opts.Policies.AutoApplyTransactions();

            // F. Polling & Retry (Tes paramètres sont bons)
            opts.Durability.HealthCheckPollingTime = TimeSpan.FromSeconds(10);
            opts.Durability.NodeReassignmentPollingTime = TimeSpan.FromSeconds(5);
            opts.Durability.ScheduledJobPollingTime = TimeSpan.FromSeconds(5);

            //opts.Policies.OnException<TimeoutException>().RetryWithCooldown(TimeSpan.FromSeconds(5));
            opts.Policies.OnException<DbException>().RetryTimes(5);
            opts.Policies.OnException<OperationCanceledException>().RetryTimes(3);
            opts.Policies.OnException<Exception>().MoveToErrorQueue();

            // Dans ta config Wolverine
            opts.Policies.OnException<NpgsqlException>()
                .RetryWithCooldown(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30));

            opts.Policies.OnException<TimeoutException>()
                .RetryWithCooldown(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30));

            // Capture aussi l'exception spécifique EF Core qui wrap le timeout
            opts.Policies.OnException<Microsoft.EntityFrameworkCore.DbUpdateException>()
                .RetryWithCooldown(TimeSpan.FromSeconds(5));

        });

        return services;
    }

    //Assembly assembly = typeof(ServiceCollectionExtensions).Assembly;

    //services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

    ////builder.Host.UseWolverine();
    //services.AddWolverine(opts =>
    //{
    //    //opts.Durability.WorkerCount = 1; // 🔥 Important pour ffmpeg
    //    opts.Discovery.DisableConventionalDiscovery();

    //    opts.PersistMessagesWithPostgresql(
    //        configuration.GetConnectionString("Postgres"),
    //        schemaName: "wolverine");

    //    opts.Durability.Mode = DurabilityMode.Solo;

    //    opts.Durability.EnableInboxPartitioning = true;

    //    opts.Policies.UseDurableInboxOnAllListeners();
    //    opts.Policies.UseDurableLocalQueues();
    //    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();

    //    // Polling
    //    opts.Durability.HealthCheckPollingTime = TimeSpan.FromSeconds(10);
    //    opts.Durability.NodeReassignmentPollingTime = TimeSpan.FromSeconds(5);
    //    opts.Durability.ScheduledJobPollingTime = TimeSpan.FromSeconds(5);

    //    // Retry
    //    opts.Policies.OnException<TimeoutException>()
    //        .RetryWithCooldown(TimeSpan.FromSeconds(5));

    //    opts.Policies.OnException<DbException>()
    //        .RetryTimes(5);

    //    opts.Policies.OnException<OperationCanceledException>()
    //        .RetryTimes(3);

    //    opts.Policies.OnException<Exception>()
    //        .MoveToErrorQueue();

    //    opts.Policies.AutoApplyTransactions();
    //});

    //return services;
}
