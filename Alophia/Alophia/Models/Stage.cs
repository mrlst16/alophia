namespace Alophia.Models;

public sealed record Stage
{
    public string Id { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string Bot { get; init; } = string.Empty;
    public int Start { get; init; }
    public int Duration { get; init; }
    public int Actual { get; init; }
    public string ColorSolid { get; init; } = "#6366F1";
    public string ColorLight { get; init; } = "#E0E7FF";
    public string? FolderPath { get; init; }  // null = fixed stage, set = project stage

    public bool IsProject => !string.IsNullOrEmpty(FolderPath);
}
