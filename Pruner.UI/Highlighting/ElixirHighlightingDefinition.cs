using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Pruner.UI.Highlighting;

internal static class ElixirHighlightingDefinition
{
    public static IHighlightingDefinition Build()
    {
        var ruleSet = new HighlightingRuleSet();

        AddKeywords(ruleSet, Color.FromRgb(86, 156, 214),
            "after","alias","and","case","catch","cond","def","defdelegate",
            "defexception","defimpl","defmacro","defmacrop","defmodule","defoverridable",
            "defp","defprotocol","defstruct","do","else","end","exit","fn","for",
            "if","import","in","not","or","quote","raise","receive","require",
            "rescue","return","send","super","throw","try","unquote","unquote_splicing",
            "unless","use","when","with","true","false","nil");

        AddKeywords(ruleSet, Color.FromRgb(220, 220, 170),
            "Enum","Stream","Map","List","String","Integer","Float","Atom","Tuple",
            "Keyword","IO","File","Path","System","Process","Agent","Task","GenServer",
            "Supervisor","Application","Module","Kernel","inspect","puts","gets",
            "length","hd","tl","elem","put_elem","is_list","is_map","is_tuple",
            "is_atom","is_binary","is_integer","is_float","is_nil","is_boolean");

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@"#"),
            EndExpression   = new System.Text.RegularExpressions.Regex(@"$"),
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
            Regex = new System.Text.RegularExpressions.Regex(@":[a-zA-Z_][\w]*"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(156, 220, 254)) },
        });

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new System.Text.RegularExpressions.Regex(@"\b\d+\.?\d*\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(181, 206, 168)) },
        });

        return new SimpleHighlighting("Elixir", ruleSet);
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