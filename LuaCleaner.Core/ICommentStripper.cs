namespace LuaCleaner.Core;

public interface ICommentStripper
{
    StripResult Strip(string source);
}