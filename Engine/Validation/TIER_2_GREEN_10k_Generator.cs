using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Aegitox.Bot.Data.Generators;

/// <summary>
/// Tier 2: Cross-Platform Logistical Micro-Bursts (10,000 rows).
/// Injects benign, structurally identical 1-3 word spans to counter length-starvation false positives.
/// Employs deterministic O(1) mathematical routing to ensure exact distribution quotas.
/// </summary>
// ==========================================================
// TIER 2: LOGISTICAL MICRO-BURST VOCABULARY EXPANSION
// ==========================================================

public sealed class TIER_2_GREEN_10k_Generator
{
    // ---------------------------------------------------------
    // CATEGORY 1: GAMING SHORTHAND (Low-Latency Tactical & Status)
    // ---------------------------------------------------------
    private static readonly ImmutableArray<string> _gamingActions = ImmutableArray.Create(
        "push",
        "defend",
        "need",
        "drop",
        "lagging",
        "rushing",
        "holding",
        "flanking",
        "pulling",
        "baiting",
        "covering",
        "rotating",
        "pinging",
        "tracking",
        "scouting",
        "planting",
        "defusing",
        "peeking",
        "saving",
        "stacking",
        "resetting",
        "ulting",
        "healing",
        "reviving",
        "crafting",
        "looting",
        "trading",
        "farming",
        "focusing",
        "muting",
        "checking",
        "joining",
        "leaving",
        "crashing",
        "streaming",
        "queueing",
        "carrying",
        "throwing",
        "warding",
        "buying",
        "selling",
        "equipping",
        "reloading",
        "spawning",
        "rushing",
        "camping",
        "kiting",
        "zoning",
        "smurfing",
        "dodging",
        // ── Append to _gamingActions  (+50 → total 99; also remove the duplicate "rushing") ──
        "ganking",
        "engaging",
        "disengaging",
        "diving",
        "backing",
        "freezing",
        "roaming",
        "invading",
        "peeling",
        "poking",
        "sieging",
        "anchoring",
        "tapping",
        "sprinting",
        "crouching",
        "sniping",
        "jungling",
        "laning",
        "escaping",
        "chasing",
        "patrolling",
        "ambushing",
        "countering",
        "sweeping",
        "dueling",
        "raiding",
        "grinding",
        "questing",
        "surrendering",
        "spectating",
        "reporting",
        "warping",
        "boosting",
        "spraying",
        "aiming",
        "sliding",
        "vaulting",
        "swapping",
        "snowballing",
        "splitpushing",
        "protecting",
        "escorting",
        "assaulting",
        "neutralizing",
        "capping",
        "backdooring",
        "contesting",
        "staggering",
        "leashing",
        "harassing"
    );

    private static readonly ImmutableArray<string> _gamingTargets = ImmutableArray.Create(
        "mid",
        "heals",
        "ammo",
        "site",
        "spawn",
        "point",
        "base",
        "cart",
        "top",
        "bot",
        "jungle",
        "objective",
        "flag",
        "payload",
        "shields",
        "armor",
        "mana",
        "cooldowns",
        "ult",
        "stats",
        "ping",
        "fps",
        "lag",
        "lobby",
        "queue",
        "discord",
        "voice",
        "mic",
        "comms",
        "stream",
        "main",
        "alt",
        "smurf",
        "carry",
        "support",
        "tank",
        "dps",
        "aggro",
        "loot",
        "mats",
        "gold",
        "xp",
        "creeps",
        "minions",
        "boss",
        "adds",
        "buff",
        "nerf",
        "party",
        "guild",
        "clan",
        "squad",
        "duo",
        "trio",
        "settings",
        "binds",
        // ── Append to _gamingTargets  (+50 → total 106) ─────────────────────────
        "flank",
        "angle",
        "corner",
        "choke",
        "tunnel",
        "bridge",
        "tower",
        "nexus",
        "throne",
        "crystal",
        "core",
        "barracks",
        "fountain",
        "streak",
        "match",
        "round",
        "zone",
        "lane",
        "wall",
        "pit",
        "turret",
        "sentry",
        "bombsite",
        "extract",
        "cache",
        "crate",
        "chest",
        "vault",
        "health",
        "stamina",
        "energy",
        "meter",
        "respawn",
        "chokepoint",
        "loadout",
        "inventory",
        "backpack",
        "radar",
        "minimap",
        "scoreboard",
        "leaderboard",
        "shop",
        "map",
        "route",
        "perimeter",
        "kills",
        "assists",
        "deaths",
        "rank",
        "account"
    );

    private static readonly ImmutableArray<string> _gamingStatus = ImmutableArray.Create(
        "brb",
        "omw",
        "afk",
        "gg",
        "mb",
        "wp",
        "glhf",
        "nt",
        "ty",
        "np",
        "rq",
        "dc",
        "lfp",
        "lfg",
        "pve",
        "pvp",
        "aoe",
        "cc",
        "oom",
        "hp",
        "rng",
        "ttyl",
        "cya",
        "sec",
        "wtb",
        "wts",
        "wtt",
        "inc",
        "bbl",
        "gn",
        "gm",
        "thx",
        "yw",
        "idk",
        "idc",
        "irl",
        "jk",
        "nvm",
        "tbh",
        "ggs",
        "ez",
        "ff",
        "vgg",
        "lmao",
        "lol",
        "rofl",
        "imo",
        "imho",
        "eta",
        // ── Append to _gamingStatus  (+100 → total 149) ────────────────────────
        // Universal gaming / chat reactions
        "gl",
        "hf",
        "bg",
        "gz",
        "gj",
        "f",
        "rip",
        "oof",
        "sus",
        "pog",
        "kek",
        "smh",
        "gtg",
        "wb",
        "rn",
        "ngl",
        "ofc",
        "nah",
        "fr",
        "xd",
        "lmfao",
        "bbs",
        "k",
        "ok",
        "ig",
        "omg",
        "lul",
        "kk",
        "yolo",
        "wya",
        // Meta / rank signals
        "dced",
        "diff",
        "based",
        "poggers",
        "w",
        "l",
        "og",
        "op",
        "elo",
        "mmr",
        "mia",
        "rez",
        "ready",
        "clear",
        "done",
        "req",
        "dm",
        "meta",
        "tbf",
        "fwiw",
        // Reaction shorthand
        "wdym",
        "obv",
        "yep",
        "hype",
        "stomp",
        "tilt",
        "fed",
        "gratz",
        "bruh",
        "bet",
        "goat",
        "cap",
        "nope",
        "sure",
        "meh",
        "soon",
        "chill",
        "wow",
        "nice",
        "dude",
        "bro",
        "lowkey",
        "deadass",
        "facts",
        "mood",
        // Coordination
        "valid",
        "noted",
        "heard",
        "copy",
        "roger",
        "hold",
        "wait",
        "standby",
        "here",
        "close",
        "iirc",
        "afaik",
        "ama",
        "tmr",
        "nbd",
        // Game modes / context
        "scrim",
        "pub",
        "ranked",
        "casual",
        "customs",
        "aram",
        "draft",
        "inhouse",
        "tourney",
        "league"
    );

    // ---------------------------------------------------------
    // CATEGORY 2: PROFESSIONAL SHORTHAND (Corporate/Tech/Remote)
    // ---------------------------------------------------------
    private static readonly ImmutableArray<string> _profActions = ImmutableArray.Create(
        "check",
        "merge",
        "rebooting",
        "ping",
        "deploying",
        "reverting",
        "syncing",
        "approved",
        "reviewing",
        "debugging",
        "testing",
        "pushing",
        "pulling",
        "committing",
        "updating",
        "installing",
        "fixing",
        "closing",
        "opening",
        "resolving",
        "assigning",
        "blocking",
        "unblocking",
        "tracking",
        "auditing",
        "logging",
        "scaling",
        "migrating",
        "cloning",
        "building",
        "compiling",
        "drafting",
        "reading",
        "writing",
        "sending",
        "forwarding",
        "replying",
        "joining",
        "presenting",
        "formatting",
        "parsing",
        "rendering",
        "caching",
        "hosting",
        "querying",
        "fetching",
        "patching",
        "mocking",
        "routing",
        "restarting",
        // ── Append to _profActions  (+50 → total 100) ────────────────────────────
        "importing",
        "exporting",
        "uploading",
        "downloading",
        "archiving",
        "deleting",
        "creating",
        "modifying",
        "validating",
        "verifying",
        "linting",
        "refactoring",
        "optimizing",
        "profiling",
        "benchmarking",
        "monitoring",
        "alerting",
        "notifying",
        "scheduling",
        "triggering",
        "invoking",
        "registering",
        "subscribing",
        "encrypting",
        "decrypting",
        "signing",
        "tokenizing",
        "serializing",
        "deserializing",
        "indexing",
        "searching",
        "filtering",
        "sorting",
        "paginating",
        "aggregating",
        "summarizing",
        "analyzing",
        "visualizing",
        "charting",
        "graphing",
        "mapping",
        "transforming",
        "normalizing",
        "cleaning",
        "documenting",
        "approving",
        "rejecting",
        "escalating",
        "spawning",
        "connecting"
    );

    private static readonly ImmutableArray<string> _profTargets = ImmutableArray.Create(
        "repo",
        "pr",
        "system",
        "me",
        "main",
        "prod",
        "branch",
        "ticket",
        "dev",
        "staging",
        "master",
        "trunk",
        "bug",
        "feature",
        "hotfix",
        "patch",
        "server",
        "database",
        "api",
        "ui",
        "backend",
        "frontend",
        "logs",
        "metrics",
        "cloud",
        "instance",
        "container",
        "pod",
        "cluster",
        "meeting",
        "call",
        "sync",
        "standup",
        "retro",
        "demo",
        "email",
        "slack",
        "teams",
        "chat",
        "thread",
        "doc",
        "wiki",
        "spec",
        "reqs",
        "code",
        "script",
        "node",
        "app",
        "pipeline",
        // ── Append to _profTargets  (+50 → total 99) ─────────────────────────────
        "framework",
        "library",
        "package",
        "module",
        "service",
        "microservice",
        "endpoint",
        "webhook",
        "queue",
        "topic",
        "event",
        "schema",
        "table",
        "record",
        "field",
        "index",
        "query",
        "view",
        "function",
        "method",
        "class",
        "interface",
        "struct",
        "enum",
        "lambda",
        "runtime",
        "build",
        "artifact",
        "image",
        "registry",
        "namespace",
        "config",
        "secret",
        "token",
        "cert",
        "key",
        "proxy",
        "gateway",
        "firewall",
        "vpc",
        "subnet",
        "env",
        "envvar",
        "workflow",
        "job",
        "task",
        "sprint",
        "epic",
        "story",
        "route"
    );

    private static readonly ImmutableArray<string> _profStatus = ImmutableArray.Create(
        "wfh",
        "pto",
        "brb",
        "ack",
        "lgtm",
        "tbd",
        "wip",
        "ooo",
        "fyi",
        "fya",
        "tldr",
        "eta",
        "nsfw",
        "sfw",
        "afk",
        "eod",
        "eow",
        "cob",
        "q1",
        "q2",
        "q3",
        "q4",
        "ytd",
        "kpi",
        "okr",
        "roi",
        "mvp",
        "qa",
        "uat",
        "poc",
        "rfc",
        "cr",
        "lmk",
        "imo",
        "imho",
        "rsvp",
        "asap",
        "nrn",
        "eom",
        "v1",
        "v2",
        "b2b",
        "b2c",
        "sla",
        "to",
        "cc",
        "bcc",
        "fw",
        "re",
        "nd",
        // ── Append to _profStatus  (+100 → total 150) ───────────────────────────
        // DevOps / infra / language
        "p0",
        "p1",
        "p2",
        "p3",
        "sev1",
        "sev2",
        "ci",
        "cd",
        "ops",
        "devops",
        "sre",
        "ml",
        "ai",
        "k8s",
        "vm",
        "cli",
        "db",
        "sql",
        "sdk",
        "js",
        "ts",
        // Business metrics / titles
        "vp",
        "cto",
        "coo",
        "pm",
        "yoy",
        "mom",
        "qoq",
        "eoy",
        "eoq",
        "dau",
        "mau",
        "nps",
        "mrr",
        "arr",
        "cac",
        "ltv",
        "arpu",
        "ctr",
        "hq",
        "svp",
        // Release / security
        "v0",
        "v3",
        "v4",
        "v5",
        "rc",
        "ga",
        "sso",
        "mfa",
        "vpn",
        "dns",
        "cdn",
        "ssl",
        "tls",
        // Professional chat shorthand
        "nbd",
        "tmr",
        "ama",
        "iirc",
        "afaik",
        "ftr",
        "tia",
        "bbs",
        "bbl",
        "noted",
        "heard",
        "copy",
        "done",
        "wait",
        "fwiw",
        "wdym",
        "tbf",
        "obv",
        "yep",
        "kk",
        "ok",
        "k",
        "ig",
        "fr",
        "nah",
        "ofc",
        "ngl",
        "wb",
        "gtg",
        "smh",
        "lol",
        "hf",
        "gl",
        "sure",
        "meh",
        "valid",
        "req",
        // Tech formats / misc
        "json",
        "csv",
        "rest",
        "ux",
        "ootb",
        "vip",
        "d2c",
        "b2g",
        "grpc",
        "rnd"
    );

    // Pre-allocated classification parameters to bypass heap fragmentation
    private static readonly ImmutableArray<string> _benignScores = ImmutableArray.Create(
        "0.10",
        "0.12",
        "0.13"
    );
    private const string GamingCategory = "Green";
    private const string ProfCategory = "Green";

    public async Task GenerateAsync(
        ChannelWriter<string> gamingWriter,
        ChannelWriter<string> profWriter,
        CancellationToken cancellationToken = default
    )
    {
        var task1 = StreamDomainAsync(
            gamingWriter,
            _gamingActions,
            _gamingTargets,
            _gamingStatus,
            GamingCategory,
            5000,
            cancellationToken
        );
        var task2 = StreamDomainAsync(
            profWriter,
            _profActions,
            _profTargets,
            _profStatus,
            ProfCategory,
            5000,
            cancellationToken
        );

        await Task.WhenAll(task1, task2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task StreamDomainAsync(
        ChannelWriter<string> writer,
        ImmutableArray<string> actions,
        ImmutableArray<string> targets,
        ImmutableArray<string> statuses,
        string category,
        int quota, // 5 000
        CancellationToken cancellationToken
    )
    {
        // ── Tier quotas ─────────────────────────────────────────────────────────
        // Tier 1: exhaust every unique status exactly once
        int tier1Count = statuses.Length; // 149 / 150
        // Tier 2a/2b: each pulls from a separate 2-word Cartesian product
        int tier2aCount = 1_000; // action + target
        int tier2bCount = 1_000; // status + target
        // Tier 3: remainder drawn from ~1.56 M combos — zero collision risk
        int tier3Count = quota - tier1Count - tier2aCount - tier2bCount; // ~2 850

        // ── Build pools ─────────────────────────────────────────────────────────

        // Tier 1 — standalone status words
        var tier1Pool = statuses.ToArray();
        Shuffle(tier1Pool);

        // Tier 2a — "action target"  (~10 494 gaming / ~10 600 prof combos)
        var tier2aPool = CartesianPairs(actions, targets);
        Shuffle(tier2aPool);

        // Tier 2b — "status target"  (~15 794 gaming / ~15 900 prof combos)
        var tier2bPool = CartesianPairs(statuses, targets);
        Shuffle(tier2bPool);

        // Tier 3 — "status action target"  (~1.56 M combos)
        // Encode each triple as a single integer; partial Fisher-Yates selects
        // exactly tier3Count unique ones without materialising 1.56 M strings.
        int sLen = statuses.Length;
        int aLen = actions.Length;
        int tLen = targets.Length;
        int totalTriplets = sLen * aLen * tLen; // ≈ 1 563 894  (~6 MB int[])
        var tripletIdx = new int[totalTriplets];
        for (int i = 0; i < totalTriplets; i++)
            tripletIdx[i] = i;
        PartialShuffle(tripletIdx, tier3Count); // only walks tier3Count steps

        // ── Assemble all rows into a flat array ─────────────────────────────────
        var allRows = new string[quota];
        int pos = 0;

        for (int i = 0; i < tier1Count; i++)
            allRows[pos++] = tier1Pool[i];
        for (int i = 0; i < tier2aCount; i++)
            allRows[pos++] = tier2aPool[i];
        for (int i = 0; i < tier2bCount; i++)
            allRows[pos++] = tier2bPool[i];

        for (int i = 0; i < tier3Count; i++)
        {
            int encoded = tripletIdx[i];
            int sIdx = encoded / (aLen * tLen);
            int aIdx = (encoded / tLen) % aLen;
            int tIdx = encoded % tLen;
            allRows[pos++] = string.Concat(statuses[sIdx], " ", actions[aIdx], " ", targets[tIdx]);
        }

        // Final inter-tier shuffle so word-count tiers are interspersed naturally
        Shuffle(allRows);

        // ── Stream to channel ────────────────────────────────────────────────────
        await writer.WriteAsync("Content,Score,Category", cancellationToken);

        var rng = Random.Shared;
        foreach (var content in allRows)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string score = _benignScores[rng.Next(_benignScores.Length)];
            await writer.WriteAsync(
                string.Concat(content, ",", score, ",", category),
                cancellationToken
            );
        }
    }

    /// <summary>Full Fisher-Yates shuffle — O(n), in-place.</summary>
    private static void Shuffle<T>(T[] arr)
    {
        var rng = Random.Shared; // thread-safe in .NET 6+
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }

    /// <summary>
    /// Partial Fisher-Yates: after this call arr[0..count-1] is a uniform
    /// random sample of <paramref name="count"/> distinct elements.
    /// Only walks count steps — never materialises the full shuffle.
    /// </summary>
    private static void PartialShuffle(int[] arr, int count)
    {
        var rng = Random.Shared;
        int n = arr.Length;
        for (int i = 0; i < count; i++)
        {
            int j = rng.Next(i, n); // [i, n-1] inclusive
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }

    /// <summary>Enumerate every "a b" pair in the Cartesian product.</summary>
    private static string[] CartesianPairs(ImmutableArray<string> a, ImmutableArray<string> b)
    {
        var result = new string[a.Length * b.Length];
        int k = 0;
        foreach (var x in a)
        foreach (var y in b)
            result[k++] = string.Concat(x, " ", y);
        return result;
    }
}
