using Aegitox.Bot.Core.Interfaces;

namespace Aegitox.Bot.Engine.Validation;

/// <summary>
/// A NASA-level Reverse-Validation Trapdoor using an array-backed Aho-Corasick FSM.
/// Executes in strict O(M + Z) time with absolute zero memory allocation.
/// </summary>
public sealed class AhoCorasickValidator : IValidator
{
    private sealed class TrieNode
    {
        // 128 length covers all ASCII and leetspeak characters in O(1) memory offset
        public readonly TrieNode?[] Children = new TrieNode?[128];
        public TrieNode? FailLink;
        public TrieNode? OutputLink; // Direct pointer to the next valid match in the fail chain
        public bool IsContraband;
        public int Length;
    }

    private readonly TrieNode _root = new();

    public AhoCorasickValidator(ContrabandRegistry registry)
    {
        BuildTrie(registry.BannedEntities);
        BuildFailLinks();
    }

    private void BuildTrie(IEnumerable<string> bannedWords)
    {
        foreach (var word in bannedWords)
        {
            var current = _root;
            foreach (char c in word)
            {
                char lower = char.ToLowerInvariant(c);
                if (lower > 127)
                    continue; // Ensure within fast-ASCII bounds

                current.Children[lower] ??= new TrieNode();
                current = current.Children[lower]!;
            }
            current.IsContraband = true;
            current.Length = word.Length;
        }
    }

    private void BuildFailLinks()
    {
        var queue = new Queue<TrieNode>();

        // Initialize root children's fail links to root
        for (int i = 0; i < 128; i++)
        {
            var child = _root.Children[i];
            if (child != null)
            {
                child.FailLink = _root;
                queue.Enqueue(child);
            }
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            for (int i = 0; i < 128; i++)
            {
                var child = current.Children[i];
                if (child == null)
                    continue;

                var fail = current.FailLink;
                while (fail != null && fail.Children[i] == null)
                {
                    fail = fail.FailLink;
                }

                child.FailLink = fail?.Children[i] ?? _root;

                // Precompute output links: skips empty fail links to instantly find overlapping matches
                child.OutputLink = child.FailLink.IsContraband
                    ? child.FailLink
                    : child.FailLink.OutputLink;

                queue.Enqueue(child);
            }
        }
    }

    public bool IsValid(ReadOnlySpan<char> generatedText)
    {
        var current = _root;

        for (int i = 0; i < generatedText.Length; i++)
        {
            char c = char.ToLowerInvariant(generatedText[i]);

            // If the generator mutates a non-ASCII character, gracefully reset the state machine
            if (c > 127)
            {
                current = _root;
                continue;
            }

            // Traverse Fail Links
            while (current != _root && current.Children[c] == null)
            {
                current = current.FailLink!;
            }

            current = current.Children[c] ?? _root;

            // Evaluate Matches (including overlapping subsets via OutputLink)
            var matchNode = current.IsContraband ? current : current.OutputLink;

            while (matchNode != null)
            {
                // Boundary Enforcement: Prevents "game" from flagging inside "gamer"
                int matchStart = i - matchNode.Length + 1;

                bool leftBoundary =
                    matchStart == 0 || !char.IsLetterOrDigit(generatedText[matchStart - 1]);
                bool rightBoundary =
                    i == generatedText.Length - 1 || !char.IsLetterOrDigit(generatedText[i + 1]);

                if (leftBoundary && rightBoundary)
                {
                    // Trapdoor triggered. Entity leaked. Instant vaporization.
                    return false;
                }

                matchNode = matchNode.OutputLink;
            }
        }

        // Clean, untargeted Void.
        return true;
    }
}
