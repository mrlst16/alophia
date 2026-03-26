using System.Collections.Generic;

namespace Alophia.Models;

public sealed record Repository
{
    public string Id { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string Color { get; init; } = "#6366F1";
    public IReadOnlyList<Commit> Commits { get; init; } = [];
}

public sealed record Commit
{
    public string Hash { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string Time { get; init; } = string.Empty;
    public IReadOnlyList<FileDiff> Files { get; init; } = [];
}

public sealed record FileDiff
{
    public string File { get; init; } = string.Empty;
    public IReadOnlyList<string> Lines { get; init; } = [];
}
