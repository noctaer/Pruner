using System.Text;

namespace LuaCleaner.Core.Strippers;

public sealed class CStyleCommentStripper : BaseCommentStripper
{
    private static readonly System.Collections.Generic.HashSet<char> ValueEndChars =
        new() { ')', ']', '}' };

    private readonly bool _supportTemplateLiterals;

    public CStyleCommentStripper(bool supportTemplateLiterals)
    {
        _supportTemplateLiterals = supportTemplateLiterals;
    }

    public override StripResult Strip(string source)
    {
        var output = new StringBuilder(source.Length);
        int commentsRemoved = 0;
        int pos = 0;
        bool lastTokenWasValue = false;

        while (pos < source.Length)
        {
            char c = source[pos];

            if (_supportTemplateLiterals && c == '`')
            {
                output.Append(c);
                pos++;
                while (pos < source.Length)
                {
                    char tc = source[pos];
                    output.Append(tc);
                    pos++;
                    if (tc == '\\' && pos < source.Length)
                    {
                        output.Append(source[pos]);
                        pos++;
                        continue;
                    }
                    if (tc == '`')
                        break;
                }
                lastTokenWasValue = true;
                continue;
            }

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
                lastTokenWasValue = true;
                continue;
            }

            if (c == '/' && pos + 1 < source.Length && source[pos + 1] == '/')
            {
                pos += 2;
                while (pos < source.Length && source[pos] != '\n' && source[pos] != '\r')
                    pos++;
                commentsRemoved++;
                TrimTrailingWhitespace(output);
                lastTokenWasValue = false;
                continue;
            }

            if (c == '/' && pos + 1 < source.Length && source[pos + 1] == '*')
            {
                pos += 2;
                while (pos + 1 < source.Length && !(source[pos] == '*' && source[pos + 1] == '/'))
                    pos++;
                if (pos + 1 < source.Length)
                    pos += 2;
                commentsRemoved++;
                TrimTrailingWhitespace(output);
                lastTokenWasValue = false;
                continue;
            }

            if (_supportTemplateLiterals && c == '/' && !lastTokenWasValue)
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

            if (char.IsLetter(c) || c == '_' || c == '$')
            {
                output.Append(c);
                pos++;
                while (pos < source.Length && (char.IsLetterOrDigit(source[pos]) || source[pos] == '_' || source[pos] == '$'))
                {
                    output.Append(source[pos]);
                    pos++;
                }
                lastTokenWasValue = true;
                continue;
            }

            if (char.IsDigit(c))
            {
                output.Append(c);
                pos++;
                while (pos < source.Length && (char.IsDigit(source[pos]) || source[pos] == '.' || source[pos] == '_'))
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

    private static bool IsRegexFlag(char c) => "gimsuy".IndexOf(c) >= 0;
}