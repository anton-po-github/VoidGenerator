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
    // 🚨 V3 PURE SEED WHITELIST (Expanded for Pure Burst Pillar)
    // 60+ elements. Zero targeted hate. Pure frustration and mechanical failure.
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
        "pain",
        "suffering",
        "agony",
        "cursed",
        "worthless",
        "awful",
        "terrible",
        "horrible",
        "disgusting",
        "miserable",
        "exhausting",
        "insanity",
        "numb",
        "empty",
        "void",
        "abyss",
        "ruined",
        "quit",
        "fade",
        "unplayable",
        "chalked",
        "ggs",
        "dead",
        "grief",
        "griefed",
        "doomed",
        "cursed",
        "unlucky",
        "tragic",
        "troll",
        "trolling",
        "despair"
    );

    // 🚨 V3 EXCLAMATIONS
    // 40+ elements.
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
        "good lord",
        "lord",
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

    // 🚨 V3 ABSTRACT NOUNS
    // 40+ elements. Replaces all specific targets with philosophical/broad concepts.
    private static readonly HashSet<string> _abstractNounAnchors = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        "everything",
        "it all",
        "existence",
        "life",
        "nothing",
        "reality",
        "the universe",
        "all of it",
        "every single thing",
        "the whole thing",
        "absolute bullshit",
        "the situation",
        "time",
        "humanity",
        "the world",
        "fate",
        "destiny",
        "the pain",
        "the suffering",
        "the misery",
        "the agony",
        "the void",
        "the abyss",
        "the illusion",
        "the cycle",
        "the loop",
        "the nonsense",
        "this outcome",
        "the logic",
        "the state of things",
        "the mechanics",
        "the simulation",
        "the matrix",
        "the tragedy",
        "the horror",
        "the nightmare",
        "the struggle",
        "the chaos",
    };

    // 🚨 V3 RESOLUTIONS
    // 40+ elements. The action of giving up entirely.
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
        "let it end",
        "make it end",
        "make it fade",
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
        "let it burn",
        "shut it down",
        "fade away",
        "checking out",
        "peace out",
        "walking away",
        "stepping away",
        "unplugging",
        "im gone",
        "log off",
        "logging out",
        "deleting",
        "uninstalling",
        "its over",
        "im retiring",
    };

    // 🚨 THE GLOBAL ENTITY FIREWALL
    // Exponentially expanded to encompass all Cat 1, Cat 2 (except I/Im), Cat 3 anchors and extreme hate.
    private static readonly HashSet<string> _bannedEntities = new(StringComparer.OrdinalIgnoreCase)
    {
        // Category 1 & 3 Original Anchors
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
        // Expanded Category 1: Interpersonal / Targets
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
        // Expanded Category 2: Self
        "my",
        "mine",
        "myself",
        // Expanded Category 3: Environment / Infrastructure
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
        // The Slur/Hate Pre-Filter (Zero-Tolerance Vaporization)
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
        // Implicit Violence, Directives & Hard Toxicity
        "kys",
        "kill",
        "die",
        "whore",
        "slut",
        "bitch",
        "cunt",
        "trash",
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

        // Pre-allocate the arrays at their required minimums for O(1) efficiency to avoid array resizing
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
            // If the CSV contains unmapped trash or slurs, they are structurally ignored.
            if (_exclamationAnchors.Contains(word))
                exclamationsBuilder.Add(word);
            else if (_abstractNounAnchors.Contains(word))
                abstractNounsBuilder.Add(word);
            else if (_resolutionAnchors.Contains(word))
                resolutionsBuilder.Add(word);
        }

        // Freeze collections instantly for downstream O(1) performance in the Generation Pillars
        return new VoidLexicon(
            BaseProfanity: _pureBaseProfanity,
            Exclamations: exclamationsBuilder.ToImmutable(),
            AbstractNouns: abstractNounsBuilder.ToImmutable(),
            Resolutions: resolutionsBuilder.ToImmutable(),
            ContrabandList: _bannedEntities.ToFrozenSet(StringComparer.OrdinalIgnoreCase)
        );
    }
}
