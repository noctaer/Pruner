using System.Text;

namespace Pruner.Core.Strippers;

public sealed class RustCommentStripper : BaseCommentStripper
{
    public override StripResult Strip(string source)
    {
        var output = new StringBuilder(source.Length);
        int commentsRemoved = 0;
        int pos = 0;

        while (pos < source.Length)
        {
            char c = source[pos];

            // Line comments: //, ///, //!
            if (c == '/' && pos + 1 < source.Length && source[pos + 1] == '/')
            {
                pos += 2;
                while (pos < source.Length && source[pos] != '\n' && source[pos] != '\r')
                    pos++;
                commentsRemoved++;
                TrimTrailingWhitespace(output);
                continue;
            }

            // Block comments: /* */ — Rust supports nested block comments
            if (c == '/' && pos + 1 < source.Length && source[pos + 1] == '*')
            {
                pos += 2;
                int depth = 1;
                while (pos + 1 < source.Length && depth > 0)
                {
                    if (source[pos] == '/' && source[pos + 1] == '*') { depth++; pos += 2; continue; }
                    if (source[pos] == '*' && source[pos + 1] == '/') { depth--; pos += 2; continue; }
                    pos++;
                }
                commentsRemoved++;
                TrimTrailingWhitespace(output);
                continue;
            }

            // Raw strings: r"..." r#"..."# r##"..."##
            if (c == 'r' && pos + 1 < source.Length && (source[pos + 1] == '"' || source[pos + 1] == '#'))
            {
                int hashes = 0;
                int scan = pos + 1;
                while (scan < source.Length && source[scan] == '#') { hashes++; scan++; }

                if (scan < source.Length && source[scan] == '"')
                {
                    string closing = "\"" + new string('#', hashes);
                    output.Append(source, pos, scan - pos + 1);
                    pos = scan + 1;
                    while (pos < source.Length)
                    {
                        if (pos + closing.Length <= source.Length &&
                            source.Substring(pos, closing.Length) == closing)
                        {
                            output.Append(closing);
                            pos += closing.Length;
                            break;
                        }
                        output.Append(source[pos]);
                        pos++;
                    }
                    continue;
                }
            }

            // Byte strings: b"..." b'...'
            if (c == 'b' && pos + 1 < source.Length && (source[pos + 1] == '"' || source[pos + 1] == '\''))
            {
                output.Append(c);
                pos++;
                c = source[pos];
            }

            // Regular strings " and '
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