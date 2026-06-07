using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Aegitox.Bot.Data.Generators;

/// <summary>
/// Tier 4: Balanced Mechanical Positivity & Courtesies (6,000 rows).
/// Establishes safe anchors for unpunctuated goodwill (1-4 words).
/// Uses O(1) Bijective Tensor Mapping to prevent all collisions without loops.
/// </summary>
public sealed class TIER_4_GREEN_6k_Generator
{
    // ==========================================================
    // GAMING SHORTHAND (Functional Acronyms)
    // ==========================================================

    // ==========================================================
    // GAMING SHORTHAND (Functional Acronyms) - 11,110 Combinations
    // ==========================================================

    private static readonly ImmutableArray<string> _gaming1 = ImmutableArray.Create(
        "gg",
        "ggs",
        "wp",
        "nt",
        "gl",
        "hf",
        "ty",
        "thx",
        "mb",
        "sry"
    );

    private static readonly ImmutableArray<string> _gaming2A = ImmutableArray.Create(
        "gg",
        "nt",
        "mb",
        "sry",
        "ty",
        "gl",
        "wp",
        "thx",
        "nice",
        "good"
    );
    private static readonly ImmutableArray<string> _gaming2B = ImmutableArray.Create(
        "team",
        "all",
        "guys",
        "bro",
        "man",
        "mate",
        "dude",
        "game",
        "play",
        "round"
    );

    private static readonly ImmutableArray<string> _gaming3A = ImmutableArray.Create(
        "gg",
        "nt",
        "sry",
        "mb",
        "ty",
        "gl",
        "thx",
        "wp",
        "nice",
        "good"
    );
    private static readonly ImmutableArray<string> _gaming3B = ImmutableArray.Create(
        "my",
        "our",
        "the",
        "for",
        "about",
        "team",
        "you",
        "all",
        "that",
        "this"
    );
    private static readonly ImmutableArray<string> _gaming3C = ImmutableArray.Create(
        "bad",
        "lag",
        "help",
        "game",
        "match",
        "try",
        "play",
        "carry",
        "save",
        "round"
    );

    private static readonly ImmutableArray<string> _gaming4A = ImmutableArray.Create(
        "gg",
        "nt",
        "sry",
        "mb",
        "ty",
        "gl",
        "thanks",
        "wow",
        "nice",
        "good"
    );
    private static readonly ImmutableArray<string> _gaming4B = ImmutableArray.Create(
        "team",
        "guys",
        "everyone",
        "for",
        "about",
        "really",
        "very",
        "much",
        "so",
        "super"
    );
    private static readonly ImmutableArray<string> _gaming4C = ImmutableArray.Create(
        "the",
        "that",
        "my",
        "your",
        "good",
        "well",
        "nice",
        "solid",
        "clean",
        "great"
    );
    private static readonly ImmutableArray<string> _gaming4D = ImmutableArray.Create(
        "played",
        "game",
        "match",
        "lag",
        "help",
        "try",
        "work",
        "round",
        "save",
        "carry"
    );

    // ==========================================================
    // GENERAL COURTESIES (Everyday / Non-Gaming) - 11,110 Combinations
    // ==========================================================

    private static readonly ImmutableArray<string> _gen1 = ImmutableArray.Create(
        "thanks",
        "ty",
        "appreciated",
        "cheers",
        "awesome",
        "perfect",
        "clean",
        "good",
        "solid",
        "nice"
    );

    private static readonly ImmutableArray<string> _gen2A = ImmutableArray.Create(
        "looks",
        "sounds",
        "seems",
        "very",
        "really",
        "much",
        "thanks",
        "good",
        "super",
        "pretty"
    );
    private static readonly ImmutableArray<string> _gen2B = ImmutableArray.Create(
        "clean",
        "good",
        "perfect",
        "solid",
        "appreciated",
        "man",
        "bro",
        "mate",
        "stuff",
        "work"
    );

    private static readonly ImmutableArray<string> _gen3A = ImmutableArray.Create(
        "thanks",
        "ty",
        "appreciate",
        "looks",
        "sounds",
        "really",
        "very",
        "much",
        "super",
        "pretty"
    );
    private static readonly ImmutableArray<string> _gen3B = ImmutableArray.Create(
        "for",
        "the",
        "your",
        "really",
        "very",
        "good",
        "nice",
        "solid",
        "clean",
        "great"
    );
    private static readonly ImmutableArray<string> _gen3C = ImmutableArray.Create(
        "help",
        "info",
        "work",
        "job",
        "update",
        "clean",
        "stuff",
        "time",
        "catch",
        "save"
    );

    private static readonly ImmutableArray<string> _gen4A = ImmutableArray.Create(
        "thanks",
        "ty",
        "appreciate",
        "looks",
        "sounds",
        "whoops",
        "sorry",
        "much",
        "really",
        "very"
    );
    private static readonly ImmutableArray<string> _gen4B = ImmutableArray.Create(
        "for",
        "about",
        "dropped",
        "really",
        "very",
        "the",
        "your",
        "my",
        "that",
        "this"
    );
    private static readonly ImmutableArray<string> _gen4C = ImmutableArray.Create(
        "the",
        "that",
        "my",
        "good",
        "nice",
        "clean",
        "solid",
        "quick",
        "great",
        "helpful"
    );
    private static readonly ImmutableArray<string> _gen4D = ImmutableArray.Create(
        "help",
        "pen",
        "work",
        "job",
        "update",
        "info",
        "fix",
        "time",
        "catch",
        "save"
    );

    // Fixed safe latency/benign scores for Green routing
    private static readonly ImmutableArray<string> _benignScores = ImmutableArray.Create(
        "0.11",
        "0.14",
        "0.15"
    );

    private const string GamingCategory = "Green";
    private const string ProfCategory = "Green";

    public async Task GenerateAsync(
        ChannelWriter<string> gamingWriter,
        ChannelWriter<string> profWriter,
        CancellationToken cancellationToken = default
    )
    {
        // Route 1: Gaming Functional Acronyms (3,000)
        var task1 = StreamCombinatorialPathsAsync(
            gamingWriter,
            new[] { _gaming1 },
            new[] { _gaming2A, _gaming2B },
            new[] { _gaming3A, _gaming3B, _gaming3C },
            new[] { _gaming4A, _gaming4B, _gaming4C, _gaming4D },
            GamingCategory,
            3000,
            cancellationToken
        );

        // Route 2: General/Everyday Courtesies (3,000)
        var task2 = StreamCombinatorialPathsAsync(
            profWriter,
            new[] { _gen1 },
            new[] { _gen2A, _gen2B },
            new[] { _gen3A, _gen3B, _gen3C },
            new[] { _gen4A, _gen4B, _gen4C, _gen4D },
            ProfCategory,
            3000,
            cancellationToken
        );

        await Task.WhenAll(task1, task2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task StreamCombinatorialPathsAsync(
        ChannelWriter<string> writer,
        ImmutableArray<string>[] path1,
        ImmutableArray<string>[] path2,
        ImmutableArray<string>[] path3,
        ImmutableArray<string>[] path4,
        string category,
        int quota,
        CancellationToken cancellationToken
    )
    {
        // 1. Calculate O(1) Tensor Constraints
        int c1 = path1[0].Length;
        int c2 = path2[0].Length * path2[1].Length;
        int c3 = path3[0].Length * path3[1].Length * path3[2].Length;
        int c4 = path4[0].Length * path4[1].Length * path4[2].Length * path4[3].Length;

        int totalCombinations = c1 + c2 + c3 + c4; // With 8-item arrays, this yields 4,682 combinations.

        if (quota > totalCombinations)
        {
            throw new InvalidOperationException(
                $"Quota ({quota}) exceeds total combinatorial space ({totalCombinations})."
            );
        }

        // 2. Map the multi-dimensional space to a flat 1D integer array
        int[] indices = new int[totalCombinations];
        for (int i = 0; i < totalCombinations; i++)
        {
            indices[i] = i;
        }

        // 3. Partial Fisher-Yates (Walks only 'quota' steps)
        var rng = Random.Shared;
        for (int i = 0; i < quota; i++)
        {
            int j = rng.Next(i, totalCombinations);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        await writer.WriteAsync("Content,Score,Category", cancellationToken);

        // 4. Bijective Decoding (O(1) Array resolution mapping)
        for (int i = 0; i < quota; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int x = indices[i];
            string content;

            if (x < c1)
            {
                // 1-Word Path
                content = path1[0][x];
            }
            else if (x < c1 + c2)
            {
                // 2-Word Path
                x -= c1;
                content = string.Concat(
                    path2[0][x / path2[1].Length],
                    " ",
                    path2[1][x % path2[1].Length]
                );
            }
            else if (x < c1 + c2 + c3)
            {
                // 3-Word Path
                x -= (c1 + c2);
                int aLen = path3[1].Length * path3[2].Length;
                int bLen = path3[2].Length;
                content = string.Concat(
                    path3[0][x / aLen],
                    " ",
                    path3[1][(x / bLen) % path3[1].Length],
                    " ",
                    path3[2][x % path3[2].Length]
                );
            }
            else
            {
                // 4-Word Path
                x -= (c1 + c2 + c3);
                int aLen = path4[1].Length * path4[2].Length * path4[3].Length;
                int bLen = path4[2].Length * path4[3].Length;
                int cLen = path4[3].Length;
                content = string.Concat(
                    path4[0][x / aLen],
                    " ",
                    path4[1][(x / bLen) % path4[1].Length],
                    " ",
                    path4[2][(x / cLen) % path4[2].Length],
                    " ",
                    path4[3][x % path4[3].Length]
                );
            }

            string score = _benignScores[rng.Next(_benignScores.Length)];
            await writer.WriteAsync(
                string.Concat(content, ",", score, ",", category),
                cancellationToken
            );
        }
    }
}
