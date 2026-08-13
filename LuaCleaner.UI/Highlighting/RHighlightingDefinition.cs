using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace LuaCleaner.UI.Highlighting;

internal static class RHighlightingDefinition
{
    public static IHighlightingDefinition Build()
    {
        var ruleSet = new HighlightingRuleSet();

        AddKeywords(ruleSet, Color.FromRgb(86, 156, 214),
            "break","else","FALSE","for","function","if","in","Inf","NA","NA_character_",
            "NA_complex_","NA_integer_","NA_real_","NaN","next","NULL","repeat",
            "return","TRUE","while");

        AddKeywords(ruleSet, Color.FromRgb(220, 220, 170),
            "print","cat","paste","paste0","sprintf","message","warning","stop",
            "c","list","data.frame","matrix","array","vector","factor","table",
            "length","nrow","ncol","dim","names","colnames","rownames","str","summary",
            "mean","sum","min","max","range","sd","var","cor","median",
            "library","require","source","setwd","getwd","ls","rm",
            "read.csv","write.csv","read.table","readRDS","saveRDS",
            "lapply","sapply","vapply","tapply","mapply","Map","Reduce",
            "which","any","all","is.na","is.null","is.numeric","is.character",
            "as.numeric","as.character","as.integer","as.logical");

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
            Regex = new System.Text.RegularExpressions.Regex(@"\b\d+\.?\d*[iL]?\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(181, 206, 168)) },
        });

        return new SimpleHighlighting("R", ruleSet);
    }

    private static void AddKeywords(HighlightingRuleSet ruleSet, Color color, params string[] keywords)
    {
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new System.Text.RegularExpressions.Regex(@"\b(" + string.Join("|", keywords.Select(System.Text.RegularExpressions.Regex.Escape)) + @")\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(color) },
        });
    }
}