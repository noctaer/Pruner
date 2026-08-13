using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace LuaCleaner.UI.Highlighting;

internal static class CStyleHighlightingDefinition
{
    public static IHighlightingDefinition Build(string name)
    {
        var ruleSet = new HighlightingRuleSet();

        string[] keywords = name == "C#"
            ? new[] { "abstract","as","base","bool","break","byte","case","catch","char",
                      "checked","class","const","continue","decimal","default","delegate",
                      "do","double","else","enum","event","explicit","extern","false",
                      "finally","fixed","float","for","foreach","goto","if","implicit",
                      "in","int","interface","internal","is","lock","long","namespace",
                      "new","null","object","operator","out","override","params","private",
                      "protected","public","readonly","ref","return","sbyte","sealed",
                      "short","sizeof","stackalloc","static","string","struct","switch",
                      "this","throw","true","try","typeof","uint","ulong","unchecked",
                      "unsafe","ushort","using","virtual","void","volatile","while",
                      "var","dynamic","async","await","record","init","required","file" }
            : new[] { "break","case","catch","class","const","continue","debugger",
                      "default","delete","do","else","export","extends","false","finally",
                      "for","function","if","import","in","instanceof","let","new","null",
                      "return","static","super","switch","this","throw","true","try",
                      "typeof","undefined","var","void","while","with","yield","async",
                      "await","of","from","as","type","interface","enum","implements",
                      "readonly","declare","namespace","abstract","override" };

        AddKeywords(ruleSet, Color.FromRgb(86, 156, 214), keywords);

        AddKeywords(ruleSet, Color.FromRgb(220, 220, 170),
            "console", "Math", "JSON", "Object", "Array", "String", "Number",
            "Boolean", "Promise", "setTimeout", "setInterval", "clearTimeout",
            "document", "window", "process", "require", "module", "exports",
            "Console", "DateTime", "List", "Dictionary", "Task", "Exception");

        // Single-line comment
        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@"//"),
            EndExpression = new System.Text.RegularExpressions.Regex(@"$"),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85)) },
        });

        // Block comment
        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@"/\*"),
            EndExpression = new System.Text.RegularExpressions.Regex(@"\*/"),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85)) },
        });

        // Strings
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

        return new SimpleHighlighting(name, ruleSet);
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