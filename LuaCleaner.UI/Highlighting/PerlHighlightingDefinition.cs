using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace LuaCleaner.UI.Highlighting;

internal static class PerlHighlightingDefinition
{
    public static IHighlightingDefinition Build()
    {
        var ruleSet = new HighlightingRuleSet();

        AddKeywords(ruleSet, Color.FromRgb(86, 156, 214),
            "BEGIN","END","CHECK","INIT","UNITCHECK","AUTOLOAD","DESTROY",
            "and","cmp","continue","do","else","elsif","eq","for","foreach",
            "ge","gt","if","last","le","local","lt","my","ne","next","no",
            "not","or","our","package","redo","require","return","state",
            "sub","unless","until","use","while","xor");

        AddKeywords(ruleSet, Color.FromRgb(220, 220, 170),
            "print","say","printf","sprintf","die","warn","exit","chomp","chop",
            "push","pop","shift","unshift","splice","reverse","sort","grep","map",
            "join","split","length","substr","index","rindex","uc","lc","ucfirst",
            "lcfirst","defined","undef","ref","scalar","wantarray","caller",
            "open","close","read","write","seek","tell","eof","binmode",
            "keys","values","each","delete","exists");

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@"#"),
            EndExpression   = new System.Text.RegularExpressions.Regex(@"$"),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85)) },
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
            Regex = new System.Text.RegularExpressions.Regex(@"\$[\w]+|@[\w]+|%[\w]+"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(156, 220, 254)) },
        });

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new System.Text.RegularExpressions.Regex(@"\b\d+\.?\d*\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(181, 206, 168)) },
        });

        return new SimpleHighlighting("Perl", ruleSet);
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