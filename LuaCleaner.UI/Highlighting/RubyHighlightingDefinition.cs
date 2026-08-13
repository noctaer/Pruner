using System.Text.RegularExpressions;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace LuaCleaner.UI.Highlighting;

internal static class RubyHighlightingDefinition
{
    public static IHighlightingDefinition Build()
    {
        var ruleSet = new HighlightingRuleSet();

        AddKeywords(ruleSet, Color.FromRgb(197, 134, 192),
            "def", "end", "class", "module", "do", "if", "elsif", "else",
            "unless", "case", "when", "while", "until", "for", "in", "return",
            "yield", "begin", "rescue", "ensure", "raise", "then", "self",
            "super", "and", "or", "not");

        AddKeywords(ruleSet, Color.FromRgb(86, 156, 214),
            "require", "require_relative", "include", "extend",
            "attr_accessor", "attr_reader", "attr_writer",
            "nil", "true", "false");

        AddKeywords(ruleSet, Color.FromRgb(220, 220, 170),
            "puts", "print", "p", "pp", "gets", "chomp",
            "new", "initialize", "freeze", "dup", "clone");

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new Regex(@"(?m)^=begin\b"),
            EndExpression   = new Regex(@"(?m)^=end\b"),
            SpanColor       = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85)) },
        });

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new Regex(@"#"),
            EndExpression   = new Regex(@"$"),
            SpanColor       = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85)) },
        });

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new Regex(@"<<[-~]?[A-Za-z_]\w*"),
            EndExpression   = new Regex(@"^\s*[A-Za-z_]\w*$"),
            SpanColor       = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(206, 145, 120)) },
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
            Regex = new Regex(@":[A-Za-z_]\w*"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(78, 201, 176)) },
        });

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new Regex(@"@{1,2}[A-Za-z_]\w*"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(156, 220, 254)) },
        });

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new Regex(@"\b\d+\.?\d*\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(181, 206, 168)) },
        });

        return new SimpleHighlighting("Ruby", ruleSet);
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