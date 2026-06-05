using Aegitox.Bot.Core.Interfaces;
using Aegitox.Bot.Data;
using Aegitox.Bot.Engine;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IMatrixParser, MatrixParser>();

//builder.Services.AddTransient<VoidDatasetOrchestrator>();
builder.Services.AddTransient<Tier1MicroBurstOrchestrator>();

// ==========================================================
// 🚀 TIER 1 MICRO-BURST GENERATOR (Pre-Flight Generation)
// ==========================================================
const string matrixPath = "aegitox_matrix.csv";
const string tier1OutputPath = "Tier1_MicroBurst_10k.csv";

var app = builder.Build();

if (!File.Exists(tier1OutputPath))
{
    Console.WriteLine(
        "⚠️ Tier 1 Micro-Burst artifact missing. Initiating NASA-grade synthetic generation..."
    );

    using var scope = app.Services.CreateScope();
    var orchestrator = scope.ServiceProvider.GetRequiredService<Tier1MicroBurstOrchestrator>();

    try
    {
        await orchestrator.GenerateDatasetAsync(matrixPath, tier1OutputPath, 10000);
        Console.WriteLine("✅ Tier 1 Micro-Burst Matrix successfully compiled and secured.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ CRITICAL VOID COMPILATION FAILURE: {ex.Message}");
        throw;
    }
}

/* builder.Services.AddTransient<VoidDatasetOrchestrator>();

const string voidMatrixPath = "aegitox_matrix.csv";
const string voidOutputPath = "Void_30k.csv";

var app = builder.Build();

if (!File.Exists(voidOutputPath))
{
    Console.WriteLine(
        "⚠️ Void Dataset artifact missing. Initiating NASA-grade synthetic generation..."
    );

    using var scope = app.Services.CreateScope();
    var orchestrator = scope.ServiceProvider.GetRequiredService<VoidDatasetOrchestrator>();

    try
    {
        await orchestrator.GenerateDatasetAsync(voidMatrixPath, voidOutputPath, 30000);
        Console.WriteLine("✅ Void Matrix successfully compiled and serialized to disk.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ CRITICAL VOID COMPILATION FAILURE: {ex.Message}");
        throw;
    }
} */

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching",
};

app.MapGet(
        "/weatherforecast",
        () =>
        {
            var forecast = Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        }
    )
    .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
