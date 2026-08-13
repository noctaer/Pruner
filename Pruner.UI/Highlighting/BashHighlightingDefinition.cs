using System.Text.RegularExpressions;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Pruner.UI.Highlighting;

internal static class BashHighlightingDefinition
{
    public static IHighlightingDefinition Build()
    {
        var ruleSet = new HighlightingRuleSet();

        AddKeywords(ruleSet, Color.FromRgb(197, 134, 192),
            "if", "then", "else", "elif", "fi", "for", "while", "until", "do",
            "done", "case", "esac", "in", "function", "return", "exit", "break",
            "continue", "local", "readonly", "export", "unset", "shift", "set");

        AddKeywords(ruleSet, Color.FromRgb(220, 220, 170),
            "echo", "printf", "read", "source", "cd", "ls", "mkdir", "rm",
            "cp", "mv", "cat", "grep", "sed", "awk", "find", "chmod", "chown",
            "curl", "wget", "tar", "zip", "unzip", "sudo", "apt", "yum", "brew");

        AddKeywords(ruleSet, Color.FromRgb(86, 156, 214),
            "true", "false", "null");

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new Regex(@"#"),
            EndExpression   = new Regex(@"$"),
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
            Regex = new Regex(@"\$\w+|\$\{[^}]+\}"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(156, 220, 254)) },
        });

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new Regex(@"\b\d+\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(181, 206, 168)) },
        });

        return new SimpleHighlighting("Bash", ruleSet);
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