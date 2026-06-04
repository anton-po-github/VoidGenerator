using System.Text;

namespace Aegitox.Bot.Core.Interfaces;

/// <summary>
/// Contract for the 4 Generation Pillars.
/// </summary>
public interface IVoidGenerator
{
    /// <summary>
    /// Populates a pooled StringBuilder instead of allocating a new string.
    /// This guarantees zero-allocation generation.
    /// </summary>
    void Generate(StringBuilder builder);
}
