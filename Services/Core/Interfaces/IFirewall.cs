namespace Aegitox.Bot.Core.Interfaces;

/// <summary>
/// The Global Deduplication Firewall contract.
/// </summary>
public interface IFirewall
{
    /// <summary>
    /// Attempts to register a string. Returns false if it is a duplicate.
    /// Must execute in strict O(1) time.
    /// </summary>
    bool TryRegister(string generatedString);
}
