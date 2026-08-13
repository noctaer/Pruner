using System.Text;

namespace LuaCleaner.Core.Strippers;

public sealed class SqlCommentStripper : BaseCommentStripper
{
    public override StripResult Strip(string source)
    {
        var output = new StringBuilder(source.Length);
        int commentsRemoved = 0;
        int pos = 0;

        while (pos < source.Length)
        {
            char c = source[pos];

            // Single-quoted string literals
            if (c == '\'')
            {
                output.Append(c);
                pos++;
                while (pos < source.Length)
                {
                    char sc = source[pos];
                    output.Append(sc);
                    pos++;
                    // SQL escapes single quote by doubling: ''
                    if (sc == '\'' && pos < source.Length && source[pos] == '\'')
                    {
                        output.Append(source[pos]);
                        pos++;
                        continue;
                    }
                    if (sc == '\'')
                        break;
                }
                continue;
            }

            // Single-line comment --
            if (c == '-' && pos + 1 < source.Length && source[pos + 1] == '-')
            {
                pos += 2;
                while (pos < source.Length && source[pos] != '\n' && source[pos] != '\r')
                    pos++;
                commentsRemoved++;
                TrimTrailingWhitespace(output);
                continue;
            }

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