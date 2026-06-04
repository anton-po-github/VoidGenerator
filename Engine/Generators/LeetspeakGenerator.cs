using System.Text;
using Aegitox.Bot.Core.Interfaces;
using Aegitox.Bot.Core.Models;

namespace Aegitox.Bot.Engine.Generators;

public sealed class LeetspeakGenerator : IVoidGenerator
{
    private readonly VoidLexicon _lexicon;
    private static readonly char[] Separators = ['.', '*', '!'];

    public LeetspeakGenerator(VoidLexicon lexicon)
    {
        _lexicon = lexicon;
    }

    public void Generate(StringBuilder builder)
    {
        if (_lexicon.BaseProfanity.Length == 0)
            return;

        string word = string.Empty;

        // 1. O(1) Bounded Probe (Length Restriction)
        // We attempt to find a short word (< 8 chars) instantly.
        // Max 5 attempts guarantees constant time execution without O(N) array scanning.
        for (int i = 0; i < 5; i++)
        {
            var candidate = _lexicon.BaseProfanity[
                Random.Shared.Next(_lexicon.BaseProfanity.Length)
            ];
            if (candidate.Length < 8)
            {
                word = candidate;
                break;
            }
        }

        // Failsafe if the probe somehow missed 5 times
        if (string.IsNullOrEmpty(word))
            return;

        char separator = Separators[Random.Shared.Next(Separators.Length)];

        // 2. Eliminate Repetition & Apply Surgical Symbol Injection
        for (int i = 0; i < word.Length; i++)
        {
            char c = word[i];

            // 30% probability for surgical separator injection
            if (Random.Shared.NextDouble() < 0.3)
            {
                if (c is 'a' or 'e' or 'i' or 'o' or 'u')
                {
                    // Replaces the vowel entirely (e.g., f*ck)
                    builder.Append(separator);
                }
                else
                {
                    // Sits in the middle of consonants (e.g., f.uck)
                    builder.Append(c);
                    if (i < word.Length - 1)
                    {
                        builder.Append(separator);
                    }
                }
            }
            else
            {
                // Standard Leetspeak mapping (e.g., s -> $)
                builder.Append(ObfuscateChar(c));
            }
        }
    }

    // Mathematical constant-time symbol substitution
    private static char ObfuscateChar(char c) =>
        c switch
        {
            'a' => '@',
            's' => '$',
            'i' => '!',
            'e' => '3',
            'o' => '0',
            't' => '7',
            _ => c,
        };
}
