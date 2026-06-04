using System.Text;
using Aegitox.Bot.Core.Interfaces;

namespace Aegitox.Bot.Engine.Generators;

public sealed class KeyboardMashGenerator : IVoidGenerator
{
    // Authentic human hand placements (Biometric Strike Clusters)
    private static readonly string[] MashClusters =
    [
        "asdf",
        "qwer",
        "zxcv",
        "wasd",
        "jkl;",
        "uiop",
        "m,./",
        "qwwe",
        "asdd",
        "zxcc",
        "xcv",
        "asdfasdf",
        "qweqwe",
        "zaq",
        "wsx",
        "edc",
        "rfv",
        "tgb",
        "yhn",
        "ujm",
        "asdfgh",
        "qwerty",
        "zxcvbn",
        "1234",
        "asdf1",
    ];

    // Pre-allocated array representing physical left-hand WASD cluster weightings
    private static readonly char[] LeftHandCluster =
    [
        'w',
        'a',
        's',
        'd',
        'q',
        'e',
        'r',
        'f',
        'z',
        'x',
        'c',
    ];
    private static readonly string[] LinearRolls = ["asdf", "qwer", "zxcv", "wasd"];
    private static readonly char[] Numbers = ['1', '2', '3', '4'];

    public void Generate(StringBuilder builder)
    {
        // 🚨 ENTROPY EXPLOSION: Combine 2 to 5 different clusters randomly
        // instead of repeating the same exact cluster.
        int clusterCount = Random.Shared.Next(2, 6);
        for (int i = 0; i < clusterCount; i++)
        {
            string cluster = MashClusters[Random.Shared.Next(MashClusters.Length)];
            builder.Append(cluster);
        }

        // 10% "Fat-Finger Collision" Injector
        if (Random.Shared.NextDouble() < 0.10 && builder.Length > 0)
        {
            int mutateIdx = Random.Shared.Next(builder.Length);
            builder[mutateIdx] = GetAdjacentKey(builder[mutateIdx]);
        }
    }

    // Mathematical constant-time physical keyboard adjacency map for the Collision Injector
    private static char GetAdjacentKey(char key) =>
        key switch
        {
            'q' => 'w',
            'w' => 'e',
            'e' => 'r',
            'r' => 't',
            'a' => 's',
            's' => 'd',
            'd' => 'f',
            'f' => 'g',
            'z' => 'x',
            'x' => 'c',
            'c' => 'v',
            'v' => 'b',
            'u' => 'i',
            'i' => 'o',
            'o' => 'p',
            'j' => 'k',
            'k' => 'l',
            'm' => ',',
            ',' => '.',
            _ => key, // Fallback
        };
}
