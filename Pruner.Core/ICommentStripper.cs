namespace Pruner.Core;

public interface ICommentStripper
{
    StripResult Strip(string source);
}