using System.Text;

namespace Pruner.Core.Strippers;

public sealed class HaskellCommentStripper : BaseCommentStripper
{
    public override StripResult Strip(string source)
    {
        if (source.Length == 0)
            return new StripResult { CleanedSource = string.Empty, CommentsRemoved = 0 };

        var sb = new StringBuilder(source.Length);
        int i = 0;
        int count = 0;

        while (i < source.Length)
        {
            char c = source[i];

            if (c == '"')
            {
                int start = i++;
                while (i < source.Length)
                {
                    if (source[i] == '\\' && i + 1 < source.Length)
                    {
                        i += 2;
                    }
                    else if (source[i] == '"')
                    {
                        i++;
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
                sb.Append(source, start, i - start);
                continue;
            }

            if (c == '\'' && i + 2 < source.Length)
            {
                if (source[i + 1] == '\\' && i + 3 < source.Length && source[i + 3] == '\'')
                {
                    sb.Append(source, i, 4);
                    i += 4;
                    continue;
                }
                if (source[i + 2] == '\'')
                {
                    sb.Append(source, i, 3);
                    i += 3;
                    continue;
                }
            }

            if (c == '{' && i + 1 < source.Length && source[i + 1] == '-')
            {
                i += 2;
                int depth = 1;
                while (i < source.Length && depth > 0)
                {
                    if (source[i] == '{' && i + 1 < source.Length && source[i + 1] == '-')
                    {
                        depth++;
                        i += 2;
                    }
                    else if (source[i] == '-' && i + 1 < source.Length && source[i + 1] == '}')
                    {
                        depth--;
                        i += 2;
                    }
                    else
                    {
                        i++;
                    }
                }
                count++;
                continue;
            }

            if (c == '-' && i + 1 < source.Length && source[i + 1] == '-')
            {
                while (i < source.Length && source[i] != '\n')
                    i++;
                TrimTrailingWhitespace(sb);
                count++;
                continue;
            }

            sb.Append(c);
            i++;
        }

        return new StripResult
        {
            CleanedSource = CollapseBlankLines(sb.ToString()),
            CommentsRemoved = count,
        };
    }
}