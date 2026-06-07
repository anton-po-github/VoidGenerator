using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Aegitox.Bot.Data.Generators;

/// <summary>
/// Tier 1: Multi-Vertical Micro-Burst Generator for all-MiniLM-L6-v2 ONNX fine-tuning.
/// Generates exactly 6,000 strings (1-3 words) to solidify extreme boundaries of the latent space.
/// Architected for .NET 10: Zero-allocation permutation paths, strict O(1) complexity per row,
/// and direct integration with the existing Bounded Channel stream.
/// </summary>
// ==========================================================
// EXPANDED 8-VARIABLE TENSOR MATRICES (1-3 WORD BOUNDARIES)
// ==========================================================

public sealed class TIER_1_NUC_6k_Generator
{
    // ---------------------------------------------------------
    // CATEGORY 1: GAMING HOSTILITY (Discord/Lobby Specific)
    // ---------------------------------------------------------
    private static readonly ImmutableArray<string> _gamingPrefixes = ImmutableArray.Create(
        "actual",
        "literal",
        "fucking",
        "absolute",
        "complete",
        "utter",
        "pure",
        "holy",
        "massive",
        "toxic",
        "genuinely"
    );

    private static readonly ImmutableArray<string> _gamingDirectives = ImmutableArray.Create(
        "stfu",
        "uninstall",
        "kys",
        "quit",
        "dodge",
        "ff",
        "altf4",
        "delete",
        "leave",
        "logoff",
        "neck",
        "rope"
    );

    private static readonly ImmutableArray<string> _gamingAdjectives = ImmutableArray.Create(
        "braindead",
        "dogshit",
        "useless",
        "boosted",
        "delusional",
        "griefing",
        "iron",
        "bronze",
        "hardstuck",
        "clueless",
        "trolling",
        "carried",
        "sweaty",
        "cringe",
        "washed",
        "unranked",
        "helpless"
    );

    private static readonly ImmutableArray<string> _gamingTargets = ImmutableArray.Create(
        "bot",
        "kid",
        "trash",
        "npc",
        "random",
        "thrower",
        "griefer",
        "noob",
        "feeder",
        "smurf",
        "animal",
        "scrub",
        "inter",
        "shitter",
        "tourist",
        "casual"
    );

    private static readonly ImmutableArray<string> _universalPrefixes = ImmutableArray.Create(
        "fucking",
        "pure",
        "absolute",
        "literal",
        "sick",
        "disgusting",
        "vile",
        "twisted",
        "insane",
        "bloody",
        "truly"
    );

    private static readonly ImmutableArray<string> _universalDirectives = ImmutableArray.Create(
        "die",
        "choke",
        "leave",
        "perish",
        "suffer",
        "burn",
        "jump",
        "hang",
        "drown",
        "bleed",
        "rot",
        "croak",
        "vanish"
    );

    private static readonly ImmutableArray<string> _universalAdjectives = ImmutableArray.Create(
        "worthless",
        "subhuman",
        "pathetic",
        "miserable",
        "repulsive",
        "degenerate",
        "brainless",
        "retarded",
        "autistic",
        "ugly",
        "fat",
        "inbred",
        "psychotic",
        "schizo",
        "pointless",
        "deranged",
        "hideous"
    );

    private static readonly ImmutableArray<string> _universalTargets = ImmutableArray.Create(
        "scum",
        "freak",
        "retard",
        "garbage",
        "waste",
        "mistake",
        "rat",
        "pig",
        "parasite",
        "disease",
        "whore",
        "slut",
        "cancer",
        "faggot",
        "bitch",
        "cunt",
        "bastard",
        "tumor",
        "mutt"
    );

    // O(1) Pre-allocated fast-path lookups for scores and categories.
    // Prevents float.ToString() allocations in the inner loop.
    private static readonly ImmutableArray<string> _scores = ImmutableArray.Create(
        "0.97",
        "0.98",
        "0.99"
    );
    private const string GamingCategory = "Nuclear";
    private const string UniversalCategory = "Nuclear";

    /// <summary>
    /// Executes the bifurcated generation loop targeting two discrete data channels.
    /// </summary>
    public async Task GenerateAsync(
        ChannelWriter<string> gamingWriter,
        ChannelWriter<string> universalWriter,
        CancellationToken cancellationToken = default
    )
    {
        var task1 = StreamDomainAsync(
            gamingWriter,
            _gamingPrefixes,
            _gamingDirectives,
            _gamingAdjectives,
            _gamingTargets,
            GamingCategory,
            3000,
            cancellationToken
        );
        var task2 = StreamDomainAsync(
            universalWriter,
            _universalPrefixes,
            _universalDirectives,
            _universalAdjectives,
            _universalTargets,
            UniversalCategory,
            3000,
            cancellationToken
        );

        await Task.WhenAll(task1, task2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task StreamDomainAsync(
        ChannelWriter<string> writer,
        ImmutableArray<string> prefixes,
        ImmutableArray<string> directives,
        ImmutableArray<string> adjectives,
        ImmutableArray<string> targets,
        string category,
        int quota,
        CancellationToken cancellationToken
    )
    {
        // 1. Calculate O(1) Combinatorial Cartesian Bounds
        int pLen = prefixes.Length;
        int dLen = directives.Length;
        int aLen = adjectives.Length;
        int tLen = targets.Length;

        int countA = dLen * tLen; // Path A: Directive + Target
        int countB = aLen * tLen; // Path B: Adjective + Target
        int countC = pLen * aLen * tLen; // Path C: Prefix + Adjective + Target
        int countD = dLen * pLen * tLen; // Path D: Directive + Prefix + Target

        // Gaming Capacity: 5,568 | Universal Capacity: 6,840
        int totalCombinations = countA + countB + countC + countD;

        // 2. Map the entire combinatorial space to a flat 1D integer array
        int[] indices = new int[totalCombinations];
        for (int i = 0; i < totalCombinations; i++)
        {
            indices[i] = i;
        }

        // 3. Partial Fisher-Yates Shuffle (The Tier 2 "Magic Formula")
        // Shuffles exactly 'quota' distinct elements to the front.
        // Never wastes cycles materializing the unneeded remainder.
        var rng = Random.Shared;
        for (int i = 0; i < quota; i++)
        {
            int j = rng.Next(i, totalCombinations);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        // 4. Stream output using the exact unique combinations
        await writer.WriteAsync("Content,Score,Category", cancellationToken);

        for (int i = 0; i < quota; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int x = indices[i];
            string content;

            // 5. Bijective mapping: Decode the 1D index back into precise syntax paths
            if (x < countA)
            {
                content = string.Concat(directives[x / tLen], " ", targets[x % tLen]);
            }
            else if (x < countA + countB)
            {
                x -= countA;
                content = string.Concat(adjectives[x / tLen], " ", targets[x % tLen]);
            }
            else if (x < countA + countB + countC)
            {
                x -= (countA + countB);
                content = string.Concat(
                    prefixes[x / (aLen * tLen)],
                    " ",
                    adjectives[(x / tLen) % aLen],
                    " ",
                    targets[x % tLen]
                );
            }
            else
            {
                x -= (countA + countB + countC);
                content = string.Concat(
                    directives[x / (pLen * tLen)],
                    " ",
                    prefixes[(x / tLen) % pLen],
                    " ",
                    targets[x % tLen]
                );
            }

            // Pseudo-random assignment of the benign/nuclear score bounds
            string score = _scores[rng.Next(_scores.Length)];

            // Bypass params string[] allocation; pure contiguous string memory
            string row = string.Concat(content, ",", score, ",", category);

            await writer.WriteAsync(row, cancellationToken);
        }
    }
}
