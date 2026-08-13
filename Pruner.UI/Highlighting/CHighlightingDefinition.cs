using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Pruner.UI.Highlighting;

internal static class CHighlightingDefinition
{
    public static IHighlightingDefinition Build()
    {
        var ruleSet = new HighlightingRuleSet();

        AddKeywords(ruleSet, Color.FromRgb(86, 156, 214),
            "auto","break","case","char","const","continue","default","do","double",
            "else","enum","extern","float","for","goto","if","inline","int","long",
            "register","restrict","return","short","signed","sizeof","static","struct",
            "switch","typedef","union","unsigned","void","volatile","while",
            "NULL","true","false","bool","_Bool","_Complex","_Imaginary");

        AddKeywords(ruleSet, Color.FromRgb(220, 220, 170),
            "printf","scanf","malloc","calloc","realloc","free","memcpy","memset",
            "strlen","strcpy","strcmp","strcat","fopen","fclose","fread","fwrite",
            "fprintf","sprintf","exit","abort","assert");

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

        return new SimpleHighlighting("C", ruleSet);
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