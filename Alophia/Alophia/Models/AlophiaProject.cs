using System.Collections.Generic;

namespace Alophia.Models;

public sealed record AlophiaProject
{
    public string FormatVersion { get; init; } = "1.0";
    public string Title { get; init; } = string.Empty;
    public int TotalWeeks { get; init; } = 16;
    public string ContextPath { get; init; } = string.Empty;  // root folder
    public string? FilePath { get; init; }  // where the .ap file is saved

    public IReadOnlyList<Stage> Stages { get; init; } = [];
    public IReadOnlyList<Bot> Bots { get; init; } = [];
    public IReadOnlyList<Requirement> Requirements { get; init; } = [];
    public IReadOnlyList<Repository> Repositories { get; init; } = [];

    // Key = bot name
    public IReadOnlyDictionary<string, IReadOnlyList<DmMessage>> DirectMessages { get; init; } =
        new Dictionary<string, IReadOnlyList<DmMessage>>();
}
