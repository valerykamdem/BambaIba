using BambaIba.Application.Abstractions.Caching;
using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Audios;
using BambaIba.Domain.Comments;
using BambaIba.Domain.Likes;
using BambaIba.Domain.LiveChatMessages;
using BambaIba.Domain.LiveStream;
using BambaIba.Domain.MediaBase;
using BambaIba.Domain.PlaylistItems;
using BambaIba.Domain.Playlists;
using BambaIba.Domain.VideoQualities;
using BambaIba.Domain.Videos;
using BambaIba.Infrastructure.Caching;
using BambaIba.Infrastructure.Persistence;
using BambaIba.Infrastructure.Repositories;
using BambaIba.Infrastructure.Repositories.Authentications;
using BambaIba.Infrastructure.Services;
using BambaIba.Infrastructure.Settings;
using BambaIba.Infrastructure.Time;
using BambaIba.SharedKernel;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Minio;
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
        .AddMinIo(configuration)
        .Authentication(configuration)
        .AddHealthChecks(configuration);

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

        string connectionString = configuration.GetConnectionString("DefaultConnection") ??
                    throw new ArgumentNullException(nameof(configuration));

        services.AddSingleton<IDbConnectionFactory>(_ =>
        new DbConnectionFactory(new NpgsqlDataSourceBuilder(connectionString).Build()));

        services.AddDbContext<BambaIbaDbContext>(
            options => options
                .UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default)
                    .EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null
        )).UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<BambaIbaDbContext>());

        services.AddScoped<ILiveStreamRepository, LiveStreamRepository>();
        services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
        //services.AddScoped<IAudioRepository, AudioRepository>();
        //services.AddScoped<IVideoRepository, VideoRepository>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IVideoQualityRepository, VideoQualityRepository>();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<IPlaylistRepository, PlaylistRepository>();
        services.AddScoped<IPlaylistItemRepository, PlaylistVideoRepository>();
        services.AddScoped<IMediaRepository, MediaRepository>();
        services.AddScoped<IMediaStorageService, MinIOMediaStorageService>();
        services.AddScoped<IMediaProcessingService, FFmpegMediaProcessingService>();

        services.AddHttpContextAccessor();
        services.AddHttpClient<IKeycloakAuthService, KeycloakAuthService>();

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

        return services;
    }

    private static IServiceCollection AddMinIo(this IServiceCollection services, IConfiguration configuration)
    {
        //// Add Minio using the default endpoint
        // 1. Enregistrer la configuration pour l'injection (IOptions)
        services.Configure<MinIOSettings>(
            configuration.GetSection(MinIOSettings.SectionName));

        services.AddMinio(client =>
        {
            MinIOSettings? settings = configuration
                .GetSection(MinIOSettings.SectionName)
                .Get<MinIOSettings>();

            //Console.WriteLine($"MinIO Config: {settings!.Endpoint}, Bucket: {settings.BucketName}");

            client.WithEndpoint(settings!.Endpoint)
                .WithCredentials(settings.AccessKey, settings.SecretKey)
                .WithSSL(settings.UseSSL)
                .Build();
        });

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

    //private static IServiceCollection AddAuthorization(this IServiceCollection services)
    //{
    //    services.AddAuthorization();

    //    //services.AddScoped<AuthorizationService>();

    //    //services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();

    //    //services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

    //    //services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    //    return services;
    //}

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!) // Use the correct connection string name
            .AddRedis(configuration.GetConnectionString("Redis")!);

        return services;
    }
}
