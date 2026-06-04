using System.Text;
using Aegitox.Bot.Core.Interfaces;
using Aegitox.Bot.Core.Models;

namespace Aegitox.Bot.Engine.Generators;

public sealed class PureBurstGenerator : IVoidGenerator
{
    private readonly VoidLexicon _lexicon;

    public PureBurstGenerator(VoidLexicon lexicon)
    {
        _lexicon = lexicon;
    }

    public void Generate(StringBuilder builder)
    {
        bool useProfanity = Random.Shared.Next(2) == 0;
        var sourceArray = useProfanity ? _lexicon.BaseProfanity : _lexicon.Resolutions;
        if (sourceArray.Length == 0)
            return;

        // 🚨 ENTROPY EXPLOSION: Combine 1 to 3 words.
        // Increases unique variations from ~500 to over 3,000,000.
        int wordCount = Random.Shared.Next(1, 4);

        for (int w = 0; w < wordCount; w++)
        {
            string word = sourceArray[Random.Shared.Next(sourceArray.Length)];
            double mutationRoll = Random.Shared.NextDouble();

            // 20% Vowel Drop
            if (mutationRoll < 0.20)
            {
                for (int i = 0; i < word.Length; i++)
                {
                    char c = word[i];
                    if (c is not ('a' or 'e' or 'i' or 'o' or 'u'))
                        builder.Append(c);
                }
            }
            // 5% QWERTY Fat-Finger
            else if (mutationRoll < 0.25)
            {
                builder.Append(word);
                if (builder.Length > 0)
                {
                    int lastIdx = builder.Length - 1;
                    builder[lastIdx] = GetAdjacentKey(builder[lastIdx]);
                }
            }
            // 60% Trailing Roll
            else if (mutationRoll < 0.85)
            {
                builder.Append(word);
                if (builder.Length > 0)
                {
                    char lastChar = builder[^1];
                    if (
                        lastChar
                        is 'a'
                            or 'e'
                            or 'i'
                            or 'o'
                            or 'u'
                            or 'k'
                            or 't'
                            or 's'
                            or 'f'
                            or 'h'
                    )
                    {
                        int rolls = Random.Shared.Next(1, 5);
                        builder.Append(lastChar, rolls);
                    }
                }
            }
            else // Baseline
            {
                builder.Append(word);
            }

            // Append space between words, but not after the final word
            if (w < wordCount - 1)
                builder.Append(' ');
        }
    }

    // O(1) constant time jump table mapping rightwards adjacency on a standard QWERTY
    private static char GetAdjacentKey(char c) =>
        c switch
        {
            'q' => 'w',
            'w' => 'e',
            'e' => 'r',
            'r' => 't',
            't' => 'y',
            'a' => 's',
            's' => 'd',
            'd' => 'f',
            'f' => 'g',
            'g' => 'h',
            'z' => 'x',
            'x' => 'c',
            'c' => 'v',
            'v' => 'b',
            'b' => 'n',
            _ => c, // Fallback to same character if not mapped
        };
}
