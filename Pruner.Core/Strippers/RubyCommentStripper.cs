using System.Text;

namespace Pruner.Core.Strippers;

public sealed class RubyCommentStripper : BaseCommentStripper
{
    // Tokens que indicam que um '/' seguinte é DIVISÃO, não regex.
    // Qualquer outro contexto trata '/' como início de regex literal.
    private static readonly System.Collections.Generic.HashSet<char> ValueEndChars =
        new() { ')', ']', '}' };

    public override StripResult Strip(string source)
    {
        var output = new StringBuilder(source.Length);
        int commentsRemoved = 0;
        int pos = 0;
        bool lastTokenWasValue = false;

        while (pos < source.Length)
        {
            char c = source[pos];

            // =begin / =end block comment — must start at column 0
            if (c == '=' && IsLineStart(source, pos) && MatchesAt(source, pos, "=begin"))
            {
                int afterTag = pos + 6;
                if (afterTag >= source.Length || IsWhitespaceOrNewline(source[afterTag]))
                {
                    pos = SkipToEndOfLine(source, pos);
                    while (pos < source.Length)
                    {
                        int lineStart = pos;
                        if (MatchesAt(source, lineStart, "=end"))
                        {
                            int afterEnd = lineStart + 4;
                            if (afterEnd >= source.Length || IsWhitespaceOrNewline(source[afterEnd]))
                            {
                                pos = SkipToEndOfLine(source, lineStart);
                                break;
                            }
                        }
                        pos = SkipToEndOfLine(source, pos);
                    }
                    commentsRemoved++;
                    TrimTrailingWhitespace(output);
                    lastTokenWasValue = false;
                    continue;
                }
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

            // Heredoc — <<IDENT, <<-IDENT, <<~IDENT, <<'IDENT', <<"IDENT"
            if (c == '<' && pos + 1 < source.Length && source[pos + 1] == '<')
            {
                int heredocStart = pos;
                int scan = pos + 2;
                bool indented = false;
                bool squiggly = false;

                if (scan < source.Length && source[scan] == '-') { indented = true; scan++; }
                else if (scan < source.Length && source[scan] == '~') { squiggly = true; scan++; }

                if (scan < source.Length)
                {
                    char q = source[scan];
                    string? delimiter = null;

                    if (q == '\'' || q == '"' || q == '`')
                    {
                        scan++;
                        int delimStart = scan;
                        while (scan < source.Length && source[scan] != q && source[scan] != '\n')
                            scan++;
                        if (scan < source.Length && source[scan] == q)
                        {
                            delimiter = source.Substring(delimStart, scan - delimStart);
                            scan++;
                        }
                    }
                    else if (IsIdentStart(q))
                    {
                        int delimStart = scan;
                        while (scan < source.Length && IsIdentChar(source[scan]))
                            scan++;
                        delimiter = source.Substring(delimStart, scan - delimStart);
                    }

                    if (delimiter != null)
                    {
                        int endOfOpeningLine = scan;
                        while (endOfOpeningLine < source.Length && source[endOfOpeningLine] != '\n')
                            endOfOpeningLine++;
                        if (endOfOpeningLine < source.Length)
                            endOfOpeningLine++;

                        output.Append(source, heredocStart, endOfOpeningLine - heredocStart);
                        pos = endOfOpeningLine;

                        while (pos < source.Length)
                        {
                            int lineStart = pos;
                            int lineEnd = pos;
                            while (lineEnd < source.Length && source[lineEnd] != '\n')
                                lineEnd++;
                            if (lineEnd < source.Length) lineEnd++;

                            string rawLine = source.Substring(lineStart, lineEnd - lineStart);
                            string trimmed = (indented || squiggly) ? rawLine.TrimStart() : rawLine;
                            string lineContent = trimmed.TrimEnd('\r', '\n');

                            output.Append(rawLine);
                            pos = lineEnd;

                            if (lineContent == delimiter)
                                break;
                        }

                        lastTokenWasValue = true;
                        continue;
                    }
                }

                output.Append(c);
                pos++;
                lastTokenWasValue = false;
                continue;
            }

            // Double-quoted string
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
                    if (sc == '"' || sc == '\n')
                        break;
                }
                lastTokenWasValue = true;
                continue;
            }

            // Single-quoted string — only \\ and \' are escapes
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
                    if (sc == '\'' || sc == '\n')
                        break;
                }
                lastTokenWasValue = true;
                continue;
            }

            // Regex literal /pattern/flags
            // Distinguido de divisão pelo contexto do token anterior.
            if (c == '/' && !lastTokenWasValue)
            {
                output.Append(c);
                pos++;
                while (pos < source.Length)
                {
                    char rc = source[pos];
                    if (rc == '\\' && pos + 1 < source.Length)
                    {
                        output.Append(rc);
                        pos++;
                        output.Append(source[pos]);
                        pos++;
                        continue;
                    }
                    if (rc == '\n')
                        break;
                    output.Append(rc);
                    pos++;
                    if (rc == '/')
                    {
                        // consume flags: i, m, x, o, u, e, s, n
                        while (pos < source.Length && IsRegexFlag(source[pos]))
                        {
                            output.Append(source[pos]);
                            pos++;
                        }
                        break;
                    }
                }
                lastTokenWasValue = true;
                continue;
            }

            // Track whether last meaningful token was a value
            if (char.IsLetterOrDigit(c) || c == '_')
            {
                output.Append(c);
                pos++;
                while (pos < source.Length && (char.IsLetterOrDigit(source[pos]) || source[pos] == '_'))
                {
                    output.Append(source[pos]);
                    pos++;
                }
                lastTokenWasValue = true;
                continue;
            }

            if (ValueEndChars.Contains(c))
            {
                output.Append(c);
                pos++;
                lastTokenWasValue = true;
                continue;
            }

            // Newlines reset: after newline, '/' is regex unless line ends with value
            if (c == '\n')
            {
                output.Append(c);
                pos++;
                lastTokenWasValue = false;
                continue;
            }

            if (!char.IsWhiteSpace(c))
                lastTokenWasValue = false;

            output.Append(c);
            pos++;
        }

        return new StripResult
        {
            CleanedSource = CollapseBlankLines(output.ToString()),
            CommentsRemoved = commentsRemoved,
        };
    }

    private static bool IsLineStart(string source, int pos) =>
        pos == 0 || source[pos - 1] == '\n';

    private static bool IsWhitespaceOrNewline(char c) =>
        c == ' ' || c == '\t' || c == '\n' || c == '\r';

    private static int SkipToEndOfLine(string source, int pos)
    {
        while (pos < source.Length && source[pos] != '\n')
            pos++;
        if (pos < source.Length) pos++;
        return pos;
    }

    private static bool MatchesAt(string source, int pos, string pattern)
    {
        if (pos + pattern.Length > source.Length) return false;
        for (int i = 0; i < pattern.Length; i++)
            if (source[pos + i] != pattern[i]) return false;
        return true;
    }

    private static bool IsIdentStart(char c) => char.IsLetter(c) || c == '_';
    private static bool IsIdentChar(char c) => char.IsLetterOrDigit(c) || c == '_';
    private static bool IsRegexFlag(char c) => "imxouesn".IndexOf(c) >= 0;
}