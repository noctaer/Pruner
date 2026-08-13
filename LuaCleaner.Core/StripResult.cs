namespace LuaCleaner.Core;

public sealed class StripResult
{
    public required string CleanedSource { get; init; }
    public int CommentsRemoved { get; init; }
}