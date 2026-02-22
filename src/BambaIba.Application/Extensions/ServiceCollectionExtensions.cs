using System.Data.Common;
using System.Reflection;
using Cortex.Mediator.DependencyInjection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Postgresql;


namespace BambaIba.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        Assembly assembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        //builder.Host.UseWolverine();
        services.AddWolverine(opts =>
        {
            //opts.Durability.WorkerCount = 1; // 🔥 Important pour ffmpeg

            opts.PersistMessagesWithPostgresql(
                configuration.GetConnectionString("Postgres"),
                schemaName: "wolverine");

            opts.Durability.Mode = DurabilityMode.Solo;

            opts.Durability.EnableInboxPartitioning = true;

            opts.Policies.UseDurableInboxOnAllListeners();
            opts.Policies.UseDurableLocalQueues();
            opts.Policies.UseDurableOutboxOnAllSendingEndpoints();

            // Polling
            opts.Durability.HealthCheckPollingTime = TimeSpan.FromSeconds(10);
            opts.Durability.NodeReassignmentPollingTime = TimeSpan.FromSeconds(5);
            opts.Durability.ScheduledJobPollingTime = TimeSpan.FromSeconds(5);

            // Retry
            opts.Policies.OnException<TimeoutException>()
                .RetryWithCooldown(TimeSpan.FromSeconds(5));

            opts.Policies.OnException<DbException>()
                .RetryTimes(5);

            opts.Policies.OnException<OperationCanceledException>()
                .RetryTimes(3);

            opts.Policies.OnException<Exception>()
                .MoveToErrorQueue();

            opts.Policies.AutoApplyTransactions();
        });


        services.AddCortexMediator(
            [typeof(ServiceCollectionExtensions)], // Scanne l'assembly pour trouver les handlers
           options => options.AddDefaultBehaviors());

        return services;
    }
}
