using System.Reflection;
using BambaIba.Application.Abstractions.Services;
using Cortex.Mediator.DependencyInjection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;

namespace BambaIba.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        Assembly assembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        //builder.Host.UseWolverine();

        services.AddWolverine(opts =>
        {
            // Découverte automatique des handlers dans l’assembly Application
            opts.Discovery.IncludeAssembly(typeof(ServiceCollectionExtensions).Assembly);

            //// Exemple : si tu veux RabbitMQ
            //opts.UseRabbitMq("amqp://guest:guest@rabbitmq:5672")
            //    .AutoProvision()
            //    .AutoPurgeOnStartup();

            // Politique de transactions automatiques
            opts.Policies.AutoApplyTransactions();
        });

        services.AddScoped<MediaPublisher>();

        services.AddCortexMediator(configuration,
            [typeof(ServiceCollectionExtensions)], // Scanne l'assembly pour trouver les handlers
           options => options.AddDefaultBehaviors());

        return services;
    }

    //private static void AddRedis(this IServiceCollection services, IConfiguration configuration)
    //{
    //    string connectionString = configuration.GetConnectionString("Redis") ??
    //                              throw new ArgumentNullException(nameof(configuration));

    //    services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);

    //    var redis = ConnectionMultiplexer.Connect(connectionString);
    //    services.AddSingleton<IConnectionMultiplexer>(redis);
    //}

    //private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    //{

    //    string connectionString = configuration.GetConnectionString("DefaultConnection") ??
    //                throw new ArgumentNullException(nameof(configuration));

    //    services.AddDbContext<BambaIbaDbContext>(options =>
    //    options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
    //        .EnableSensitiveDataLogging()
    //        .LogTo(Console.WriteLine, LogLevel.Information));

    //    services.AddScoped<IVideoProcessingService, FFmpegVideoProcessingService>();
    //    services.AddScoped<IVideoStorageService, MinIOVideoStorageService>();
    //    services.AddScoped<IVideoRepository, VideoRepository>();
    //    services.AddScoped<IVideoQualityRepository, VideoQualityRepository>();

    //    services.Configure<MinIOSettings>(
    //        configuration.GetSection(MinIOSettings.SectionName));

    //    services.Configure<FFmpegSettings>(
    //        configuration.GetSection(FFmpegSettings.SectionName));

    //    //services.AddScoped<ITransferIdGenerator, TransferIdGenerator>();

    //    //services.AddScoped<IPendingTransactionRedisStore, PendingTransactionRedisStore>();
    //    //services.AddScoped<IPendingUpdateUserRedisStore, PendingUpdateUserRedisStore>();

    //    //services.AddScoped<IOperationRepository, OperationRepository>();

    //    //services.AddScoped<IAccountRepository, AccountRepository>();

    //    //services.AddScoped<IAccountLogRepository, AccountLogRepository>();

    //    //services.AddScoped<IUserRepository, UserRepository>();

    //    //services.AddScoped<IUserPinRepository, UserPinRepository>();

    //    //services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

    //    //services.AddScoped<IUnitOfWork, UnitOfWork>();

    //    // Add Minio using the default endpoint
    //    services.AddMinio(configuration["MinIO:AccessKey"], configuration["MinIO:SecretKey"]);

    //    services.AddSingleton<ISqlConnectionFactory>(_ =>
    //     new SqlConnectionFactory(connectionString));

    //    SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    //}

    //private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    //{

    //    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //.AddJwtBearer(options =>
    //{
    //    string? keycloakUrl = configuration["Keycloak:BaseUrl"];
    //    string? realm = configuration["Keycloak:Realm"];
    //    string? audience = configuration["Keycloak:Audience"];

    //    options.Authority = $"{keycloakUrl}/realms/{realm}";
    //    options.Audience = audience;
    //    options.RequireHttpsMetadata = false; // Dev only

    //    options.TokenValidationParameters = new TokenValidationParameters
    //    {
    //        ValidateIssuer = true,
    //        ValidIssuer = $"{keycloakUrl}/realms/{realm}",
    //        ValidateAudience = true,
    //        ValidAudience = audience,
    //        ValidateLifetime = true,
    //        ValidateIssuerSigningKey = true, // 🔑 Obligatoire
    //        ClockSkew = TimeSpan.Zero
    //    };

    //    options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
    //            $"{keycloakUrl}/realms/{realm}/.well-known/openid-configuration",
    //            new OpenIdConnectConfigurationRetriever(),
    //            new HttpDocumentRetriever { RequireHttps = false } // ✅ pour autoriser HTTP en dev
    //        );

    //    options.BackchannelHttpHandler = new HttpClientHandler
    //    {
    //        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    //    };

    //    options.Events = new JwtBearerEvents
    //    {
    //        OnMessageReceived = context => Task.CompletedTask,
    //        OnAuthenticationFailed = context => Task.CompletedTask
    //    };
    //});

    //    //services.Configure<KeycloakSettings>(
    //    //    configuration.GetSection(KeycloakSettings.SectionName));

    //    services.Configure<KeycloakSettings>(
    //        configuration.GetSection(KeycloakSettings.SectionName));

    //    //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

    //    //services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));

    //    //services.ConfigureOptions<JwtBearerOptionsSetup>();

    //    //services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

    //    //services.AddTransient<AdminAuthorizationDelegatingHandler>();

    //    //services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
    //    //{
    //    //    KeycloakOptions keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

    //    //    httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
    //    //})
    //    //.AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

    //    //services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpClient) =>
    //    //{
    //    //    KeycloakOptions keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

    //    //    httpClient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
    //    //});

    //    services.AddHttpContextAccessor();
    //    services.AddHttpClient<IKeycloakAuthService, KeycloakAuthService>();
    //    services.AddScoped<IUserContextService, UserContextService>();

    //}

    //private static void AddAuthorization(IServiceCollection services)
    //{
    //    services.AddAuthorization();

    //    //services.AddScoped<AuthorizationService>();

    //    //services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();

    //    //services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

    //    //services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    //}
}
