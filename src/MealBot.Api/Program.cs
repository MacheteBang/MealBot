using MealBot.Api.Auth;
using Serilog.Core;

Log.Logger = CreateLoggerConfiguration();

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration.AddJsonFile("appsettings.Secrets.json", true);

    // Add services to the container.
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddPresentation();
    builder.Services.AddDatabaseProvider(builder.Configuration);
    builder.Services.AddMeals();
    builder.Services.AddAuth(builder.Configuration);
    builder.Services.AddCors(options =>
    {
        // TODO: Remove the hard-coded origins from CORS and replace with a configuration
        options.AddDefaultPolicy(builder => builder
            .WithOrigins("https://localhost:7188", "http://localhost:5103")
            .AllowCredentials()
            .AllowAnyMethod()
            .AllowAnyHeader());
    });

    builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
    {
        options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    app.UseSerilogRequestLogging();
    app.UseCors();
    app.UsePresentation();
    app.MapMealBotEndpoints();
}

app.Run();


static Logger CreateLoggerConfiguration()
{
    return new LoggerConfiguration()
        .WriteTo.Console()
        .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Http", LogEventLevel.Warning)
        .CreateLogger();
}