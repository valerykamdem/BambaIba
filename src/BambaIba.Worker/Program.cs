using BambaIba.Application.Extensions;
using BambaIba.Infrastructure.Extensions;
using BambaIba.Worker;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration);

builder.Services.AddHostedService<MediaConsumer>();

// 👉 Logger
builder.Services.AddLogging(config =>
{
    config.AddConsole();
});

IHost host = builder.Build();
host.Run();
