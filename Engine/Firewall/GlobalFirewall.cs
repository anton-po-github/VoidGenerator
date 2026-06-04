using System.Buffers;
using Aegitox.Bot.Core.Interfaces;

namespace Aegitox.Bot.Engine.Firewall;

/// <summary>
/// The final gatekeeper. Sanitizes strings on-the-fly using stack memory
/// and verifies uniqueness in O(1) time with zero allocation overhead.
/// </summary>
public sealed class GlobalFirewall : IFirewall
{
    private readonly HashSet<string> _uniqueStrings;

    // .NET 9/10 feature: allows zero-allocation HashSet lookups using Spans
    private readonly HashSet<string>.AlternateLookup<ReadOnlySpan<char>> _lookup;

    public GlobalFirewall(int expectedCapacity = 30000)
    {
        // By using StringComparer.Ordinal, the HashSet natively supports AlternateLookup
        _uniqueStrings = new HashSet<string>(expectedCapacity, StringComparer.Ordinal);
        _lookup = _uniqueStrings.GetAlternateLookup<ReadOnlySpan<char>>();
    }

    public bool TryRegister(string generatedString)
    {
        // O(1) Memory Mandate: Allocate sanitization buffer on the Stack.
        // 256 chars is safely beyond the maximum length of a Void string.
        Span<char> sanitizedSpan = stackalloc char[256];
        int length = 0;

        // We MUST preserve punctuation and spaces! They are critical ML entropy.
        // We only standardize the casing to prevent identical visual duplicates.
        for (int i = 0; i < generatedString.Length; i++)
        {
            sanitizedSpan[length++] = char.ToLowerInvariant(generatedString[i]);
        }

        var finalSpan = sanitizedSpan[..length];

        // O(1) check using the span. If it exists, drop it before we allocate anything.
        if (_lookup.Contains(finalSpan))
        {
            return false;
        }

        // It is fully unique. Allocate the sanitized string to the HashSet to lock it out.
        // (Note: The Orchestrator will still write the original formatted string to disk).
        _lookup.Add(finalSpan);
        return true;
    }
}
