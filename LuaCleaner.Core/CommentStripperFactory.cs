using LuaCleaner.Core.Strippers;

namespace LuaCleaner.Core;

public static class CommentStripperFactory
{
    public static ICommentStripper Get(Language language) => language switch
    {
        Language.Luau       => new LuauCommentStripper(),
        Language.Python     => new PythonCommentStripper(),
        Language.JavaScript => new CStyleCommentStripper(supportTemplateLiterals: true),
        Language.TypeScript => new CStyleCommentStripper(supportTemplateLiterals: true),
        Language.CSharp     => new CStyleCommentStripper(supportTemplateLiterals: false),
        Language.Sql        => new SqlCommentStripper(),
        Language.Ruby       => new RubyCommentStripper(),
        Language.Go         => new CStyleCommentStripper(supportTemplateLiterals: false),
        Language.Kotlin     => new CStyleCommentStripper(supportTemplateLiterals: false),
        Language.Swift      => new CStyleCommentStripper(supportTemplateLiterals: false),
        Language.Bash       => new PythonCommentStripper(),
        Language.Rust       => new RustCommentStripper(),
        Language.Html       => new HtmlCommentStripper(),
        Language.Css        => new CssCommentStripper(),
        Language.Php        => new PhpCommentStripper(),
        Language.Java       => new CStyleCommentStripper(supportTemplateLiterals: false),
        Language.C          => new CStyleCommentStripper(supportTemplateLiterals: false),
        Language.Cpp        => new CStyleCommentStripper(supportTemplateLiterals: false),
        Language.Dart       => new CStyleCommentStripper(supportTemplateLiterals: false),
        Language.PowerShell => new PowerShellCommentStripper(),
        Language.Scala      => new CStyleCommentStripper(supportTemplateLiterals: false),
        Language.R          => new PythonCommentStripper(),
        Language.Perl       => new PerlCommentStripper(),
        Language.Haskell    => new HaskellCommentStripper(),
        Language.Elixir     => new PythonCommentStripper(),
        _                   => throw new ArgumentOutOfRangeException(nameof(language)),
    };
}