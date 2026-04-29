using CheckerboardGameApp.Factories;
using CheckerboardGameApp.Interfaces;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy => policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddSingleton<GameFactory>();
builder.Services.AddSingleton<IGameService>(sp =>
{
    var factory = sp.GetRequiredService<GameFactory>();
    return factory.CreateGame();
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("logs/game-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .WriteTo.File("logs/application-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1))
    .WriteTo.File(new JsonFormatter(), "logs/application-json-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30));


builder.Services.AddControllers();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
{
    options.DocumentPath = "/openapi/v1.json";
});
}

app.UseSerilogRequestLogging();
app.UseRouting();

app.MapHealthChecks("/");

app.MapControllers();

app.Run();
