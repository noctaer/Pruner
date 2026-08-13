using System.Text;

namespace LuaCleaner.Core.Strippers;

public sealed class PythonCommentStripper : BaseCommentStripper
{
    public override StripResult Strip(string source)
    {
        var output = new StringBuilder(source.Length);
        int commentsRemoved = 0;
        int pos = 0;

        while (pos < source.Length)
        {
            char c = source[pos];

            // Triple-quoted strings: """ or '''
            if ((c == '"' || c == '\'') && pos + 2 < source.Length
                && source[pos + 1] == c && source[pos + 2] == c)
            {
                string delim = new string(c, 3);
                int end = source.IndexOf(delim, pos + 3, StringComparison.Ordinal);
                if (end < 0)
                {
                    output.Append(source, pos, source.Length - pos);
                    pos = source.Length;
                }
                else
                {
                    int len = end + 3 - pos;
                    output.Append(source, pos, len);
                    pos += len;
                }
                continue;
            }

            // Single-quoted strings: " or '
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
                    if (sc == c || sc == '\n')
                        break;
                }
                continue;
            }

            // Single-line comment
            if (c == '#')
            {
                while (pos < source.Length && source[pos] != '\n' && source[pos] != '\r')
                    pos++;
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