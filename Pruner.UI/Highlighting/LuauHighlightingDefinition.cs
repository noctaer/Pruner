using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Pruner.UI.Highlighting;

internal static class LuauHighlightingDefinition
{
    public static IHighlightingDefinition Build()
    {
        var ruleSet = new HighlightingRuleSet();

        AddKeywords(ruleSet, Color.FromRgb(86, 156, 214),
            "and", "break", "do", "else", "elseif", "end", "false", "for",
            "function", "if", "in", "local", "nil", "not", "or", "repeat",
            "return", "then", "true", "until", "while", "type", "export", "continue");

        AddKeywords(ruleSet, Color.FromRgb(220, 220, 170),
            "game", "workspace", "script", "math", "table", "string", "os",
            "print", "warn", "error", "pcall", "xpcall", "ipairs", "pairs",
            "next", "select", "tonumber", "tostring", "type", "unpack",
            "require", "rawget", "rawset", "setmetatable", "getmetatable",
            "task", "tick", "wait", "spawn", "delay", "coroutine", "bit32",
            "Instance", "Vector3", "Vector2", "CFrame", "Color3", "UDim",
            "UDim2", "Enum", "BrickColor", "TweenInfo", "Ray", "Region3",
            "Players", "RunService", "TweenService", "UserInputService",
            "ReplicatedStorage", "DataStoreService", "HttpService",
            "Debris", "Lighting", "SoundService", "CollectionService");

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@"--(?!\[=*\[)"),
            EndExpression = new System.Text.RegularExpressions.Regex(@"$"),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85)) },
        });

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@"--\[=*\["),
            EndExpression = new System.Text.RegularExpressions.Regex(@"\]=*\]"),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85)) },
        });

        foreach (var delim in new[] { "\"", "'" })
        {
            ruleSet.Spans.Add(new HighlightingSpan
            {
                StartExpression = new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape(delim)),
                EndExpression = new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape(delim)),
                SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(206, 145, 120)) },
            });
        }

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@"\[=*\["),
            EndExpression = new System.Text.RegularExpressions.Regex(@"\]=*\]"),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(206, 145, 120)) },
        });

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new System.Text.RegularExpressions.Regex(@"\b(0x[\da-fA-F]+|\d+\.?\d*([eE][+-]?\d+)?)\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(181, 206, 168)) },
        });

        return new SimpleHighlighting("Luau", ruleSet);
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