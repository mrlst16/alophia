using System.Collections.Generic;

namespace Alophia.Models;

public sealed record Requirement
{
    public string Id { get; init; } = string.Empty;
    public string Type { get; init; } = "epic";
    public string Title { get; init; } = string.Empty;
    public string Status { get; init; } = "todo";
    public string Assignee { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<string> Acceptance { get; init; } = [];
    public IReadOnlyList<RequirementTask> Tasks { get; init; } = [];
}

public sealed record RequirementTask
{
    public string Id { get; init; } = string.Empty;
    public string Type { get; init; } = "task";
    public string Title { get; init; } = string.Empty;
    public string Status { get; init; } = "todo";
    public string Assignee { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<string> Acceptance { get; init; } = [];
}
