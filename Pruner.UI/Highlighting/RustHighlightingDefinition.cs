using System.Text.RegularExpressions;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Pruner.UI.Highlighting;

internal static class RustHighlightingDefinition
{
    public static IHighlightingDefinition Build()
    {
        var ruleSet = new HighlightingRuleSet();

        AddKeywords(ruleSet, Color.FromRgb(197, 134, 192),
            "as", "async", "await", "break", "const", "continue", "crate", "dyn",
            "else", "enum", "extern", "false", "fn", "for", "if", "impl", "in",
            "let", "loop", "match", "mod", "move", "mut", "pub", "ref", "return",
            "self", "Self", "static", "struct", "super", "trait", "true", "type",
            "union", "unsafe", "use", "where", "while");

        AddKeywords(ruleSet, Color.FromRgb(86, 156, 214),
            "bool", "char", "f32", "f64", "i8", "i16", "i32", "i64", "i128",
            "isize", "str", "u8", "u16", "u32", "u64", "u128", "usize", "String",
            "Vec", "Option", "Result", "Box", "Rc", "Arc", "HashMap", "HashSet");

        AddKeywords(ruleSet, Color.FromRgb(220, 220, 170),
            "println", "print", "eprintln", "eprint", "panic", "assert",
            "assert_eq", "assert_ne", "todo", "unimplemented", "unreachable",
            "dbg", "vec", "format", "write", "writeln");

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new Regex(@"//"),
            EndExpression   = new Regex(@"$"),
            SpanColor       = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85)) },
        });

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new Regex(@"/\*"),
            EndExpression   = new Regex(@"\*/"),
            SpanColor       = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85)) },
        });

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new Regex(@""""),
            EndExpression   = new Regex(@""""),
            SpanColor       = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(206, 145, 120)) },
        });

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new Regex(@"'[a-z_][a-z0-9_]*"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(78, 201, 176)) },
        });

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new Regex(@"\b\d+\.?\d*\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(181, 206, 168)) },
        });

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new Regex(@"[A-Z][A-Z0-9_]{2,}\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(156, 220, 254)) },
        });

        return new SimpleHighlighting("Rust", ruleSet);
    }

    private static void AddKeywords(HighlightingRuleSet ruleSet, Color color, params string[] keywords)
    {
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new Regex(@"\b(" + string.Join("|", keywords) + @")\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(color) },
        });
    }
}