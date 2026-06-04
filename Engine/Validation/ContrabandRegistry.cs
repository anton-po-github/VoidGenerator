using System.Collections.Frozen;

namespace Aegitox.Bot.Engine.Validation;

/// <summary>
/// Stores exact-match banned anchors (e.g., "you", "me", "game").
/// </summary>
public sealed class ContrabandRegistry
{
    public FrozenSet<string> BannedEntities { get; }

    public ContrabandRegistry(IEnumerable<string> bannedWords)
    {
        // FrozenSet provides the absolute fastest O(1) read operations in .NET.
        // Once created, it cannot be mutated, making it thread-safe and blisteringly fast.
        BannedEntities = bannedWords.ToFrozenSet();
    }
}
