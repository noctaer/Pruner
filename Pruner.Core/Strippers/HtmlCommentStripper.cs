using System.Text;

namespace Pruner.Core.Strippers;

public sealed class HtmlCommentStripper : BaseCommentStripper
{
    public override StripResult Strip(string source)
    {
        var output = new StringBuilder(source.Length);
        int commentsRemoved = 0;
        int pos = 0;

        while (pos < source.Length)
        {
            // HTML comment <!-- -->
            if (pos + 3 < source.Length &&
                source[pos] == '<' && source[pos + 1] == '!' &&
                source[pos + 2] == '-' && source[pos + 3] == '-')
            {
                pos += 4;
                while (pos + 2 < source.Length &&
                       !(source[pos] == '-' && source[pos + 1] == '-' && source[pos + 2] == '>'))
                    pos++;
                if (pos + 2 < source.Length)
                    pos += 3;
                commentsRemoved++;
                TrimTrailingWhitespace(output);
                continue;
            }

            // Script block — delegate to CStyle stripper logic inline
            if (pos + 7 < source.Length &&
                string.Compare(source, pos, "<script", 0, 7, StringComparison.OrdinalIgnoreCase) == 0)
            {
                int scriptStart = pos;
                while (pos < source.Length && source[pos] != '>') pos++;
                if (pos < source.Length) pos++;
                output.Append(source, scriptStart, pos - scriptStart);

                int bodyStart = pos;
                int closeTag = source.IndexOf("</script", pos, StringComparison.OrdinalIgnoreCase);
                if (closeTag < 0) closeTag = source.Length;

                string scriptBody = source.Substring(bodyStart, closeTag - bodyStart);
                var scriptStripper = new CStyleCommentStripper(supportTemplateLiterals: true);
                StripResult scriptResult = scriptStripper.Strip(scriptBody);
                output.Append(scriptResult.CleanedSource.TrimEnd('\n'));
                commentsRemoved += scriptResult.CommentsRemoved;
                pos = closeTag;
                continue;
            }

            // Style block — delegate to CSS stripper logic inline
            if (pos + 6 < source.Length &&
                string.Compare(source, pos, "<style", 0, 6, StringComparison.OrdinalIgnoreCase) == 0)
            {
                int styleStart = pos;
                while (pos < source.Length && source[pos] != '>') pos++;
                if (pos < source.Length) pos++;
                output.Append(source, styleStart, pos - styleStart);

                int bodyStart = pos;
                int closeTag = source.IndexOf("</style", pos, StringComparison.OrdinalIgnoreCase);
                if (closeTag < 0) closeTag = source.Length;

                string styleBody = source.Substring(bodyStart, closeTag - bodyStart);
                var cssStripper = new CssCommentStripper();
                StripResult cssResult = cssStripper.Strip(styleBody);
                output.Append(cssResult.CleanedSource.TrimEnd('\n'));
                commentsRemoved += cssResult.CommentsRemoved;
                pos = closeTag;
                continue;
            }

            // Attribute strings " and '
            if (source[pos] == '"' || source[pos] == '\'')
            {
                char q = source[pos];
                output.Append(q);
                pos++;
                while (pos < source.Length)
                {
                    char sc = source[pos];
                    output.Append(sc);
                    pos++;
                    if (sc == q) break;
                }
                continue;
            }

            output.Append(source[pos]);
            pos++;
        }

        return new StripResult
        {
            CleanedSource = CollapseBlankLines(output.ToString()),
            CommentsRemoved = commentsRemoved,
        };
    }
}