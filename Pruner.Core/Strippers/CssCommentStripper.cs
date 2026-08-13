using System.Text;

namespace Pruner.Core.Strippers;

public sealed class CssCommentStripper : BaseCommentStripper
{
    public override StripResult Strip(string source)
    {
        var output = new StringBuilder(source.Length);
        int commentsRemoved = 0;
        int pos = 0;

        while (pos < source.Length)
        {
            char c = source[pos];

            // Block comment /* */
            if (c == '/' && pos + 1 < source.Length && source[pos + 1] == '*')
            {
                pos += 2;
                while (pos + 1 < source.Length && !(source[pos] == '*' && source[pos + 1] == '/'))
                    pos++;
                if (pos + 1 < source.Length)
                    pos += 2;
                commentsRemoved++;
                TrimTrailingWhitespace(output);
                continue;
            }

            // Strings " and '
            if (c == '"' || c == '\'')
            {
                output.Append(c);
                pos++;
                while (pos < source.Length)
                {
                    char sc = source[pos];
                    output.Append(sc);
                    pos++;
                    if (sc == '\\' && pos < source.Length)
                    {
                        output.Append(source[pos]);
                        pos++;
                        continue;
                    }
                    if (sc == c) break;
                }
                continue;
            }

            output.Append(c);
            pos++;
        }

        return new StripResult
        {
            CleanedSource = CollapseBlankLines(output.ToString()),
            CommentsRemoved = commentsRemoved,
        };
    }
}