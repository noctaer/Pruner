using LuaCleaner.Core;
using LuaCleaner.Core.Strippers;
using Xunit;

namespace LuaCleaner.Tests;

public sealed class LuauStripperTests
{
    private static readonly ICommentStripper _luau = CommentStripperFactory.Get(Language.Luau);

    private static string Strip(string source) => _luau.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _luau.Strip(source).CommentsRemoved;

    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "-- hello\nlocal x = 1";
        string result = Strip(input);
        Assert.DoesNotContain("-- hello", result);
        Assert.Contains("local x = 1", result);
    }

    [Fact] public void InlineComment_IsRemovedAndCodePreserved()
    {
        string input = "local x = 1 -- hello\nlocal y = 2";
        string result = Strip(input);
        Assert.Contains("local x = 1", result);
        Assert.DoesNotContain("-- hello", result);
        Assert.Contains("local y = 2", result);
    }

    [Fact] public void InlineComment_TrailingWhitespaceRemoved()
    {
        string input = "local x = 1 -- comment\n";
        string result = Strip(input);
        Assert.StartsWith("local x = 1", result);
        Assert.DoesNotContain("1 \n", result);
        Assert.DoesNotContain("1\t\n", result);
    }

    [Fact] public void LongComment_IsRemoved()
    {
        string input = "--[[\nhello\nworld\n]]\nlocal x = 1";
        string result = Strip(input);
        Assert.DoesNotContain("hello", result);
        Assert.DoesNotContain("world", result);
        Assert.Contains("local x = 1", result);
    }

    [Fact] public void LongCommentWithLevel_IsRemoved()
    {
        string input = "--[==[\nhello\n]==]\nlocal x = 1";
        string result = Strip(input);
        Assert.DoesNotContain("hello", result);
        Assert.Contains("local x = 1", result);
    }

    [Fact] public void StringContainingDoubleDash_IsPreserved()
    {
        string input = "local x = \"-- hello\"";
        string result = Strip(input);
        Assert.Equal(input, result);
    }

    [Fact] public void SingleQuoteStringContainingDoubleDash_IsPreserved()
    {
        string input = "local x = '-- hello'";
        string result = Strip(input);
        Assert.Equal(input, result);
    }

    [Fact] public void LongStringContainingDoubleDash_IsPreserved()
    {
        string input = "local x = [[\n-- hello\n]]";
        string result = Strip(input);
        Assert.Contains("-- hello", result);
    }

    [Fact] public void LongStringContainingDoubleDashNotTreatedAsComment()
    {
        string input = "local e = [[--]]";
        string result = Strip(input);
        Assert.Equal(input, result);
    }

    [Fact] public void MixedContent_CorrectlyProcessed()
    {
        string input =
            "-- comment\n" +
            "local x = \"-- not comment\" -- comment\n" +
            "local y = [[\n-- not comment\n]]\n" +
            "--[[\ncomment\n]]\n" +
            "local z = 10";
        string result = Strip(input);
        Assert.DoesNotContain("-- comment\n", result);
        Assert.DoesNotContain("--[[\ncomment\n]]", result);
        Assert.Contains("\"-- not comment\"", result);
        Assert.Contains("-- not comment", result);
        Assert.Contains("local y = [[", result);
        Assert.Contains("local z = 10", result);
    }

    [Fact] public void Banner_MultiLineComment_IsRemoved()
    {
        string input =
            "--[[\n=========================================\nAUTO GOAL\n=========================================\n]]\nlocal x = 1";
        string result = Strip(input);
        Assert.DoesNotContain("AUTO GOAL", result);
        Assert.Contains("local x = 1", result);
    }

    [Fact] public void Banner_RepeatedSingleLineComments_AreRemoved()
    {
        string input = "-- ================\n-- TARGET SOLVER\n-- ================\nlocal x = 1";
        string result = Strip(input);
        Assert.DoesNotContain("TARGET SOLVER", result);
        Assert.Contains("local x = 1", result);
    }

    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }

    [Fact] public void OnlyComments_ReturnsEmpty()
    {
        string input = "-- comment 1\n-- comment 2\n";
        Assert.DoesNotContain("comment", Strip(input));
    }

    [Fact] public void CommentAtEndOfFile_IsRemoved()
    {
        string input = "local x = 1\n-- end comment";
        string result = Strip(input);
        Assert.Contains("local x = 1", result);
        Assert.DoesNotContain("end comment", result);
    }

    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "-- a\nlocal x = 1 -- b\n--[[\nc\n]]\n";
        Assert.Equal(3, CountRemoved(input));
    }

    [Fact] public void EscapedQuoteInsideString_DoesNotBreakLexer()
    {
        string input = "local x = \"he said \\\"--\\\" here\"\n-- comment";
        string result = Strip(input);
        Assert.Contains("\\\"--\\\"", result);
        Assert.DoesNotContain("-- comment", result);
    }

    [Fact] public void DoubleDashInStringLiterals_AllPreserved()
    {
        string input = "local a = \"--\"\nlocal b = \"---\"\nlocal c = \"--[[\"\nlocal d = \"]]\"";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void Utf8Characters_ArePreserved()
    {
        string input = "local nome = \"João\"\nlocal texto = \"Olá mundo\"\n-- comentário";
        string result = Strip(input);
        Assert.Contains("João", result);
        Assert.Contains("Olá mundo", result);
        Assert.DoesNotContain("comentário", result);
    }

    [Fact] public void LongComment_MismatchedLevel_NotTreatedAsLongComment()
    {
        string input = "--[ this is still a single-line comment\nlocal x = 1";
        string result = Strip(input);
        Assert.DoesNotContain("single-line comment", result);
        Assert.Contains("local x = 1", result);
    }

    [Fact] public void DashFollowedByNonDash_IsPreserved()
    {
        string input = "local x = a - b";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void LongString_WithLevel_IsPreserved()
    {
        string input = "local x = [==[hello]==]";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void CodeIsNotModifiedBeyondCommentRemoval()
    {
        string input = "local Players = game:GetService(\"Players\")\nlocal Player = Players.LocalPlayer";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void CrlfPreserved()
    {
        string input = "local x = 1\r\n-- comment\r\nlocal y = 2\r\n";
        string result = Strip(input);
        Assert.Contains("\r\n", result);
        Assert.Contains("local x = 1", result);
        Assert.Contains("local y = 2", result);
        Assert.DoesNotContain("comment", result);
    }

    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "local x = 1\nlocal y = x + 2\nreturn y\n";
        Assert.Equal(input, Strip(input));
    }
}

public sealed class PythonStripperTests
{
    private static readonly ICommentStripper _python = CommentStripperFactory.Get(Language.Python);

    private static string Strip(string source) => _python.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _python.Strip(source).CommentsRemoved;

    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "# hello\nx = 1";
        string result = Strip(input);
        Assert.DoesNotContain("# hello", result);
        Assert.Contains("x = 1", result);
    }

    [Fact] public void InlineComment_IsRemoved()
    {
        string input = "x = 1 # inline\ny = 2";
        string result = Strip(input);
        Assert.Contains("x = 1", result);
        Assert.DoesNotContain("# inline", result);
        Assert.Contains("y = 2", result);
    }

    [Fact] public void HashInsideDoubleQuotedString_IsPreserved()
    {
        string input = "x = \"hello # world\"";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void HashInsideSingleQuotedString_IsPreserved()
    {
        string input = "x = 'hello # world'";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void TripleDoubleQuotedString_IsPreserved()
    {
        string input = "x = \"\"\"hello\n# not a comment\nworld\"\"\"";
        string result = Strip(input);
        Assert.Contains("# not a comment", result);
    }

    [Fact] public void TripleSingleQuotedString_IsPreserved()
    {
        string input = "x = '''hello\n# not a comment\nworld'''";
        string result = Strip(input);
        Assert.Contains("# not a comment", result);
    }

    [Fact] public void Docstring_IsPreserved()
    {
        string input = "def foo():\n    \"\"\"This is a docstring.\"\"\"\n    pass";
        string result = Strip(input);
        Assert.Contains("This is a docstring.", result);
    }

    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }

    [Fact] public void OnlyComments_ReturnsEmpty()
    {
        string input = "# comment 1\n# comment 2\n";
        Assert.DoesNotContain("comment", Strip(input));
    }

    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "# a\nx = 1 # b\n# c\n";
        Assert.Equal(3, CountRemoved(input));
    }

    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "x = 1\ny = x + 2\nreturn y\n";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void TrailingWhitespaceAfterInlineComment_IsRemoved()
    {
        string input = "x = 1 # comment\n";
        string result = Strip(input);
        Assert.StartsWith("x = 1", result);
        Assert.DoesNotContain("x = 1 \n", result);
    }
}

public sealed class JavaScriptStripperTests
{
    private static readonly ICommentStripper _js = CommentStripperFactory.Get(Language.JavaScript);

    private static string Strip(string source) => _js.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _js.Strip(source).CommentsRemoved;

    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "// hello\nconst x = 1;";
        string result = Strip(input);
        Assert.DoesNotContain("// hello", result);
        Assert.Contains("const x = 1;", result);
    }

    [Fact] public void InlineComment_IsRemoved()
    {
        string input = "const x = 1; // inline\nconst y = 2;";
        string result = Strip(input);
        Assert.Contains("const x = 1;", result);
        Assert.DoesNotContain("// inline", result);
        Assert.Contains("const y = 2;", result);
    }

    [Fact] public void BlockComment_IsRemoved()
    {
        string input = "/* block\ncomment */\nconst x = 1;";
        string result = Strip(input);
        Assert.DoesNotContain("block", result);
        Assert.Contains("const x = 1;", result);
    }

    [Fact] public void CommentInsideString_IsPreserved()
    {
        string input = "const x = \"// not a comment\";";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void CommentInsideSingleQuoteString_IsPreserved()
    {
        string input = "const x = '// not a comment';";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void CommentInsideTemplateLiteral_IsPreserved()
    {
        string input = "const x = `hello // world`;";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void RegexLiteral_IsPreserved()
    {
        string input = "const r = /#\\w+/g;";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void RegexLiteralWithSlash_IsPreserved()
    {
        string input = "const r = /https:\\/\\//;";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }

    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "// a\nconst x = 1; // b\n/* c */\n";
        Assert.Equal(3, CountRemoved(input));
    }

    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "const x = 1;\nconst y = x + 2;\n";
        Assert.Equal(input, Strip(input));
    }
}

public sealed class CSharpStripperTests
{
    private static readonly ICommentStripper _cs = CommentStripperFactory.Get(Language.CSharp);

    private static string Strip(string source) => _cs.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _cs.Strip(source).CommentsRemoved;

    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "// hello\nint x = 1;";
        string result = Strip(input);
        Assert.DoesNotContain("// hello", result);
        Assert.Contains("int x = 1;", result);
    }

    [Fact] public void XmlDocComment_IsRemoved()
    {
        string input = "/// <summary>docs</summary>\npublic void Foo() {}";
        string result = Strip(input);
        Assert.DoesNotContain("summary", result);
        Assert.Contains("public void Foo()", result);
    }

    [Fact] public void BlockComment_IsRemoved()
    {
        string input = "/* block */\nint x = 1;";
        string result = Strip(input);
        Assert.DoesNotContain("block", result);
        Assert.Contains("int x = 1;", result);
    }

    [Fact] public void CommentInsideString_IsPreserved()
    {
        string input = "var x = \"// not a comment\";";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void VerbatimString_WithComment_IsPreserved()
    {
        string input = "var x = @\"hello // world\";";
        string result = Strip(input);
        Assert.Contains("// world", result);
    }

    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }

    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "// a\nint x = 1; // b\n/* c */\n";
        Assert.Equal(3, CountRemoved(input));
    }

    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "int x = 1;\nint y = x + 2;\n";
        Assert.Equal(input, Strip(input));
    }
}

public sealed class SqlStripperTests
{
    private static readonly ICommentStripper _sql = CommentStripperFactory.Get(Language.Sql);

    private static string Strip(string source) => _sql.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _sql.Strip(source).CommentsRemoved;

    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "-- hello\nSELECT 1;";
        string result = Strip(input);
        Assert.DoesNotContain("-- hello", result);
        Assert.Contains("SELECT 1;", result);
    }

    [Fact] public void InlineComment_IsRemoved()
    {
        string input = "SELECT 1 -- inline\nFROM t;";
        string result = Strip(input);
        Assert.Contains("SELECT 1", result);
        Assert.DoesNotContain("-- inline", result);
        Assert.Contains("FROM t;", result);
    }

    [Fact] public void BlockComment_IsRemoved()
    {
        string input = "/* block */\nSELECT 1;";
        string result = Strip(input);
        Assert.DoesNotContain("block", result);
        Assert.Contains("SELECT 1;", result);
    }

    [Fact] public void CommentInsideString_IsPreserved()
    {
        string input = "SELECT '-- not a comment';";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void EscapedSingleQuoteInString_IsPreserved()
    {
        string input = "SELECT 'it''s fine -- not a comment';";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }

    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "-- a\nSELECT 1; -- b\n/* c */\n";
        Assert.Equal(3, CountRemoved(input));
    }

    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "SELECT id, name\nFROM users\nWHERE active = 1;\n";
        Assert.Equal(input, Strip(input));
    }
}

public sealed class RubyStripperTests
{
    private static readonly ICommentStripper _ruby = CommentStripperFactory.Get(Language.Ruby);

    private static string Strip(string source) => _ruby.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _ruby.Strip(source).CommentsRemoved;

    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "# hello\nx = 1";
        string result = Strip(input);
        Assert.DoesNotContain("# hello", result);
        Assert.Contains("x = 1", result);
    }

    [Fact] public void InlineComment_IsRemoved()
    {
        string input = "x = 1 # inline\ny = 2";
        string result = Strip(input);
        Assert.Contains("x = 1", result);
        Assert.DoesNotContain("# inline", result);
        Assert.Contains("y = 2", result);
    }

    [Fact] public void HashInsideDoubleQuotedString_IsPreserved()
    {
        string input = "x = \"hello # world\"";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void HashInsideSingleQuotedString_IsPreserved()
    {
        string input = "x = 'hello # world'";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void Interpolation_IsPreserved()
    {
        string input = "x = \"hello #{name}\"";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void BeginEndBlockComment_IsRemoved()
    {
        string input = "=begin\nhello\nworld\n=end\nx = 1";
        string result = Strip(input);
        Assert.DoesNotContain("hello", result);
        Assert.DoesNotContain("world", result);
        Assert.Contains("x = 1", result);
    }

    [Fact] public void RegexLiteral_IsPreserved()
    {
        string input = "r = /#[a-z]+/i";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void RegexLiteralAfterAssignment_IsPreserved()
    {
        string input = "pattern = /#\\w+/";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }

    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "# a\nx = 1 # b\n=begin\nc\n=end\n";
        Assert.Equal(3, CountRemoved(input));
    }

    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "x = 1\ny = x + 2\n";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void TrailingWhitespaceAfterInlineComment_IsRemoved()
    {
        string input = "x = 1 # comment\n";
        string result = Strip(input);
        Assert.StartsWith("x = 1", result);
        Assert.DoesNotContain("x = 1 \n", result);
    }
}

public sealed class GoStripperTests
{
    private static readonly ICommentStripper _go = CommentStripperFactory.Get(Language.Go);

    private static string Strip(string source) => _go.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _go.Strip(source).CommentsRemoved;

    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "// hello\nvar x = 1";
        string result = Strip(input);
        Assert.DoesNotContain("// hello", result);
        Assert.Contains("var x = 1", result);
    }

    [Fact] public void InlineComment_IsRemoved()
    {
        string input = "var x = 1 // inline\nvar y = 2";
        string result = Strip(input);
        Assert.Contains("var x = 1", result);
        Assert.DoesNotContain("// inline", result);
        Assert.Contains("var y = 2", result);
    }

    [Fact] public void BlockComment_IsRemoved()
    {
        string input = "/* block\ncomment */\nvar x = 1";
        string result = Strip(input);
        Assert.DoesNotContain("block", result);
        Assert.Contains("var x = 1", result);
    }

    [Fact] public void CommentInsideString_IsPreserved()
    {
        string input = "var x = \"// not a comment\"";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }

    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "// a\nvar x = 1 // b\n/* c */\n";
        Assert.Equal(3, CountRemoved(input));
    }

    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "var x = 1\nvar y = x + 2\n";
        Assert.Equal(input, Strip(input));
    }
}

public sealed class KotlinStripperTests
{
    private static readonly ICommentStripper _kotlin = CommentStripperFactory.Get(Language.Kotlin);

    private static string Strip(string source) => _kotlin.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _kotlin.Strip(source).CommentsRemoved;

    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "// hello\nval x = 1";
        string result = Strip(input);
        Assert.DoesNotContain("// hello", result);
        Assert.Contains("val x = 1", result);
    }

    [Fact] public void BlockComment_IsRemoved()
    {
        string input = "/* block */\nval x = 1";
        string result = Strip(input);
        Assert.DoesNotContain("block", result);
        Assert.Contains("val x = 1", result);
    }

    [Fact] public void CommentInsideString_IsPreserved()
    {
        string input = "val x = \"// not a comment\"";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }

    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "// a\nval x = 1 // b\n/* c */\n";
        Assert.Equal(3, CountRemoved(input));
    }

    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "val x = 1\nval y = x + 2\n";
        Assert.Equal(input, Strip(input));
    }
}

public sealed class SwiftStripperTests
{
    private static readonly ICommentStripper _swift = CommentStripperFactory.Get(Language.Swift);

    private static string Strip(string source) => _swift.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _swift.Strip(source).CommentsRemoved;

    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "// hello\nlet x = 1";
        string result = Strip(input);
        Assert.DoesNotContain("// hello", result);
        Assert.Contains("let x = 1", result);
    }

    [Fact] public void BlockComment_IsRemoved()
    {
        string input = "/* block */\nlet x = 1";
        string result = Strip(input);
        Assert.DoesNotContain("block", result);
        Assert.Contains("let x = 1", result);
    }

    [Fact] public void CommentInsideString_IsPreserved()
    {
        string input = "let x = \"// not a comment\"";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }

    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "// a\nlet x = 1 // b\n/* c */\n";
        Assert.Equal(3, CountRemoved(input));
    }

    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "let x = 1\nlet y = x + 2\n";
        Assert.Equal(input, Strip(input));
    }
}

public sealed class BashStripperTests
{
    private static readonly ICommentStripper _bash = CommentStripperFactory.Get(Language.Bash);

    private static string Strip(string source) => _bash.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _bash.Strip(source).CommentsRemoved;

    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "# hello\nx=1";
        string result = Strip(input);
        Assert.DoesNotContain("# hello", result);
        Assert.Contains("x=1", result);
    }

    [Fact] public void InlineComment_IsRemoved()
    {
        string input = "x=1 # inline\ny=2";
        string result = Strip(input);
        Assert.Contains("x=1", result);
        Assert.DoesNotContain("# inline", result);
        Assert.Contains("y=2", result);
    }

    [Fact] public void HashInsideString_IsPreserved()
    {
        string input = "x=\"hello # world\"";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void Shebang_IsRemoved()
    {
        string input = "#!/usr/bin/env bash\necho hello";
        string result = Strip(input);
        Assert.DoesNotContain("#!/usr/bin/env bash", result);
        Assert.Contains("echo hello", result);
    }

    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }

    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "# a\nx=1 # b\n# c\n";
        Assert.Equal(3, CountRemoved(input));
    }

    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "x=1\ny=2\n";
        Assert.Equal(input, Strip(input));
    }
}

public sealed class RustStripperTests
{
    private static readonly ICommentStripper _rust = CommentStripperFactory.Get(Language.Rust);

    private static string Strip(string source) => _rust.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _rust.Strip(source).CommentsRemoved;

    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "// hello\nlet x = 1;";
        string result = Strip(input);
        Assert.DoesNotContain("// hello", result);
        Assert.Contains("let x = 1;", result);
    }

    [Fact] public void DocComment_IsRemoved()
    {
        string input = "/// doc comment\nfn foo() {}";
        string result = Strip(input);
        Assert.DoesNotContain("doc comment", result);
        Assert.Contains("fn foo()", result);
    }

    [Fact] public void InnerDocComment_IsRemoved()
    {
        string input = "//! inner doc\nfn foo() {}";
        string result = Strip(input);
        Assert.DoesNotContain("inner doc", result);
        Assert.Contains("fn foo()", result);
    }

    [Fact] public void BlockComment_IsRemoved()
    {
        string input = "/* block */\nlet x = 1;";
        string result = Strip(input);
        Assert.DoesNotContain("block", result);
        Assert.Contains("let x = 1;", result);
    }

    [Fact] public void NestedBlockComment_IsRemoved()
    {
        string input = "/* outer /* inner */ still comment */\nlet x = 1;";
        string result = Strip(input);
        Assert.DoesNotContain("outer", result);
        Assert.DoesNotContain("inner", result);
        Assert.Contains("let x = 1;", result);
    }

    [Fact] public void CommentInsideString_IsPreserved()
    {
        string input = "let x = \"// not a comment\";";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void RawString_IsPreserved()
    {
        string input = "let x = r#\"// not a comment\"#;";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }

    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "// a\nlet x = 1; // b\n/* c */\n";
        Assert.Equal(3, CountRemoved(input));
    }

    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "let x = 1;\nlet y = x + 2;\n";
        Assert.Equal(input, Strip(input));
    }
}

public sealed class HtmlStripperTests
{
    private static readonly ICommentStripper _html = CommentStripperFactory.Get(Language.Html);

    private static string Strip(string source) => _html.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _html.Strip(source).CommentsRemoved;

    [Fact] public void HtmlComment_IsRemoved()
    {
        string input = "<!-- hello -->\n<p>text</p>";
        string result = Strip(input);
        Assert.DoesNotContain("hello", result);
        Assert.Contains("<p>text</p>", result);
    }

    [Fact] public void MultilineHtmlComment_IsRemoved()
    {
        string input = "<!--\nhello\nworld\n-->\n<p>text</p>";
        string result = Strip(input);
        Assert.DoesNotContain("hello", result);
        Assert.DoesNotContain("world", result);
        Assert.Contains("<p>text</p>", result);
    }

    [Fact] public void InlineHtmlComment_IsRemoved()
    {
        string input = "<p>text</p><!-- comment --><span>more</span>";
        string result = Strip(input);
        Assert.DoesNotContain("comment", result);
        Assert.Contains("<p>text</p>", result);
        Assert.Contains("<span>more</span>", result);
    }

    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }

    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "<!-- a -->\n<p>x</p><!-- b -->\n";
        Assert.Equal(2, CountRemoved(input));
    }

    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "<div>\n  <p>hello</p>\n</div>\n";
        Assert.Equal(input, Strip(input));
    }
}

public sealed class CssStripperTests
{
    private static readonly ICommentStripper _css = CommentStripperFactory.Get(Language.Css);

    private static string Strip(string source) => _css.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _css.Strip(source).CommentsRemoved;

    [Fact] public void BlockComment_IsRemoved()
    {
        string input = "/* comment */\n.foo { color: red; }";
        string result = Strip(input);
        Assert.DoesNotContain("comment", result);
        Assert.Contains(".foo { color: red; }", result);
    }

    [Fact] public void InlineBlockComment_IsRemoved()
    {
        string input = ".foo { color: /* red */ blue; }";
        string result = Strip(input);
        Assert.DoesNotContain("red", result);
        Assert.Contains("blue", result);
    }

    [Fact] public void CommentInsideString_IsPreserved()
    {
        string input = ".foo { content: \"/* not a comment */\"; }";
        string result = Strip(input);
        Assert.Contains("/* not a comment */", result);
    }

    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }

    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "/* a */\n.foo { /* b */ color: red; }\n";
        Assert.Equal(2, CountRemoved(input));
    }

    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = ".foo {\n  color: red;\n  margin: 0;\n}\n";
        Assert.Equal(input, Strip(input));
    }
}

public sealed class PhpStripperTests
{
    private static readonly ICommentStripper _php = CommentStripperFactory.Get(Language.Php);

    private static string Strip(string source) => _php.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _php.Strip(source).CommentsRemoved;

    [Fact] public void SingleLineSlashComment_IsRemoved()
    {
        string input = "// hello\n$x = 1;";
        string result = Strip(input);
        Assert.DoesNotContain("// hello", result);
        Assert.Contains("$x = 1;", result);
    }

    [Fact] public void SingleLineHashComment_IsRemoved()
    {
        string input = "# hello\n$x = 1;";
        string result = Strip(input);
        Assert.DoesNotContain("# hello", result);
        Assert.Contains("$x = 1;", result);
    }

    [Fact] public void BlockComment_IsRemoved()
    {
        string input = "/* block */\n$x = 1;";
        string result = Strip(input);
        Assert.DoesNotContain("block", result);
        Assert.Contains("$x = 1;", result);
    }

    [Fact] public void CommentInsideDoubleQuotedString_IsPreserved()
    {
        string input = "$x = \"// not a comment\";";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void CommentInsideSingleQuotedString_IsPreserved()
    {
        string input = "$x = '# not a comment';";
        Assert.Equal(input, Strip(input));
    }

    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }

    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "// a\n$x = 1; # b\n/* c */\n";
        Assert.Equal(3, CountRemoved(input));
    }

    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "$x = 1;\n$y = $x + 2;\n";
        Assert.Equal(input, Strip(input));
    }
}

public sealed class JavaStripperTests
{
    private static readonly ICommentStripper _java = CommentStripperFactory.Get(Language.Java);
    private static string Strip(string source) => _java.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _java.Strip(source).CommentsRemoved;
    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "// comment\nint x = 1;";
        Assert.DoesNotContain("// comment", Strip(input));
        Assert.Contains("int x = 1;", Strip(input));
    }
    [Fact] public void BlockComment_IsRemoved()
    {
        string input = "/* block */\nint x = 1;";
        Assert.DoesNotContain("block", Strip(input));
    }
    [Fact] public void CommentInsideString_IsPreserved()
    {
        string input = "String s = \"// not a comment\";";
        Assert.Equal(input, Strip(input));
    }
    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }
    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "// a\nint x = 1; /* b */\n";
        Assert.Equal(2, CountRemoved(input));
    }
    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "int x = 1;\nint y = 2;\n";
        Assert.Equal(input, Strip(input));
    }
}
public sealed class CStripperTests
{
    private static readonly ICommentStripper _c = CommentStripperFactory.Get(Language.C);
    private static string Strip(string source) => _c.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _c.Strip(source).CommentsRemoved;
    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "// comment\nint x = 1;";
        Assert.DoesNotContain("// comment", Strip(input));
    }
    [Fact] public void BlockComment_IsRemoved()
    {
        string input = "/* block */\nint x = 1;";
        Assert.DoesNotContain("block", Strip(input));
    }
    [Fact] public void CommentInsideString_IsPreserved()
    {
        string input = "char *s = \"// not a comment\";";
        Assert.Equal(input, Strip(input));
    }
    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }
    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "// a\nint x; /* b */\n";
        Assert.Equal(2, CountRemoved(input));
    }
    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "int x = 1;\nint y = 2;\n";
        Assert.Equal(input, Strip(input));
    }
}
public sealed class CppStripperTests
{
    private static readonly ICommentStripper _cpp = CommentStripperFactory.Get(Language.Cpp);
    private static string Strip(string source) => _cpp.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _cpp.Strip(source).CommentsRemoved;
    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "// comment\nint x = 1;";
        Assert.DoesNotContain("// comment", Strip(input));
    }
    [Fact] public void BlockComment_IsRemoved()
    {
        string input = "/* block */\nint x = 1;";
        Assert.DoesNotContain("block", Strip(input));
    }
    [Fact] public void CommentInsideString_IsPreserved()
    {
        string input = "std::string s = \"// not a comment\";";
        Assert.Equal(input, Strip(input));
    }
    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }
    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "// a\nauto x = 1; /* b */\n";
        Assert.Equal(2, CountRemoved(input));
    }
    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "int x = 1;\nint y = 2;\n";
        Assert.Equal(input, Strip(input));
    }
}
public sealed class DartStripperTests
{
    private static readonly ICommentStripper _dart = CommentStripperFactory.Get(Language.Dart);
    private static string Strip(string source) => _dart.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _dart.Strip(source).CommentsRemoved;
    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "// comment\nvar x = 1;";
        Assert.DoesNotContain("// comment", Strip(input));
    }
    [Fact] public void BlockComment_IsRemoved()
    {
        string input = "/* block */\nvar x = 1;";
        Assert.DoesNotContain("block", Strip(input));
    }
    [Fact] public void CommentInsideString_IsPreserved()
    {
        string input = "var s = '// not a comment';";
        Assert.Equal(input, Strip(input));
    }
    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }
    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "// a\nvar x = 1; /* b */\n";
        Assert.Equal(2, CountRemoved(input));
    }
    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "var x = 1;\nvar y = 2;\n";
        Assert.Equal(input, Strip(input));
    }
}
public sealed class PowerShellStripperTests
{
    private static readonly ICommentStripper _ps = CommentStripperFactory.Get(Language.PowerShell);
    private static string Strip(string source) => _ps.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _ps.Strip(source).CommentsRemoved;
    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "# comment\n$x = 1";
        Assert.DoesNotContain("# comment", Strip(input));
        Assert.Contains("$x = 1", Strip(input));
    }
    [Fact] public void BlockComment_IsRemoved()
    {
        string input = "<# block #>\n$x = 1";
        Assert.DoesNotContain("block", Strip(input));
        Assert.Contains("$x = 1", Strip(input));
    }
    [Fact] public void CommentInsideDoubleQuotedString_IsPreserved()
    {
        string input = "$s = \"# not a comment\"";
        Assert.Equal(input, Strip(input));
    }
    [Fact] public void CommentInsideSingleQuotedString_IsPreserved()
    {
        string input = "$s = '# not a comment'";
        Assert.Equal(input, Strip(input));
    }
    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }
    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "# a\n$x = 1 <# b #>\n";
        Assert.Equal(2, CountRemoved(input));
    }
    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "$x = 1\n$y = 2\n";
        Assert.Equal(input, Strip(input));
    }
}
public sealed class ScalaStripperTests
{
    private static readonly ICommentStripper _scala = CommentStripperFactory.Get(Language.Scala);
    private static string Strip(string source) => _scala.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _scala.Strip(source).CommentsRemoved;
    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "// comment\nval x = 1";
        Assert.DoesNotContain("// comment", Strip(input));
    }
    [Fact] public void BlockComment_IsRemoved()
    {
        string input = "/* block */\nval x = 1";
        Assert.DoesNotContain("block", Strip(input));
    }
    [Fact] public void CommentInsideString_IsPreserved()
    {
        string input = "val s = \"// not a comment\"";
        Assert.Equal(input, Strip(input));
    }
    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }
    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "// a\nval x = 1 /* b */\n";
        Assert.Equal(2, CountRemoved(input));
    }
    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "val x = 1\nval y = 2\n";
        Assert.Equal(input, Strip(input));
    }
}
public sealed class RStripperTests
{
    private static readonly ICommentStripper _r = CommentStripperFactory.Get(Language.R);
    private static string Strip(string source) => _r.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _r.Strip(source).CommentsRemoved;
    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "# comment\nx <- 1";
        Assert.DoesNotContain("# comment", Strip(input));
        Assert.Contains("x <- 1", Strip(input));
    }
    [Fact] public void CommentInsideString_IsPreserved()
    {
        string input = "s <- \"# not a comment\"";
        Assert.Equal(input, Strip(input));
    }
    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }
    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "# a\nx <- 1 # b\n";
        Assert.Equal(2, CountRemoved(input));
    }
    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "x <- 1\ny <- 2\n";
        Assert.Equal(input, Strip(input));
    }
}
public sealed class PerlStripperTests
{
    private static readonly ICommentStripper _perl = CommentStripperFactory.Get(Language.Perl);
    private static string Strip(string source) => _perl.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _perl.Strip(source).CommentsRemoved;
    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "# comment\n$x = 1;";
        Assert.DoesNotContain("# comment", Strip(input));
        Assert.Contains("$x = 1;", Strip(input));
    }
    [Fact] public void CommentInsideDoubleQuotedString_IsPreserved()
    {
        string input = "$s = \"# not a comment\";";
        Assert.Equal(input, Strip(input));
    }
    [Fact] public void CommentInsideSingleQuotedString_IsPreserved()
    {
        string input = "$s = '# not a comment';";
        Assert.Equal(input, Strip(input));
    }
    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }
    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "# a\n$x = 1; # b\n";
        Assert.Equal(2, CountRemoved(input));
    }
    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "$x = 1;\n$y = 2;\n";
        Assert.Equal(input, Strip(input));
    }
}
public sealed class HaskellStripperTests
{
    private static readonly ICommentStripper _hs = CommentStripperFactory.Get(Language.Haskell);
    private static string Strip(string source) => _hs.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _hs.Strip(source).CommentsRemoved;
    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "-- comment\nx = 1";
        Assert.DoesNotContain("-- comment", Strip(input));
        Assert.Contains("x = 1", Strip(input));
    }
    [Fact] public void BlockComment_IsRemoved()
    {
        string input = "{- block -}\nx = 1";
        Assert.DoesNotContain("block", Strip(input));
    }
    [Fact] public void NestedBlockComment_IsRemoved()
    {
        string input = "{- outer {- inner -} -}\nx = 1";
        Assert.DoesNotContain("outer", Strip(input));
        Assert.DoesNotContain("inner", Strip(input));
        Assert.Contains("x = 1", Strip(input));
    }
    [Fact] public void CommentInsideString_IsPreserved()
    {
        string input = "s = \"-- not a comment\"";
        Assert.Equal(input, Strip(input));
    }
    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }
    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "-- a\nx = 1 {- b -}\n";
        Assert.Equal(2, CountRemoved(input));
    }
    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "x = 1\ny = 2\n";
        Assert.Equal(input, Strip(input));
    }
}
public sealed class ElixirStripperTests
{
    private static readonly ICommentStripper _ex = CommentStripperFactory.Get(Language.Elixir);
    private static string Strip(string source) => _ex.Strip(source).CleanedSource;
    private static int CountRemoved(string source) => _ex.Strip(source).CommentsRemoved;
    [Fact] public void SingleLineComment_IsRemoved()
    {
        string input = "# comment\nx = 1";
        Assert.DoesNotContain("# comment", Strip(input));
        Assert.Contains("x = 1", Strip(input));
    }
    [Fact] public void CommentInsideString_IsPreserved()
    {
        string input = "s = \"# not a comment\"";
        Assert.Equal(input, Strip(input));
    }
    [Fact] public void EmptyFile_ReturnsEmpty()
    {
        Assert.Equal("", Strip(""));
    }
    [Fact] public void CommentCount_IsCorrect()
    {
        string input = "# a\nx = 1 # b\n";
        Assert.Equal(2, CountRemoved(input));
    }
    [Fact] public void NoComments_SourceUnchanged()
    {
        string input = "x = 1\ny = 2\n";
        Assert.Equal(input, Strip(input));
    }
}