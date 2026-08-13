using System.Text.RegularExpressions;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace LuaCleaner.UI.Highlighting;

internal static class CssHighlightingDefinition
{
    public static IHighlightingDefinition Build()
    {
        var ruleSet = new HighlightingRuleSet();

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

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new Regex(@"'"),
            EndExpression   = new Regex(@"'"),
            SpanColor       = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(206, 145, 120)) },
        });

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new Regex(@"--[a-zA-Z][a-zA-Z0-9-]*"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(78, 201, 176)) },
        });

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new Regex(@"#[0-9a-fA-F]{3,8}\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(206, 145, 120)) },
        });

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new Regex(@"\b(px|em|rem|vh|vw|%|pt|cm|mm|fr|deg|s|ms)\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(181, 206, 168)) },
        });

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new Regex(@"\b(display|position|color|background|margin|padding|border|font|width|height|flex|grid|overflow|opacity|transform|transition|animation|z-index|top|left|right|bottom|content|cursor|visibility|pointer-events)\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(156, 220, 254)) },
        });

        return new SimpleHighlighting("CSS", ruleSet);
    }
}