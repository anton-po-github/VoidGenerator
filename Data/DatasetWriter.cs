using System.Threading.Channels;

namespace Aegitox.Bot.Data;

/// <summary>
/// The high-performance I/O Consumer.
/// Prevents memory bloat by pulling from a Bounded Channel and streaming asynchronously to disk.
/// </summary>
public sealed class DatasetWriter
{
    /// <summary>
    /// Creates a backpressured channel. If the channel reaches 5,000 unwritten strings,
    /// the generators (producers) will be forced to wait, preventing OutOfMemory crashes.
    /// </summary>
    public static Channel<string> CreatePipelineChannel()
    {
        return Channel.CreateBounded<string>(
            new BoundedChannelOptions(capacity: 5000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true, // We have exactly one disk writer consuming
                SingleWriter = false, // We can have multiple generator threads producing
            }
        );
    }

    /// <summary>
    /// Consumes the channel indefinitely until the quota is met or the channel completes.
    /// </summary>
    public async Task ConsumeAndWriteAsync(
        ChannelReader<string> reader,
        string outputPath,
        int quota,
        CancellationToken cancellationToken = default
    )
    {
        // Allocate a massive 1MB buffer. This drastically reduces operating system
        // interrupts, writing massive chunks of the dataset in a single disk spin.
        using var stream = new FileStream(
            outputPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 1024 * 1024,
            useAsync: true
        );
        using var writer = new StreamWriter(stream);

        // 🚨 ML TENSOR HEADER
        await writer.WriteLineAsync("Content,IntentId,IntentCategory");

        int writtenCount = 0;

        // Asynchronously await data as it is pushed into the channel by the generators
        await foreach (var row in reader.ReadAllAsync(cancellationToken))
        {
            // ARCHITECTURAL MANDATE: Write sequentially to the stream buffer.
            // Avoids string interpolation ($"{row},4,Void") to eliminate GC allocations.
            await writer.WriteAsync(row);
            await writer.WriteLineAsync(",4,Void");

            writtenCount++;

            if (writtenCount >= quota)
            {
                break;
            }
        }
    }
}
