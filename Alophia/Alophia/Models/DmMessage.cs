namespace Alophia.Models;

public sealed record DmMessage
{
    public string From { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
    public string Time { get; init; } = string.Empty;
}
