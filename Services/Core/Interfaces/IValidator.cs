namespace Aegitox.Bot.Core.Interfaces;

/// <summary>
/// The Reverse-Validation Trapdoor contract.
/// </summary>
public interface IValidator
{
    /// <summary>
    /// Scans the generated span in O(1) relative time to ensure zero entity leakage.
    /// Uses ReadOnlySpan to avoid string allocation during validation.
    /// </summary>
    bool IsValid(ReadOnlySpan<char> generatedText);
}
