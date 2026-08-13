using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Pruner.UI.Highlighting;

internal static class PythonHighlightingDefinition
{
    public static IHighlightingDefinition Build()
    {
        var ruleSet = new HighlightingRuleSet();

        AddKeywords(ruleSet, Color.FromRgb(86, 156, 214),
            "False", "None", "True", "and", "as", "assert", "async", "await",
            "break", "class", "continue", "def", "del", "elif", "else", "except",
            "finally", "for", "from", "global", "if", "import", "in", "is",
            "lambda", "nonlocal", "not", "or", "pass", "raise", "return",
            "try", "while", "with", "yield");

        AddKeywords(ruleSet, Color.FromRgb(220, 220, 170),
            "print", "len", "range", "enumerate", "zip", "map", "filter",
            "list", "dict", "set", "tuple", "str", "int", "float", "bool",
            "type", "isinstance", "hasattr", "getattr", "setattr",
            "open", "super", "self", "cls");

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@"#"),
            EndExpression = new System.Text.RegularExpressions.Regex(@"$"),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85)) },
        });

        foreach (var delim in new[] { "\"\"\"", "'''" })
        {
            ruleSet.Spans.Add(new HighlightingSpan
            {
                StartExpression = new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape(delim)),
                EndExpression = new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape(delim)),
                SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(206, 145, 120)) },
            });
        }

        foreach (var delim in new[] { "\"", "'" })
        {
            ruleSet.Spans.Add(new HighlightingSpan
            {
                StartExpression = new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape(delim)),
                EndExpression = new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape(delim)),
                SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(206, 145, 120)) },
            });
        }

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new System.Text.RegularExpressions.Regex(@"\b\d+\.?\d*\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(181, 206, 168)) },
        });

        return new SimpleHighlighting("Python", ruleSet);
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