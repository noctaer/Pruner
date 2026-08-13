namespace LuaCleaner.Core.Strippers;

internal sealed class LuauLexer
{
    private readonly string _source;
    private int _pos;

    public LuauLexer(string source)
    {
        _source = source;
        _pos = 0;
    }

    public bool EndOfSource => _pos >= _source.Length;

    public char Current => _source[_pos];

    public char Peek(int offset = 1) =>
        (_pos + offset) < _source.Length ? _source[_pos + offset] : '\0';

    public char Consume()
    {
        char c = _source[_pos];
        _pos++;
        return c;
    }

    public string ConsumeShortString(char delimiter)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append(delimiter);

        while (!EndOfSource)
        {
            char c = Consume();

            if (c == '\\')
            {
                sb.Append(c);
                if (!EndOfSource)
                    sb.Append(Consume());
                continue;
            }

            sb.Append(c);

            if (c == delimiter)
                return sb.ToString();

            if (c == '\n' || c == '\r')
                return sb.ToString();
        }

        return sb.ToString();
    }

    public int PeekLongBracketLevel()
    {
        if (_pos >= _source.Length || _source[_pos] != '[')
            return -1;

        int level = 0;
        int lookahead = _pos + 1;

        while (lookahead < _source.Length && _source[lookahead] == '=')
        {
            level++;
            lookahead++;
        }

        if (lookahead < _source.Length && _source[lookahead] == '[')
            return level;

        return -1;
    }

    public string ConsumeLongString(int level)
    {
        var sb = new System.Text.StringBuilder();
        string closing = "]" + new string('=', level) + "]";

        sb.Append(Consume());
        for (int i = 0; i < level; i++)
            sb.Append(Consume());
        sb.Append(Consume());

        while (!EndOfSource)
        {
            if (MatchesAt(_pos, closing))
            {
                for (int i = 0; i < closing.Length; i++)
                    sb.Append(Consume());
                return sb.ToString();
            }

            sb.Append(Consume());
        }

        return sb.ToString();
    }

    public void ConsumeLongComment(int level)
    {
        string closing = "]" + new string('=', level) + "]";

        Consume();
        for (int i = 0; i < level; i++)
            Consume();
        Consume();

        while (!EndOfSource)
        {
            if (MatchesAt(_pos, closing))
            {
                for (int i = 0; i < closing.Length; i++)
                    Consume();
                return;
            }

            Consume();
        }
    }

    public string ConsumeSingleLineComment()
    {
        var sb = new System.Text.StringBuilder();

        while (!EndOfSource && Current != '\n' && Current != '\r')
            sb.Append(Consume());

        return sb.ToString();
    }

    private bool MatchesAt(int pos, string pattern)
    {
        if (pos + pattern.Length > _source.Length)
            return false;

        for (int i = 0; i < pattern.Length; i++)
        {
            if (_source[pos + i] != pattern[i])
                return false;
        }

        return true;
    }
}