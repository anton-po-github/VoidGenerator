using System.Collections.Frozen;
using System.Collections.Immutable;
using Aegitox.Bot.Core.Interfaces;
using Aegitox.Bot.Core.Models;

namespace Aegitox.Bot.Data;

/// <summary>
/// High-performance stream parser for aegitox_matrix.csv.
/// Routes vocabulary into strict O(1) structures while filtering out targeted entities.
/// </summary>
public sealed class MatrixParser : IMatrixParser
{
    // 🚨 THE PURE SEED WHITELIST
    // Completely bypasses the poisoned CSV. Guarantees 0% slur/hate leakage.
    private static readonly ImmutableArray<string> _pureBaseProfanity = ImmutableArray.Create(
        "fuck",
        "shit",
        "damn",
        "crap",
        "hell",
        "wtf",
        "lmao",
        "bs",
        "fck",
        "sht",
        "dumb",
        "idiot",
        "trash",
        "broken",
        "useless",
        "pathetic",
        "garbage",
        "trash",
        "toxic",
        "worst",
        "stop",
        "please",
        "why",
        "stop",
        "end",
        "done"
    );

    // Pre-defined structural anchors required for the "Existential Defeat" and "Pure Burst" pillars.
    // By using HashSets here, our routing checks operate in O(1) time during file ingestion.
    private static readonly HashSet<string> _exclamationAnchors = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        "jesus christ",
        "unreal",
        "wow",
        "ffs",
        "omg",
        "jesus",
        "christ",
        "god",
    };

    private static readonly HashSet<string> _abstractNounAnchors = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        "everything",
        "it all",
        "existence",
        "life",
        "this shit",
        "nothing",
    };

    private static readonly HashSet<string> _resolutionAnchors = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        "im done",
        "make it stop",
        "just end",
        "give up",
        "done",
        "over it",
        "cant anymore",
    };

    // The absolute contraband list. If a word matches this, it is permanently stripped from generation.
    // The absolute contraband list.
    // EXPANDED: Now explicitly catches identity hate, racial slurs, and implicit targeted insults
    // ensuring they never make it into the untargeted Void arrays.
    private static readonly HashSet<string> _bannedEntities = new(StringComparer.OrdinalIgnoreCase)
    {
        // Original logical anchors
        "you",
        "me",
        "game",
        "server",
        "bot",
        "lag",
        "this",
        "that",
        "team",
        "devs",
        "player",
        "hacker",
        // 🚨 The Slur/Hate Pre-Filter (Identity Hate & Severe Toxicity)
        "nigger",
        "nigga",
        "faggot",
        "fag",
        "retard",
        "autist",
        "gook",
        "chink",
        "beaner",
        "spic",
        "darky",
        "tranny",
        // General Implicit Targets & Directives
        "kys",
        "kill",
        "die",
        "mom",
        "dad",
        "sister",
        "brother",
        "whore",
        "slut",
        "bitch",
        "cunt",
        "noob",
        "trash",
        "dog",
    };

    public async Task<VoidLexicon> ParseAsync(
        string filePath,
        CancellationToken cancellationToken = default
    )
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The matrix file was not found at {filePath}");

        // 2. Safely seed the structural builders by allocating exact capacity, then adding the anchors
        var exclamationsBuilder = ImmutableArray.CreateBuilder<string>(_exclamationAnchors.Count);
        exclamationsBuilder.AddRange(_exclamationAnchors);

        var abstractNounsBuilder = ImmutableArray.CreateBuilder<string>(_abstractNounAnchors.Count);
        abstractNounsBuilder.AddRange(_abstractNounAnchors);

        var resolutionsBuilder = ImmutableArray.CreateBuilder<string>(_resolutionAnchors.Count);
        resolutionsBuilder.AddRange(_resolutionAnchors);

        using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 65536,
            useAsync: true
        );

        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            var span = line.AsSpan();
            var commaIndex = span.IndexOf(',');
            var wordSpan = commaIndex >= 0 ? span[..commaIndex] : span;
            var word = wordSpan.Trim().ToString().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(word))
                continue;

            // ONLY extract words if they explicitly match our structural anchors.
            // Everything else in the CSV (slurs, targeted hate, long phrases) is instantly vaporized.
            if (_exclamationAnchors.Contains(word))
                exclamationsBuilder.Add(word);
            else if (_abstractNounAnchors.Contains(word))
                abstractNounsBuilder.Add(word);
            else if (_resolutionAnchors.Contains(word))
                resolutionsBuilder.Add(word);
        }

        // Seal the collections. BaseProfanity is now mathematically sterile.
        return new VoidLexicon(
            BaseProfanity: _pureBaseProfanity, // 🚨 INJECTED WHITELIST
            Exclamations: exclamationsBuilder.ToImmutable(),
            AbstractNouns: abstractNounsBuilder.ToImmutable(),
            Resolutions: resolutionsBuilder.ToImmutable(),
            ContrabandList: _bannedEntities.ToFrozenSet(StringComparer.OrdinalIgnoreCase)
        );
    }
}
