using ICSharpCode.AvalonEdit.Highlighting;
using LuaCleaner.Core;

namespace LuaCleaner.UI.Highlighting;

internal static class HighlightingFactory
{
    public static IHighlightingDefinition Build(Language language) => language switch
    {
        Language.Luau       => LuauHighlightingDefinition.Build(),
        Language.Python     => PythonHighlightingDefinition.Build(),
        Language.JavaScript => CStyleHighlightingDefinition.Build("JavaScript"),
        Language.TypeScript => CStyleHighlightingDefinition.Build("TypeScript"),
        Language.CSharp     => CStyleHighlightingDefinition.Build("C#"),
        Language.Sql        => SqlHighlightingDefinition.Build(),
        Language.Ruby       => RubyHighlightingDefinition.Build(),
        Language.Go         => GoHighlightingDefinition.Build(),
        Language.Kotlin     => CStyleHighlightingDefinition.Build("Kotlin"),
        Language.Swift      => CStyleHighlightingDefinition.Build("Swift"),
        Language.Bash       => BashHighlightingDefinition.Build(),
        Language.Rust       => RustHighlightingDefinition.Build(),
        Language.Html       => HtmlHighlightingDefinition.Build(),
        Language.Css        => CssHighlightingDefinition.Build(),
        Language.Php        => PhpHighlightingDefinition.Build(),
        Language.Java       => JavaHighlightingDefinition.Build(),
        Language.C          => CHighlightingDefinition.Build(),
        Language.Cpp        => CppHighlightingDefinition.Build(),
        Language.Dart       => DartHighlightingDefinition.Build(),
        Language.PowerShell => PowerShellHighlightingDefinition.Build(),
        Language.Scala      => ScalaHighlightingDefinition.Build(),
        Language.R          => RHighlightingDefinition.Build(),
        Language.Perl       => PerlHighlightingDefinition.Build(),
        Language.Haskell    => HaskellHighlightingDefinition.Build(),
        Language.Elixir     => ElixirHighlightingDefinition.Build(),
        _                   => LuauHighlightingDefinition.Build(),
    };
}