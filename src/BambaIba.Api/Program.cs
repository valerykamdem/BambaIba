using BambaIba.Api.Extensions;
using BambaIba.Api.Hubs;
using BambaIba.Application.Extensions;
using BambaIba.Application.Settings;
using BambaIba.Infrastructure.Extensions;
using BambaIba.Infrastructure.Persistence;
using Carter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

//// Configurations
//builder.Services.AddControllers();

builder.Services.AddSignalR();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<IAzuraCastPollingService, AzuraCastPollingService>();
builder.Services.AddHostedService<AzuraCastPollingService>();

// 👉 Logger
builder.Services.AddLogging(config =>
{
    config.AddConsole();
});

// Enregistre la section RabbitMQ dans RabbitMqOptions
builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.Configure<RadioLiveOptions>(
    builder.Configuration.GetSection("RadioLive"));

//builder.Host.UseWolverine();

// Services
builder.Services.AddPresentation(builder.Configuration)
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

// ✅ CORS pour SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:7000", "http://localhost:8005")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddSwaggerGen(c =>
{
    const string schemeId = "Bearer";
    c.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(document =>
    {
        return new OpenApiSecurityRequirement
        {
            {
                // Utilise le constructeur de OpenApiSecuritySchemeReference
                new OpenApiSecuritySchemeReference(schemeId, document),
                new List<string>() // Liste de scopes vide pour JWT
            }
        };
    });
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/data-protection-keys"))
    .SetApplicationName("BambaIba");

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCarter();

//builder.Services.AddCors();

if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(7000); // HTTP seulement
    });
}

WebApplication app = builder.Build();

//// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseCors("AllowAll");
//app.MapEndpoints();
app.MapCarter();
app.UseGlobalExceptionHandler();
app.UseSerilogRequestLogging();
app.UseRequestContextLogging();
app.UseExceptionHandler();


////app.UseCors("AllowAngularOrigins");
//app.UseCors(options =>
//{
//    options.AllowAnyHeader();
//    options.AllowAnyMethod();
//    options.AllowAnyOrigin();
//});

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

// ✅ Mapper le Hub
app.MapHub<LiveHub>("/Hubs/LiveHub");

app.Use(async (context, next) =>
{
    using IServiceScope scope = context.RequestServices.CreateScope();
    IServiceProvider services = scope.ServiceProvider;
    IWebHostEnvironment env = services.GetRequiredService<IWebHostEnvironment>();

    if (env.IsDevelopment())
    {
        BambaIbaDbContext db = services.GetRequiredService<BambaIbaDbContext>();
        // Force la création des tables si elles n’existent pas encore
        await db.Database.EnsureCreatedAsync();
        db.Database.Migrate();
        SeedData.Initialize(services);
    }

    await next();
});

app.Use(async (context, next) =>
{
    IHttpMaxRequestBodySizeFeature? maxRequestBodySizeFeature = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
    if (maxRequestBodySizeFeature != null && !context.Request.Path.StartsWithSegments("/upload"))
    {
        maxRequestBodySizeFeature.MaxRequestBodySize = null; // désactive la limite
    }

    await next();
});

await app.RunAsync();
