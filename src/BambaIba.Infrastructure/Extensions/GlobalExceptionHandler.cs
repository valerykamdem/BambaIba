using System.Text.Json;
using BambaIba.Application.Common.Dtos;
using BambaIba.SharedKernel.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace BambaIba.SharedKernel.Extensions;

public static class GlobalExceptionHandler
{
    public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                IExceptionHandlerFeature? errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (errorFeature is null)
                    return;

                Exception exception = errorFeature.Error;
                int code = 500;

                if (exception is AppException appEx)
                    code = appEx.StatusCode;

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = code;

                var response = new ApiResult<object>(
                    IsSuccess: false,
                    Data: null,
                    Errors: [exception.Message]);

                // Remplacer WriteAsJsonAsync par cette approche
                string json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });

                await context.Response.WriteAsync(json);
            });
        });
    }
}
