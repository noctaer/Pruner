using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace LuaCleaner.UI.Highlighting;

internal static class ScalaHighlightingDefinition
{
    public static IHighlightingDefinition Build()
    {
        var ruleSet = new HighlightingRuleSet();

        AddKeywords(ruleSet, Color.FromRgb(86, 156, 214),
            "abstract","case","catch","class","def","do","else","enum","export",
            "extends","extension","false","final","finally","for","forSome","given",
            "if","implicit","import","lazy","match","new","null","object","opaque",
            "open","override","package","private","protected","return","sealed",
            "super","then","this","throw","trait","transparent","true","try","type",
            "using","val","var","while","with","yield");

        AddKeywords(ruleSet, Color.FromRgb(220, 220, 170),
            "String","Int","Long","Double","Float","Boolean","Char","Byte","Short",
            "Unit","Any","AnyVal","AnyRef","Nothing","Null","Option","Some","None",
            "List","Vector","Map","Set","Seq","Array","Tuple","Either","Left","Right",
            "Future","Try","Success","Failure","println","print","Console");

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
            StartExpression = new System.Text.RegularExpressions.Regex("\"\"\""),
            EndExpression   = new System.Text.RegularExpressions.Regex("\"\"\""),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(206, 145, 120)) },
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
            Regex = new System.Text.RegularExpressions.Regex(@"\b\d+\.?\d*[lLfFdD]?\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(181, 206, 168)) },
        });

        return new SimpleHighlighting("Scala", ruleSet);
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