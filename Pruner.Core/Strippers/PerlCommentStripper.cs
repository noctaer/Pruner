using System.Text;

namespace Pruner.Core.Strippers;

public sealed class PerlCommentStripper : BaseCommentStripper
{
    public override StripResult Strip(string source)
    {
        if (source.Length == 0)
            return new StripResult { CleanedSource = string.Empty, CommentsRemoved = 0 };

        var sb = new StringBuilder(source.Length);
        int i = 0;
        int count = 0;
        bool lastTokenWasValue = false;

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
                lastTokenWasValue = true;
                continue;
            }

            if (c == '\'')
            {
                int start = i++;
                while (i < source.Length)
                {
                    if (source[i] == '\\' && i + 1 < source.Length)
                    {
                        i += 2;
                    }
                    else if (source[i] == '\'')
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
                lastTokenWasValue = true;
                continue;
            }

            if (c == '<' && i + 1 < source.Length && source[i + 1] == '<')
            {
                int heredocStart = i;
                i += 2;

                bool indented = i < source.Length && source[i] == '~';
                if (indented) i++;

                char delim = i < source.Length ? source[i] : '\0';
                bool quoted = delim == '\'' || delim == '"' || delim == '`';
                if (quoted) i++;

                int identStart = i;
                while (i < source.Length && source[i] != '\n' && source[i] != '\r' &&
                       (quoted ? source[i] != delim : (char.IsLetterOrDigit(source[i]) || source[i] == '_')))
                    i++;

                string marker = source.Substring(identStart, i - identStart);
                if (quoted && i < source.Length && source[i] == delim) i++;

                while (i < source.Length && source[i] != '\n') i++;
                if (i < source.Length) i++;

                sb.Append(source, heredocStart, i - heredocStart);

                while (i < source.Length)
                {
                    int lineStart = i;
                    while (i < source.Length && source[i] != '\n') i++;
                    string line = source.Substring(lineStart, i - lineStart).TrimEnd('\r');
                    string trimmed = indented ? line.TrimStart() : line;
                    sb.Append(source, lineStart, i - lineStart);
                    if (i < source.Length) { sb.Append('\n'); i++; }
                    if (trimmed == marker) break;
                }

                lastTokenWasValue = true;
                continue;
            }

            if (c == '/' && !lastTokenWasValue)
            {
                int start = i++;
                while (i < source.Length)
                {
                    if (source[i] == '\\' && i + 1 < source.Length)
                    {
                        i += 2;
                    }
                    else if (source[i] == '/')
                    {
                        i++;
                        while (i < source.Length && char.IsLetter(source[i])) i++;
                        break;
                    }
                    else if (source[i] == '\n')
                    {
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
                sb.Append(source, start, i - start);
                lastTokenWasValue = true;
                continue;
            }

            if (c == '#')
            {
                while (i < source.Length && source[i] != '\n')
                    i++;
                TrimTrailingWhitespace(sb);
                count++;
                lastTokenWasValue = false;
                continue;
            }

            if (char.IsLetterOrDigit(c) || c == '_' || c == ')' || c == ']' || c == '}')
                lastTokenWasValue = true;
            else if (c == '\n')
                lastTokenWasValue = false;
            else if (!char.IsWhiteSpace(c) && c != '/')
                lastTokenWasValue = false;

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