using BambaIba.Application.Extensions;
using BambaIba.Infrastructure.Extensions;
using BambaIba.SharedKernel.Extensions;
using Carter;
using FluentValidation;
using Scalar.AspNetCore;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

// Configurations
builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Services
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCarter();

WebApplication app = builder.Build();

//// Migration automatique de la base de données
//using (IServiceScope scope = app.Services.CreateScope())
//{
//    BambaIbaDbContext dbContext = scope.ServiceProvider.GetRequiredService<BambaIbaDbContext>();
//    await dbContext.Database.MigrateAsync();
//}

//// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.MapEndpoints();
app.MapCarter();
app.UseGlobalExceptionHandler();
app.UseSerilogRequestLogging();

////app.UseCors("AllowAngularOrigins");
//app.UseCors(options =>
//{
//    options.AllowAnyHeader();
//    options.AllowAnyMethod();
//    options.AllowAnyOrigin();
//});

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

//app.Use(async (context, next) =>
//{
//    using IServiceScope scope = context.RequestServices.CreateScope();
//    IServiceProvider services = scope.ServiceProvider;
//    IWebHostEnvironment env = services.GetRequiredService<IWebHostEnvironment>();

//    if (env.IsDevelopment())
//    {
//        BambaIbaDbContext db = services.GetRequiredService<BambaIbaDbContext>();
//        // Force la création des tables si elles n’existent pas encore
//        await db.Database.EnsureCreatedAsync();

//        db.Database.Migrate();
//        SeedData.Initialize(services);
//    }

//    await next();
//});

app.Run();
