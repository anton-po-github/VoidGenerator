//Program.cs
using Aegitox.Bot.Core.Interfaces;
using Aegitox.Bot.Data;
using Aegitox.Bot.Data.Generators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IMatrixParser, MatrixParser>();

// ==========================================================
// 🚀 TIER 1 MICRO-BURST GENERATOR (Pre-Flight Generation)
// ==========================================================
//const string matrixPath = "aegitox_matrix.csv";
//const string tier1OutputPath = "Tier1_MicroBurst_10k.csv";

//builder.Services.AddTransient<VoidDatasetOrchestrator>();
//builder.Services.AddTransient<Tier1MicroBurstOrchestrator>();
builder.Services.AddTransient<TIER_1_NUC_6k_Generator>();
builder.Services.AddTransient<TIER_2_GREEN_10k_Generator>();
builder.Services.AddTransient<TIER_4_GREEN_6k_Generator>();

var app = builder.Build();

// ==========================================================
// TIER 4 MECHANICAL POSITIVITY INTEGRATION (Dual File Output)
// ==========================================================
const string tier4GamingOutputPath = "TIER_4_GREEN_GAME_3k.csv";
const string tier4ProfOutputPath = "TIER_4_GREEN_SYSTEM_3k.csv";

if (!File.Exists(tier4GamingOutputPath) || !File.Exists(tier4ProfOutputPath))
{
    Console.WriteLine(
        "⚠️ Tier 4 Mechanical Positivity artifacts missing. Initiating Dual-Channel O(1) Tensor Generation..."
    );

    using var scope = app.Services.CreateScope();
    var generator = scope.ServiceProvider.GetRequiredService<TIER_4_GREEN_6k_Generator>();

    // Dedicated channels to isolate Large Object Heap (LOH) pressure
    var gamingChannel = DatasetWriter.CreatePipelineChannel();
    var profChannel = DatasetWriter.CreatePipelineChannel();

    try
    {
        using var cts = new CancellationTokenSource();

        var generationTask = generator.GenerateAsync(
            gamingChannel.Writer,
            profChannel.Writer,
            cts.Token
        );

        var gamingIoTask = Task.Run(async () =>
        {
            await using var fs = new FileStream(
                tier4GamingOutputPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                65536,
                FileOptions.Asynchronous
            );
            await using var writer = new StreamWriter(fs);
            await foreach (var row in gamingChannel.Reader.ReadAllAsync(cts.Token))
            {
                await writer.WriteLineAsync(row);
            }
        });

        var profIoTask = Task.Run(async () =>
        {
            await using var fs = new FileStream(
                tier4ProfOutputPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                65536,
                FileOptions.Asynchronous
            );
            await using var writer = new StreamWriter(fs);
            await foreach (var row in profChannel.Reader.ReadAllAsync(cts.Token))
            {
                await writer.WriteLineAsync(row);
            }
        });

        await generationTask;

        gamingChannel.Writer.Complete();
        profChannel.Writer.Complete();

        await Task.WhenAll(gamingIoTask, profIoTask);

        Console.WriteLine(
            "✅ Tier 4 Dual-Channel Positivity Matrices successfully compiled and secured."
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ CRITICAL TIER 4 COMPILATION FAILURE: {ex.Message}");
        throw;
    }
}

const string gamingOutputPath = "TIER_1_NUC_GAME_3k.csv";
const string universalOutputPath = "TIER_1_NUC_SYSTEM_3k.csv";

// Tier 2 File Destinations
const string tier2GamingOutputPath = "TIER_2_GREEN_GAME_5k.csv";
const string tier2ProfOutputPath = "TIER_2_GREEN_SYSTEM_5k.csv";

if (!File.Exists(tier2GamingOutputPath) || !File.Exists(tier2ProfOutputPath))
{
    Console.WriteLine(
        "⚠️ Tier 2 Logistical Micro-Burst artifacts missing. Initiating Dual-Channel Benign Generation..."
    );

    using var scope = app.Services.CreateScope();
    var generator = scope.ServiceProvider.GetRequiredService<TIER_2_GREEN_10k_Generator>();

    var gamingChannel = DatasetWriter.CreatePipelineChannel();
    var profChannel = DatasetWriter.CreatePipelineChannel();

    try
    {
        using var cts = new CancellationTokenSource();

        var generationTask = generator.GenerateAsync(
            gamingChannel.Writer,
            profChannel.Writer,
            cts.Token
        );

        var gamingIoTask = Task.Run(async () =>
        {
            await using var fs = new FileStream(
                tier2GamingOutputPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                65536,
                FileOptions.Asynchronous
            );
            await using var writer = new StreamWriter(fs);
            await foreach (var row in gamingChannel.Reader.ReadAllAsync(cts.Token))
            {
                await writer.WriteLineAsync(row);
            }
        });

        var profIoTask = Task.Run(async () =>
        {
            await using var fs = new FileStream(
                tier2ProfOutputPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                65536,
                FileOptions.Asynchronous
            );
            await using var writer = new StreamWriter(fs);
            await foreach (var row in profChannel.Reader.ReadAllAsync(cts.Token))
            {
                await writer.WriteLineAsync(row);
            }
        });

        await generationTask;

        gamingChannel.Writer.Complete();
        profChannel.Writer.Complete();

        await Task.WhenAll(gamingIoTask, profIoTask);

        Console.WriteLine(
            "✅ Tier 2 Dual-Channel Logistical Matrices successfully compiled and secured."
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ CRITICAL TIER 2 COMPILATION FAILURE: {ex.Message}");
        throw;
    }
}

// ==========================================================
// TIER 1 MICRO-BURST PIPELINE INTEGRATION (Dual File Output)
// ==========================================================
// Location: Program.cs (Inside the Pre-Flight Generation Block)

if (!File.Exists(gamingOutputPath) || !File.Exists(universalOutputPath))
{
    Console.WriteLine(
        "⚠️ Tier 1 Micro-Burst artifacts missing. Initiating Dual-Channel NASA-grade synthetic generation..."
    );

    using var scope = app.Services.CreateScope();
    var generator = scope.ServiceProvider.GetRequiredService<TIER_1_NUC_6k_Generator>();

    // Dedicated channels isolate LOH pressure and decouple I/O bottlenecks
    var gamingChannel = DatasetWriter.CreatePipelineChannel();
    var universalChannel = DatasetWriter.CreatePipelineChannel();

    try
    {
        using var cts = new CancellationTokenSource();

        var generationTask = generator.GenerateAsync(
            gamingChannel.Writer,
            universalChannel.Writer,
            cts.Token
        );

        var gamingIoTask = Task.Run(async () =>
        {
            await using var fs = new FileStream(
                gamingOutputPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                65536,
                FileOptions.Asynchronous
            );
            await using var writer = new StreamWriter(fs);
            await foreach (var row in gamingChannel.Reader.ReadAllAsync(cts.Token))
            {
                await writer.WriteLineAsync(row);
            }
        });

        var universalIoTask = Task.Run(async () =>
        {
            await using var fs = new FileStream(
                universalOutputPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                65536,
                FileOptions.Asynchronous
            );
            await using var writer = new StreamWriter(fs);
            await foreach (var row in universalChannel.Reader.ReadAllAsync(cts.Token))
            {
                await writer.WriteLineAsync(row);
            }
        });

        await generationTask;

        gamingChannel.Writer.Complete();
        universalChannel.Writer.Complete();

        await Task.WhenAll(gamingIoTask, universalIoTask);

        Console.WriteLine(
            "✅ Tier 1 Dual-Channel Micro-Burst Matrices successfully compiled and secured."
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ CRITICAL MICRO-BURST COMPILATION FAILURE: {ex.Message}");
        throw;
    }
}

/* if (!File.Exists(tier1OutputPath))
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
} */

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

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
