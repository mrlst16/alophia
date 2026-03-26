namespace Alophia.Models;

public sealed record Bot
{
    public string Name { get; init; } = string.Empty;
    public string Initials { get; init; } = string.Empty;
    public string BgColor { get; init; } = "#6366F1";
    public string FgColor { get; init; } = "#FFFFFF";
}
