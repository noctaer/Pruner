using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace LuaCleaner.UI.Highlighting;

internal static class CppHighlightingDefinition
{
    public static IHighlightingDefinition Build()
    {
        var ruleSet = new HighlightingRuleSet();

        AddKeywords(ruleSet, Color.FromRgb(86, 156, 214),
            "alignas","alignof","and","and_eq","asm","auto","bitand","bitor","bool",
            "break","case","catch","char","char8_t","char16_t","char32_t","class",
            "compl","concept","const","consteval","constexpr","constinit","const_cast",
            "continue","co_await","co_return","co_yield","decltype","default","delete",
            "do","double","dynamic_cast","else","enum","explicit","export","extern",
            "false","final","float","for","friend","goto","if","inline","int","long",
            "mutable","namespace","new","noexcept","not","not_eq","nullptr","operator",
            "or","or_eq","override","private","protected","public","register",
            "reinterpret_cast","requires","return","short","signed","sizeof","static",
            "static_assert","static_cast","struct","switch","template","this",
            "thread_local","throw","true","try","typedef","typeid","typename","union",
            "unsigned","using","virtual","void","volatile","wchar_t","while","xor","xor_eq");

        AddKeywords(ruleSet, Color.FromRgb(220, 220, 170),
            "std","string","vector","map","unordered_map","set","unordered_set","list",
            "deque","queue","stack","pair","tuple","optional","variant","any",
            "shared_ptr","unique_ptr","weak_ptr","make_shared","make_unique",
            "cout","cin","cerr","endl","printf","scanf","malloc","free");

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@"//"),
            EndExpression   = new System.Text.RegularExpressions.Regex(@"$"),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85)) },
        });

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@"/\*"),
            EndExpression   = new System.Text.RegularExpressions.Regex(@"\*/"),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85)) },
        });

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@"#\s*(include|define|ifdef|ifndef|endif|pragma|undef|if|elif|else|error|line)\b"),
            EndExpression   = new System.Text.RegularExpressions.Regex(@"$"),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(155, 110, 180)) },
        });

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex("\""),
            EndExpression   = new System.Text.RegularExpressions.Regex("\""),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(206, 145, 120)) },
        });

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex("'"),
            EndExpression   = new System.Text.RegularExpressions.Regex("'"),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(206, 145, 120)) },
        });

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new System.Text.RegularExpressions.Regex(@"\b\d+\.?\d*[uUlLfF]?\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(181, 206, 168)) },
        });

        return new SimpleHighlighting("C++", ruleSet);
    }

    private static void AddKeywords(HighlightingRuleSet ruleSet, Color color, params string[] keywords)
    {
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new System.Text.RegularExpressions.Regex(@"\b(" + string.Join("|", keywords) + @")\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(color) },
        });
    }
}