using System.Threading.Channels;
using Aegitox.Bot.Core.Interfaces;

public class Tier1MicroBurstOrchestrator
{
    private readonly IMatrixParser _matrixParser;
    private readonly Random _random = new Random();

    // Semantic builders for the Micro-Burst
    private static readonly string[] _directives =
    {
        // Core Actions
        "stfu",
        "kys",
        "quit",
        "sit",
        "delete game",
        "ff",
        "uninstall",
        // Movement/Exit
        "gtfo",
        "leave",
        "log off",
        "alt f4",
        "go next",
        "dodge",
        "get out",
        // Psychological/Physical
        "choke",
        "cry more",
        "cope",
        "seethe",
        "touch grass",
        "get cancer",
        "drop dead",
        "plz quit",
        "shutup",
        "shut up",
        "kill urself",
        "neck urself",
    };

    private static readonly string[] _targets =
    {
        // Core Entities
        "bot",
        "dog",
        "kid",
        "idiot",
        "trash",
        "npc",
        "random",
        "ape",
        "scum",
        // General Toxicity
        "clown",
        "loser",
        "freak",
        "shitter",
        "pussy",
        "bitch",
        "cunt",
        "slut",
        "whore",
        "bastard",
        "dick",
        "prick",
        "bozo",
        "animal",
        "garbage",
        "waste",
        "moron",
        "imbecile",
        "troll",
        "subhuman",
        "tumor",
        // Specific Gamer Toxicity
        "noob",
        "feeder",
        "cancer",
        "griefer",
        "iron",
        "bronze",
        "hardstuck",
        "retard",
        "autist",
        "mong",
        "braindead",
    };

    private static readonly string[] _adjectives =
    {
        // --- 🔴 THE "FUCK" TREE DOMINANCE ---
        "fuck",
        "fucking",
        "fck",
        "fcking",
        "fckn",
        "fckng",
        "fucker",
        "fuk",
        "fuking",
        "fukk",
        "fukking",
        "f u c k",
        "f-u-c-k",
        "f_u_c_k",
        "f.u.c.k",
        "f u c k i n g",
        "fker",
        "fking",
        "go fk",
        "go fck",
        "fkng",
        "f*ck",
        "f*cking",
        "f**k",
        "f**king",
        "fcked",
        "fucked",
        "fvck",
        "fvcking",
        "phuck",
        "phucking",
        "fok",
        "fook",
        "fookin",
        "f_ck",
        "f_cking",
        "f k i n g",
        "f-k-i-n-g",
        "motherfucking",
        "mfing",
        "fkk",
        "f -u c-k",
        "f _u_ck",
        // --- Core Exaggerations & Absolutism ---
        "actual",
        "literal",
        "absolute",
        "insane",
        "unreal",
        // --- Qualitative Toxicity ---
        "dogshit",
        "shitty",
        "bullshit",
        "horseshit",
        "worthless",
        "useless",
        "brainless",
        "braindead",
        "delusional",
        "pathetic",
        "cringe",
        "cancerous",
        "toxic",
        "disgusting",
        "vile",
        "trashy",
        "mentally ill",
    };

    public Tier1MicroBurstOrchestrator(IMatrixParser matrixParser)
    {
        _matrixParser = matrixParser;
    }

    public async Task GenerateDatasetAsync(string matrixPath, string outputPath, int targetRowQuota)
    {
        // 1. Await the task to unpack the VoidLexicon object
        var lexicon = await _matrixParser.ParseAsync(matrixPath);

        // 2. Extract the string arrays from the Lexicon object.
        // (Use .Concat if you need both lists. If you only want the toxic words, you can just use `var matrixWords = lexicon.BaseProfanity;`)
        var matrixWords = lexicon.BaseProfanity.Concat(lexicon.Resolutions).ToArray();

        if (matrixWords.Length == 0)
            throw new InvalidOperationException("Fatal: Aegitox matrix is empty or missing.");

        var channel = Channel.CreateBounded<string>(
            new BoundedChannelOptions(50000) { SingleWriter = false, SingleReader = true }
        );

        var consumerTask = ConsumeAndWriteAsync(channel.Reader, outputPath, targetRowQuota);
        var uniqueSignatures = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        int generated = 0;

        while (generated < targetRowQuota)
        {
            string phrase = GenerateMicroBurst(matrixWords);
            string cleanPhrase = phrase.Trim().ToLower();

            // O(1) deduplication check
            if (uniqueSignatures.Add(cleanPhrase))
            {
                // Generate a random Nuclear score between 0.97 and 0.99
                double score = Math.Round(_random.NextDouble() * (0.99 - 0.97) + 0.97, 2);

                // Write out the strictly formatted ML training string (InvariantCulture ensures periods, not commas, for decimals)
                string scoreString = score.ToString(
                    System.Globalization.CultureInfo.InvariantCulture
                );
                string csvLine = $"\"{cleanPhrase}\",{scoreString},Nuclear\n";

                await channel.Writer.WriteAsync(csvLine);
                generated++;
            }
        }

        channel.Writer.Complete();
        await consumerTask;
    }

    private string GenerateMicroBurst(string[] matrixWords)
    {
        int length = _random.Next(1, 4); // Randomly choose 1, 2, or 3 words
        string mWord = matrixWords[_random.Next(matrixWords.Length)];

        if (length == 1)
        {
            return mWord;
        }
        else if (length == 2)
        {
            int pattern = _random.Next(0, 3);
            return pattern switch
            {
                0 => $"{_directives[_random.Next(_directives.Length)]} {mWord}", // e.g., "stfu [matrixWord]"
                1 => $"{_adjectives[_random.Next(_adjectives.Length)]} {mWord}", // e.g., "actual [matrixWord]"
                _ => $"{mWord} {_targets[_random.Next(_targets.Length)]}" // e.g., "[matrixWord] bot"
            };
        }
        else // length == 3
        {
            int pattern = _random.Next(0, 2);
            return pattern switch
            {
                0 =>
                    $"{_directives[_random.Next(_directives.Length)]} {_adjectives[_random.Next(_adjectives.Length)]} {mWord}", // e.g., "stfu absolute [matrixWord]"
                _ =>
                    $"{_adjectives[_random.Next(_adjectives.Length)]} {mWord} {_targets[_random.Next(_targets.Length)]}" // e.g., "fking [matrixWord] bot"
            };
        }
    }

    private async Task ConsumeAndWriteAsync(
        ChannelReader<string> reader,
        string outputPath,
        int quota
    )
    {
        using var stream = new FileStream(
            outputPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 1024 * 1024,
            useAsync: true
        );
        using var writer = new StreamWriter(stream);

        // FIXED: Replaced IntentId header with the Golden Matrix ML Training header
        await writer.WriteAsync("Content,Score,Category\n");

        int written = 0;
        await foreach (var line in reader.ReadAllAsync())
        {
            await writer.WriteAsync(line);
            written++;
            if (written >= quota)
                break;
        }
    }
}
