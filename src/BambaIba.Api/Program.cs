using BambaIba.Api.Endpoints;
using BambaIba.Api.Extensions;
using BambaIba.Api.Hubs;
using BambaIba.Api.Services;
using BambaIba.ApI.Hubs;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Abstractions.Services;
using BambaIba.Application.Extensions;
using BambaIba.Infrastructure.Extensions;
using Carter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;

// --- 1. INITIALISATION ---
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Configuration.AddEnvironmentVariables();

// --- 2. DÉTECTION DES MODES ---
bool isWorker = args.Contains("--worker-mode");
bool isStudio = args.Contains("--studio-mode");
bool isStreaming = args.Contains("--streaming-mode");

// Mode par défaut = streaming
bool isDefault = !isWorker && !isStudio && !isStreaming;

// --- 3. SERVICES COMMUNS ---
builder.Services.AddHttpClient();
builder.Services.AddLogging(config => config.AddConsole());

builder.Services.AddPresentation(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration);

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
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference(schemeId, document), new List<string>() }
    });
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/data-protection-keys"))
    .SetApplicationName("BambaIba");

builder.Services.AddOpenApi();
builder.Services.AddCarter();

// --- 4. SERVICES SPÉCIFIQUES PAR MODE ---

// STREAMING (API publique)
if (isStreaming || isDefault)
{
    builder.Services.AddSignalR();

    builder.Services.AddSingleton<IAzuraCastPollingService, AzuraCastPollingService>();
    builder.Services.AddHostedService<AzuraCastPollingService>();

    builder.Services.AddScoped<INotificationService, SignalRNotificationService>();
}

// STUDIO (API créateurs)
if (isStudio)
{
    // Pas de services spécifiques pour l'instant
}

// WORKER (Traitement vidéo)
if (isWorker)
{
    builder.Services.AddHostedService<WorkerService>();
}

// Kestrel uniquement pour Streaming + Studio
if (!isWorker && builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(7000));
}

// --- 5. BUILD ---
WebApplication app = builder.Build();

// --- 6. MODE WORKER ---
if (isWorker)
{
    app.MapHealthChecks("health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    await app.RunAsync();
    return;
}

// --- 7. MODE API (Streaming ou Studio) ---

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapHealthChecks("health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
app.UseCors("AllowAll");
app.UseGlobalExceptionHandler();
app.UseSerilogRequestLogging();
app.UseRequestContextLogging();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

// --- 8. ENDPOINTS STREAMING ---
if (isStreaming || isDefault)
{
    app.MapHub<LiveChatHub>("/Hubs/LiveChatHub");
    app.MapHub<NotificationHub>("/Hubs/NotificationHub");

    new AuthEndpoints().AddRoutes(app);
    new SearchEndpoints().AddRoutes(app);
    new RadioEndpoints().AddRoutes(app);
    new StreamingMediaEndpoints().AddRoutes(app);
    new CommentEndpoints().AddRoutes(app);
}

// --- 9. ENDPOINTS STUDIO ---
if (isStudio)
{
    new StudioMediaEndpoints().AddRoutes(app);
    new PlaylistEndpoints().AddRoutes(app);
}

app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

await app.RunAsync();



//using BambaIba.Api.Extensions;
//using BambaIba.Api.Hubs;
//using BambaIba.Api.Services;
//using BambaIba.ApI.Hubs;
//using BambaIba.Application.Abstractions.Interfaces;
//using BambaIba.Application.Abstractions.Services;
//using BambaIba.Application.Extensions;
//using BambaIba.Infrastructure.Extensions;
//using BambaIba.Infrastructure.Persistence;
//using Carter;
//using HealthChecks.UI.Client;
//using Microsoft.AspNetCore.DataProtection;
//using Microsoft.AspNetCore.Diagnostics.HealthChecks;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.OpenApi;
//using Scalar.AspNetCore;
//using Serilog;
//using Wolverine;
//using Wolverine.Postgresql;

//WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Host.UseSerilog((context, loggerConfig) =>
//    loggerConfig.ReadFrom.Configuration(context.Configuration));

////// Configurations
////builder.Services.AddControllers();

//builder.Configuration.AddEnvironmentVariables();


//builder.Services.AddSignalR();
//builder.Services.AddHttpClient();

//builder.Services.AddSingleton<IAzuraCastPollingService, AzuraCastPollingService>();
//builder.Services.AddHostedService<AzuraCastPollingService>();

//builder.Services.AddScoped<INotificationService, SignalRNotificationService>();

//// 👉 Logger
//builder.Services.AddLogging(config =>
//{
//    config.AddConsole();
//});

////// Enregistre la section RabbitMQ dans RabbitMqOptions
////builder.Services.Configure<RabbitMqOptions>(
////    builder.Configuration.GetSection("RabbitMQ"));

//builder.Services.Configure<RadioLiveOptions>(
//    builder.Configuration.GetSection("RadioLive"));

////builder.Host.UseWolverine();

//// Services
//builder.Services.AddPresentation(builder.Configuration)
//    .AddApplicationServices(builder.Configuration)
//    .AddInfrastructureServices(builder.Configuration);


//// ✅ CORS pour SignalR
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy.WithOrigins("http://localhost:3000", "http://localhost:7000", "http://localhost:8005")
//              .AllowAnyMethod()
//              .AllowAnyHeader()
//              .AllowCredentials();
//    });
//});

//builder.Services.AddSwaggerGen(c =>
//{
//    const string schemeId = "Bearer";
//    c.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
//    {
//        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.Http,
//        Scheme = "bearer",
//        BearerFormat = "JWT"
//    });
//    c.AddSecurityRequirement(document =>
//    {
//        return new OpenApiSecurityRequirement
//        {
//            {
//                // Utilise le constructeur de OpenApiSecuritySchemeReference
//                new OpenApiSecuritySchemeReference(schemeId, document),
//                new List<string>() // Liste de scopes vide pour JWT
//            }
//        };
//    });
//});

//builder.Services.AddDataProtection()
//    .PersistKeysToFileSystem(new DirectoryInfo("/app/data-protection-keys"))
//    .SetApplicationName("BambaIba");

//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

//builder.Services.AddCarter();

//if (builder.Environment.IsDevelopment())
//{
//    builder.WebHost.ConfigureKestrel(options =>
//    {
//        options.ListenAnyIP(7000); // HTTP seulement
//    });
//}

//bool isWorker = args.Contains("--worker-mode");

//if (!isWorker)
//{
//    WebApplication app = builder.Build();

//    //// Configure the HTTP request pipeline.
//    if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
//    {
//        app.MapOpenApi();
//        app.MapScalarApiReference();
//        app.UseSwagger();
//        app.UseSwaggerUI();
//        //app.ApplyMigrations();
//    }

//    app.UseHttpsRedirection();

//    app.MapHealthChecks("health", new HealthCheckOptions
//    {
//        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
//    });

//    app.UseCors("AllowAll");
//    //app.MapEndpoints();
//    app.MapCarter();
//    app.UseGlobalExceptionHandler();
//    app.UseSerilogRequestLogging();
//    app.UseRequestContextLogging();
//    app.UseExceptionHandler();


//    app.UseAuthentication();
//    app.UseAuthorization();

//    app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

//    // ✅ Mapper le Hub
//    app.MapHub<LiveChatHub>("/Hubs/LiveChatHub");
//    app.MapHub<NotificationHub>("/Hubs/NotificationHub");


//    //app.Use(async (context, next) =>
//    //{
//    //    using IServiceScope scope = context.RequestServices.CreateScope();
//    //    IServiceProvider services = scope.ServiceProvider;
//    //    IWebHostEnvironment env = services.GetRequiredService<IWebHostEnvironment>();

//    //    if (env.IsDevelopment())
//    //    {
//    //        BIDbContext db = services.GetRequiredService<BIDbContext>();
//    //        // Force la création des tables si elles n’existent pas encore
//    //        await db.Database.EnsureCreatedAsync();
//    //        db.Database.Migrate();
//    //        SeedData.Initialize(services);
//    //    }

//    //    await next();
//    //});

//    await app.RunAsync();
//}
//else
//{
//    // --- MODE WORKER ---
//    // On construit le Host mais sans app.Run() (qui lance Kestrel/HTTP)

//    // On enregistre notre WorkerService
//    builder.Services.AddHostedService<WorkerService>();

//    WebApplication host = builder.Build();

//    // Ici, on lance juste le Host.
//    // Le Host va démarrer Wolverine, qui va se connecter à Postgres.
//    // Le Host va aussi lancer WorkerService.
//    await host.RunAsync();
//}
