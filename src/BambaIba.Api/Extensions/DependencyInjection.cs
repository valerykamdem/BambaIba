
using BambaIba.Api.Helpers;
using BambaIba.Api.Infrastructure;
using BambaIba.Infrastructure.Settings;

namespace BambaIba.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services,
        IConfiguration configuration)
    {
        //services.AddEndpointsApiExplorer();
        //services.AddSwaggerGen();

        services.AddHttpContextAccessor();

        // REMARK: If you want to use Controllers, you'll need this.
        services.AddControllers();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        //services.AddApiVersioning(options =>
        //{
        //    options.DefaultApiVersion = new ApiVersion(1);
        //    options.ApiVersionReader = new UrlSegmentApiVersionReader();
        //}).AddApiExplorer(options =>
        //{
        //    options.GroupNameFormat = "'v'V";
        //    options.SubstituteApiVersionInUrl = true;
        //});

        //services.ConfigureOptions<ConfigureSwaggerGenOptions>();

        FFmpegSettings ffmpegSettings = configuration
   .GetSection(FFmpegSettings.SectionName)
   .Get<FFmpegSettings>();

        FFmpegConfigHelper.Normalize(ffmpegSettings);

        return services;
    }
}
