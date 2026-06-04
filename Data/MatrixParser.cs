using System.Collections.Frozen;
using System.Collections.Immutable;
using Aegitox.Bot.Core.Interfaces;
using Aegitox.Bot.Core.Models;

namespace Aegitox.Bot.Data;

/// <summary>
/// High-performance stream parser for aegitox_matrix.csv.
/// Routes vocabulary into strict O(1) structures while filtering out targeted entities.
/// Discord-Optimized for pure Category 4 (Untargeted/Void) vernacular.
/// </summary>
public sealed class MatrixParser : IMatrixParser
{
    // 🚨 V4 PURE SEED WHITELIST
    // Purged of "poetry" (void, abyss). Focused purely on raw Discord/Internet frustration.
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
        "broken",
        "useless",
        "pathetic",
        "garbage",
        "toxic",
        "worst",
        "stop",
        "please",
        "why",
        "end",
        "done",
        "bullshit",
        "dammit",
        "omfg",
        "bruh",
        "tilt",
        "tilted",
        "malding",
        "ridiculous",
        "joke",
        "nonsense",
        "pointless",
        "meaningless",
        "wack",
        "awful",
        "terrible",
        "horrible",
        "disgusting",
        "miserable",
        "exhausting",
        "chalked",
        "ggs",
        "dead",
        "cooked",
        "washed",
        "unlucky",
        "tragic",
        "troll",
        "trolling",
        "wild",
        "crazy",
        "insane",
        "trash",
        "braindead"
    );

    // 🚨 V4 EXCLAMATIONS (The Preamble)
    // How a Discord user starts an untargeted rant.
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
        "holy shit",
        "holy fuck",
        "bro",
        "dude",
        "man",
        "seriously",
        "literally",
        "actually",
        "honestly",
        "bruh",
        "come on",
        "what",
        "how",
        "whyyyy",
        "omfg",
        "fucking hell",
        "dear god",
        "alright",
        "okay",
        "fine",
        "nah",
        "no way",
        "insane",
        "crazy",
        "wild",
        "unbelievable",
        "absolute joke",
        "pure comedy",
        "yeah right",
        "whatever",
    };

    // 🚨 V4 ABSTRACT NOUNS (The Subject)
    // REPLACED all philosophical words (universe, destiny) with broad, untargeted descriptors.
    // NOTE: We avoid the word "this" to prevent triggering the Cat 3 Trapdoor.
    private static readonly HashSet<string> _abstractNounAnchors = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        "everything",
        "it all",
        "nothing",
        "all of it",
        "every single thing",
        "the whole thing",
        "absolute bullshit",
        "the situation",
        "the nonsense",
        "the state of things",
        "the chaos",
        "absolute garbage",
        "pure garbage",
        "total crap",
        "complete joke",
        "the logic",
        "the outcome",
        "the whole mess",
    };

    // 🚨 V4 RESOLUTIONS (The Action)
    // How a Discord user rage-quits or expresses complete defeat.
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
        "i give up",
        "just stop",
        "cant take it",
        "had enough",
        "no more",
        "im out",
        "im leaving",
        "im quitting",
        "over",
        "finished",
        "washed",
        "cooked",
        "done for",
        "ggs",
        "gg",
        "wrap it up",
        "pull the plug",
        "end it all",
        "shut it down",
        "checking out",
        "peace out",
        "walking away",
        "stepping away",
        "im gone",
        "log off",
        "logging out",
        "deleting",
        "uninstalling",
        "its over",
        "im retiring",
        "chalked",
        "gg next",
    };

    // 🚨 THE GLOBAL ENTITY FIREWALL (The Trapdoor)
    // EXPANDED to include general Discord/App infrastructure to protect the Void bucket.
    private static readonly HashSet<string> _bannedEntities = new(StringComparer.OrdinalIgnoreCase)
    {
        // Category 1: Interpersonal (Others)
        "you",
        "your",
        "ur",
        "he",
        "she",
        "they",
        "him",
        "her",
        "them",
        "teammate",
        "enemy",
        "opponent",
        "guy",
        "kid",
        "noob",
        "idiot",
        "moron",
        "clown",
        "smurf",
        "cheater",
        "report",
        "kick",
        "ban",
        "mom",
        "dad",
        "sister",
        "brother",
        "family",
        "wife",
        "gf",
        "boyfriend",
        "team",
        "player",
        // Category 2: Introspection (Self)
        "my",
        "mine",
        "myself",
        "me", // Note: "im" is permitted for "im done"
        // Category 3: Environmental / Infrastructure (Discord & Game Mix)
        "game",
        "server",
        "bot",
        "lag",
        "this",
        "that",
        "devs",
        "ping",
        "fps",
        "drop",
        "stutter",
        "connection",
        "wifi",
        "internet",
        "patch",
        "update",
        "company",
        "studio",
        "bug",
        "glitch",
        "crash",
        "client",
        "hitreg",
        "tickrate",
        "netcode",
        "match",
        "round",
        "map",
        "software",
        "hardware",
        "pc",
        "console",
        "controller",
        "mouse",
        "keyboard",
        "discord",
        "vc",
        "voice",
        "mic",
        "mod",
        "admin",
        "chat",
        "channel",
        "stream",
        "screen",
        // Zero-Tolerance Identity Hate / Slurs
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
        "kike",
        "dyke",
        "coon",
        "troon",
        // Directives & Violence
        "kys",
        "kill",
        "die",
        "whore",
        "slut",
        "bitch",
        "cunt",
        "dog",
        "rape",
        "rapist",
        "pedophile",
        "pedo",
        "nazi",
        "hitler",
        "terrorist",
        "slave",
        "groomer",
    };

    public async Task<VoidLexicon> ParseAsync(
        string filePath,
        CancellationToken cancellationToken = default
    )
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The matrix file was not found at {filePath}");

        // Pre-allocate the arrays at their required minimums for O(1) efficiency
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

            // O(1) hash lookups determine routing.
            if (_exclamationAnchors.Contains(word))
                exclamationsBuilder.Add(word);
            else if (_abstractNounAnchors.Contains(word))
                abstractNounsBuilder.Add(word);
            else if (_resolutionAnchors.Contains(word))
                resolutionsBuilder.Add(word);
        }

        // Freeze collections instantly for downstream O(1) performance
        return new VoidLexicon(
            BaseProfanity: _pureBaseProfanity,
            Exclamations: exclamationsBuilder.ToImmutable(),
            AbstractNouns: abstractNounsBuilder.ToImmutable(),
            Resolutions: resolutionsBuilder.ToImmutable(),
            ContrabandList: _bannedEntities.ToFrozenSet(StringComparer.OrdinalIgnoreCase)
        );
    }
}
