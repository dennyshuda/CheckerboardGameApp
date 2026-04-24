using CheckerboardGameApp.Factories;
using CheckerboardGameApp.Interfaces;

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
app.UseRouting();

app.MapHealthChecks("/");

app.MapControllers();

app.Run();
