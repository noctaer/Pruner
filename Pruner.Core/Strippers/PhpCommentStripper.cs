using System.Text;

namespace Pruner.Core.Strippers;

public sealed class PhpCommentStripper : BaseCommentStripper
{
    public override StripResult Strip(string source)
    {
        var output = new StringBuilder(source.Length);
        int commentsRemoved = 0;
        int pos = 0;

        while (pos < source.Length)
        {
            char c = source[pos];

            // Single-line comment //
            if (c == '/' && pos + 1 < source.Length && source[pos + 1] == '/')
            {
                pos += 2;
                while (pos < source.Length && source[pos] != '\n' && source[pos] != '\r')
                    pos++;
                commentsRemoved++;
                TrimTrailingWhitespace(output);
                continue;
            }

            // Single-line comment #
            if (c == '#')
            {
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

            // Heredoc/Nowdoc: <<<IDENT or <<<'IDENT'
            if (c == '<' && pos + 2 < source.Length && source[pos + 1] == '<' && source[pos + 2] == '<')
            {
                int heredocStart = pos;
                int scan = pos + 3;
                while (scan < source.Length && source[scan] == ' ') scan++;

                string? delimiter = null;

                if (scan < source.Length && source[scan] == '\'')
                {
                    scan++;
                    int delimStart = scan;
                    while (scan < source.Length && source[scan] != '\'') scan++;
                    delimiter = source.Substring(delimStart, scan - delimStart);
                    if (scan < source.Length) scan++;
                }
                else if (scan < source.Length && source[scan] == '"')
                {
                    scan++;
                    int delimStart = scan;
                    while (scan < source.Length && source[scan] != '"') scan++;
                    delimiter = source.Substring(delimStart, scan - delimStart);
                    if (scan < source.Length) scan++;
                }
                else if (scan < source.Length && (char.IsLetter(source[scan]) || source[scan] == '_'))
                {
                    int delimStart = scan;
                    while (scan < source.Length && (char.IsLetterOrDigit(source[scan]) || source[scan] == '_'))
                        scan++;
                    delimiter = source.Substring(delimStart, scan - delimStart);
                }

                if (delimiter != null)
                {
                    int endOfOpeningLine = scan;
                    while (endOfOpeningLine < source.Length && source[endOfOpeningLine] != '\n')
                        endOfOpeningLine++;
                    if (endOfOpeningLine < source.Length) endOfOpeningLine++;

                    output.Append(source, heredocStart, endOfOpeningLine - heredocStart);
                    pos = endOfOpeningLine;

                    while (pos < source.Length)
                    {
                        int lineStart = pos;
                        int lineEnd = pos;
                        while (lineEnd < source.Length && source[lineEnd] != '\n') lineEnd++;
                        if (lineEnd < source.Length) lineEnd++;

                        string rawLine = source.Substring(lineStart, lineEnd - lineStart);
                        string lineContent = rawLine.TrimEnd('\r', '\n');

                        output.Append(rawLine);
                        pos = lineEnd;

                        if (lineContent == delimiter || lineContent == delimiter + ";")
                            break;
                    }
                    continue;
                }
            }

            // Double-quoted strings
            if (c == '"')
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
                    if (sc == '"' || sc == '\n') break;
                }
                continue;
            }

            // Single-quoted strings
            if (c == '\'')
            {
                output.Append(c);
                pos++;
                while (pos < source.Length)
                {
                    char sc = source[pos];
                    output.Append(sc);
                    pos++;
                    if (sc == '\\' && pos < source.Length && (source[pos] == '\\' || source[pos] == '\''))
                    {
                        output.Append(source[pos]);
                        pos++;
                        continue;
                    }
                    if (sc == '\'' || sc == '\n') break;
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