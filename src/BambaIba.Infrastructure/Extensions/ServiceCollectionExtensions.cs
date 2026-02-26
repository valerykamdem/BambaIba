using Amazon.S3;
using BambaIba.Application.Abstractions.Caching;
using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Abstractions.Services;
using BambaIba.Infrastructure.Caching;
using BambaIba.Infrastructure.Persistence;
using BambaIba.Infrastructure.Repositories.Authentications;
using BambaIba.Infrastructure.Services;
using BambaIba.Infrastructure.Settings;
using BambaIba.Infrastructure.Time;
using BambaIba.SharedKernel;
using Dapper;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Npgsql;
using StackExchange.Redis;

namespace BambaIba.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
        .AddServices()
        .AddRedis(configuration)
        .AddPersistence(configuration)
        .AddMongo(configuration)
        .AddSeaweed(configuration)
        .Authentication(configuration)
        .AddHealthChecks(configuration)
        .AddElasticSearch(configuration);

    //Assembly assembly = typeof(ServiceCollectionExtensions).Assembly;

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }

    private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Redis") ??
                                  throw new ArgumentNullException(nameof(configuration));

        services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);

        var redis = ConnectionMultiplexer.Connect(connectionString);
        services.AddSingleton<IConnectionMultiplexer>(redis);

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {

        string connectionString = configuration.GetConnectionString("Postgres") ??
                    throw new ArgumentNullException(nameof(configuration));

        services.AddSingleton<IDbConnectionFactory>(_ =>
        new DbConnectionFactory(new NpgsqlDataSourceBuilder(connectionString).Build()));

        services.AddDbContext<BIDbContext>(
            options => options
                .UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default)
                    .EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null
        )).UseSnakeCaseNamingConvention());

        services.AddScoped<IBIDbContext>(
            sp => sp.GetRequiredService<BIDbContext>());

        //services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<BIDbContext>());

        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IUserContextService, UserContextService>();
        // Services métiers
        services.AddScoped<IMediaStorageService, MediaStorageService>();
        services.AddScoped<IMediaProcessingService, FFmpegMediaProcessingService>();
        services.AddScoped<IMediaStatisticsService, MediaStatisticsService>();

        services.AddHttpContextAccessor();
        services.AddHttpClient<IKeycloakAuthService, KeycloakAuthService>();

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

        return services;
    }

    private static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.Configure<MongoSettings>(
            configuration.GetSection(MongoSettings.SectionName));

        //// Register AmazonS3Client
        // 2. Créer le Client Mongo (Singleton)
        services.AddSingleton<IMongoClient, MongoClient>(sp =>
        {
            MongoSettings settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        // 3. Créer ton Contexte Mongo (qui utilise le client)
        services.AddSingleton<IBIMongoContext, BIMongoContext>();

        return services;
    }

    private static IServiceCollection AddSeaweed(this IServiceCollection services, IConfiguration configuration)
    {
        //// Add Minio using the default endpoint
        // 1. Enregistrer la configuration pour l'injection (IOptions)
        services.Configure<SeaweedSettings>(
            configuration.GetSection(SeaweedSettings.SectionName));

        //// Register AmazonS3Client
        services.AddSingleton<IAmazonS3>(sp =>
        {
            SeaweedSettings cfg = sp.GetRequiredService<IOptions<SeaweedSettings>>().Value;

            var config = new AmazonS3Config
            {
                ServiceURL = cfg.Endpoint,          // http://localhost:8333
                ForcePathStyle = true,
                //SignatureVersion = "4",
                UseHttp = true
            };

            var credentials = new Amazon.Runtime.BasicAWSCredentials(
                cfg.AccessKey,
                cfg.SecretKey
            );

            return new AmazonS3Client(credentials, config);
        });


        services.AddHostedService<S3Initializer>();


        services.Configure<FFmpegSettings>(
            configuration.GetSection(FFmpegSettings.SectionName));
        return services;
    }

    private static IServiceCollection Authentication(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        string? keycloakUrl = configuration["Keycloak:BaseUrl"];
        string? realm = configuration["Keycloak:Realm"];
        string? audience = configuration["Keycloak:Audience"];

        options.Authority = $"{keycloakUrl}/realms/{realm}";
        options.Audience = audience;
        options.RequireHttpsMetadata = false; // Dev only

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"{keycloakUrl}/realms/{realm}",
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true, // 🔑 Obligatoire
            ClockSkew = TimeSpan.Zero
        };

        options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{keycloakUrl}/realms/{realm}/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever { RequireHttps = false } // ✅ pour autoriser HTTP en dev
            );

        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context => Task.CompletedTask,
            OnAuthenticationFailed = context => Task.CompletedTask
        };
    });

        services.Configure<KeycloakSettings>(
            configuration.GetSection(KeycloakSettings.SectionName));

        services.AddAuthorization();

        return services;

    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Postgres")!) // Use the correct connection string name
            .AddRedis(configuration.GetConnectionString("Redis")!);

        return services;
    }

    private static IServiceCollection AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
    {
        string elasticUrl = configuration["ElasticSearch:Url"];
        ElasticsearchClientSettings settings = new ElasticsearchClientSettings(new Uri(elasticUrl))
            .DefaultIndex("media_index"); // Nom de ton index par défaut

        var client = new ElasticsearchClient(settings);

        services.AddSingleton<ElasticsearchClient>(client);

        return services;
    }
}
