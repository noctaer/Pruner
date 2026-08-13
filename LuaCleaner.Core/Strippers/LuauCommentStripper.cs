using System.Text;

namespace LuaCleaner.Core.Strippers;

public sealed class LuauCommentStripper : BaseCommentStripper
{
    public override StripResult Strip(string source)
    {
        var lexer = new LuauLexer(source);
        var output = new StringBuilder(source.Length);
        int commentsRemoved = 0;

        while (!lexer.EndOfSource)
        {
            char c = lexer.Current;

            if (c == '"' || c == '\'')
            {
                lexer.Consume();
                output.Append(lexer.ConsumeShortString(c));
                continue;
            }

            if (c == '[')
            {
                int level = lexer.PeekLongBracketLevel();
                if (level >= 0)
                {
                    output.Append(lexer.ConsumeLongString(level));
                    continue;
                }

                output.Append(lexer.Consume());
                continue;
            }

            if (c == '-' && lexer.Peek() == '-')
            {
                lexer.Consume();
                lexer.Consume();

                int level = lexer.PeekLongBracketLevel();
                if (level >= 0)
                {
                    lexer.ConsumeLongComment(level);
                    commentsRemoved++;
                    TrimTrailingWhitespace(output);
                }
                else
                {
                    lexer.ConsumeSingleLineComment();
                    commentsRemoved++;
                    TrimTrailingWhitespace(output);
                }

                continue;
            }

            output.Append(lexer.Consume());
        }

        return new StripResult
        {
            CleanedSource = CollapseBlankLines(output.ToString()),
            CommentsRemoved = commentsRemoved,
        };
    }
}