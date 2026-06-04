using System.Text;
using Aegitox.Bot.Core.Interfaces;
using Aegitox.Bot.Core.Models;

namespace Aegitox.Bot.Engine.Generators;

public sealed class ExistentialDefeatGenerator : IVoidGenerator
{
    private readonly VoidLexicon _lexicon;
    private static readonly string[] Pauses = [" ", " ... ", " .. ", ". "];

    public ExistentialDefeatGenerator(VoidLexicon lexicon)
    {
        _lexicon = lexicon;
    }

    public void Generate(StringBuilder builder)
    {
        // The Equation: [Exclamation] + [Random Pause] + [Abstract Noun] + [Random Pause] + [Resolution]
        AppendWord(_lexicon.Exclamations, builder);
        AppendPause(builder);

        AppendWord(_lexicon.AbstractNouns, builder);
        AppendPause(builder);

        AppendWord(_lexicon.Resolutions, builder);
    }

    /// <summary>
    /// Injects a human-like pause between conceptual blocks.
    /// </summary>
    private static void AppendPause(StringBuilder builder)
    {
        builder.Append(Pauses[Random.Shared.Next(Pauses.Length)]);
    }

    /// <summary>
    /// Appends whole words directly to the builder in O(1) time.
    /// Replaces the old robotic character-spacing loop.
    /// </summary>
    private static void AppendWord(
        System.Collections.Immutable.ImmutableArray<string> pool,
        StringBuilder builder
    )
    {
        if (pool.Length == 0)
            return;
        builder.Append(pool[Random.Shared.Next(pool.Length)]);
    }
}
