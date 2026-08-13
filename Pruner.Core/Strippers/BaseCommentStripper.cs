using System.Text;

namespace Pruner.Core.Strippers;

public abstract class BaseCommentStripper : ICommentStripper
{
    public abstract StripResult Strip(string source);

    protected static void TrimTrailingWhitespace(StringBuilder sb)
    {
        int i = sb.Length - 1;
        while (i >= 0 && (sb[i] == ' ' || sb[i] == '\t'))
            i--;
        if (i < sb.Length - 1)
            sb.Length = i + 1;
    }

    protected static string CollapseBlankLines(string source)
    {
        if (source.Length == 0)
            return string.Empty;

        bool hasCrlf = source.Contains("\r\n");
        string newline = hasCrlf ? "\r\n" : "\n";
        bool hadTrailingNewline = source.EndsWith("\n");
        string[] lines = source.Split('\n');
        var result = new StringBuilder(source.Length);
        int consecutiveBlank = 0;

        foreach (string rawLine in lines)
        {
            string line = rawLine.TrimEnd('\r');
            if (line.Trim().Length == 0)
            {
                consecutiveBlank++;
                if (consecutiveBlank <= 1)
                    result.Append(newline);
            }
            else
            {
                consecutiveBlank = 0;
                result.Append(line);
                result.Append(newline);
            }
        }

        string collapsed = result.ToString().TrimEnd('\r', '\n');
        return hadTrailingNewline ? collapsed + newline : collapsed;
    }
}