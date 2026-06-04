using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace Aegitox.Bot.Core.Memory;

/// <summary>
/// NASA-level memory management. Eliminates GC pauses by recycling StringBuilders
/// across the entire generator pipeline.
/// </summary>
public static class PoolProvider
{
    // Pre-allocates a pool of StringBuilders.
    // initialCapacity of 64 chars is optimal for Void strings (usually short bursts).
    public static readonly ObjectPool<StringBuilder> StringBuilderPool =
        new DefaultObjectPoolProvider().CreateStringBuilderPool(
            initialCapacity: 64,
            maximumRetainedCapacity: 256
        );
}
