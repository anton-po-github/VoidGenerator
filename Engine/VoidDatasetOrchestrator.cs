using Aegitox.Bot.Core.Interfaces;
using Aegitox.Bot.Core.Memory;
using Aegitox.Bot.Data;
using Aegitox.Bot.Engine.Firewall;
using Aegitox.Bot.Engine.Generators;
using Aegitox.Bot.Engine.Validation;

namespace Aegitox.Bot.Engine;

/// <summary>
/// Ties the architectural pillars together to mass-produce the Void dataset.
/// </summary>
public sealed class VoidDatasetOrchestrator
{
    private readonly IMatrixParser _parser;

    public VoidDatasetOrchestrator(IMatrixParser parser)
    {
        _parser = parser;
    }

    public async Task GenerateDatasetAsync(string matrixFilePath, string outputFilePath, int quota)
    {
        Console.WriteLine(
            $"🚀 [VOID ENGINE] Ingesting {matrixFilePath} into O(1) memory structures..."
        );
        var lexicon = await _parser.ParseAsync(matrixFilePath);

        var validator = new AhoCorasickValidator(new ContrabandRegistry(lexicon.ContrabandList));
        var firewall = new GlobalFirewall(quota);
        var writer = new DatasetWriter();

        // Instantiate the 4 Pillars
        IVoidGenerator[] generators =
        [
            new PureBurstGenerator(lexicon),
            new ExistentialDefeatGenerator(lexicon),
            new KeyboardMashGenerator(),
            new LeetspeakGenerator(lexicon),
        ];

        var channel = DatasetWriter.CreatePipelineChannel();
        var consumerTask = writer.ConsumeAndWriteAsync(channel.Reader, outputFilePath, quota);

        Console.WriteLine($"🔥 [VOID ENGINE] Generating {quota} pure entity-less rows...");

        int generated = 0;
        int consecutiveFailures = 0; // NASA-Level dead-man's switch for infinite loop protection

        while (generated < quota)
        {
            var builder = PoolProvider.StringBuilderPool.Get();
            try
            {
                // 🚨 PHASE 3: THE PSYCHOLOGICAL DISTRIBUTION ROUTER
                // O(1) mathematical routing based on real-world gamer tilt probabilities.
                double roll = Random.Shared.NextDouble();

                IVoidGenerator generator = roll switch
                {
                    < 0.60 => generators[0], // 60% Pure Burst (Index 0)
                    < 0.85 => generators[1], // 25% Existential Defeat (Index 1)
                    < 0.95 => generators[3], // 10% Leetspeak Breakdown (Index 3)
                    _ => generators[2], // 5% Keyboard Mash (Index 2)
                };

                generator.Generate(builder);

                // Materialize once for O(M+Z) validation and O(1) firewall checking
                string result = builder.ToString();
                var span = result.AsSpan();

                // 🚨 PHASE 4: THE STERILIZED EGRESS
                if (validator.IsValid(span) && firewall.TryRegister(result))
                {
                    // Asynchronously push to the backpressured channel.
                    // This does not block the thread; the DatasetWriter consumes it in parallel.
                    await channel.Writer.WriteAsync(result);
                    generated++;
                    consecutiveFailures = 0; // Reset failsafe on success
                }
                else
                {
                    // Dropped by Trapdoor (Contraband Leakage) OR Firewall (Duplicate)
                    consecutiveFailures++;

                    if (consecutiveFailures > 5000)
                    {
                        throw new InvalidOperationException(
                            "Catastrophic Generation Failure: The Firewall or Trapdoor has deadlocked the system. Check generator entropy."
                        );
                    }
                }
            }
            finally
            {
                // Absolute mandate: scrub the buffer and return it to prevent GC allocations
                builder.Clear();
                PoolProvider.StringBuilderPool.Return(builder);
            }
        }
        channel.Writer.Complete();
        await consumerTask;

        Console.WriteLine(
            $"✅ [VOID ENGINE] {quota} rows successfully compiled and serialized to {outputFilePath}."
        );
    }
}
