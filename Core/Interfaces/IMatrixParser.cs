using Aegitox.Bot.Core.Models;

namespace Aegitox.Bot.Core.Interfaces;

/// <summary>
/// Defines the contract for surgical ingestion of the matrix dataset.
/// </summary>
public interface IMatrixParser
{
    /// <summary>
    /// Parses the CSV file asynchronously and builds the VoidLexicon.
    /// </summary>
    Task<VoidLexicon> ParseAsync(string filePath, CancellationToken cancellationToken = default);
}
