using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Aegitox.Bot.Core.Models;

/// <summary>
/// The immutable, in-memory representation of the parsed matrix.
/// Designed for O(1) random access (ImmutableArray) and O(1) lookups (FrozenSet).
/// </summary>
public sealed record VoidLexicon(
    ImmutableArray<string> BaseProfanity,
    ImmutableArray<string> Exclamations,
    ImmutableArray<string> AbstractNouns,
    ImmutableArray<string> Resolutions,
    FrozenSet<string> ContrabandList
);
