using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Pruner.UI.Highlighting;

internal static class PowerShellHighlightingDefinition
{
    public static IHighlightingDefinition Build()
    {
        var ruleSet = new HighlightingRuleSet();

        AddKeywords(ruleSet, Color.FromRgb(86, 156, 214),
            "begin","break","catch","class","continue","data","define","do","dynamicparam",
            "else","elseif","end","enum","exit","filter","finally","for","foreach",
            "from","function","hidden","if","in","param","process","return","static",
            "switch","throw","trap","try","until","using","var","while");

        AddKeywords(ruleSet, Color.FromRgb(220, 220, 170),
            "Write-Host","Write-Output","Write-Error","Write-Warning","Write-Verbose",
            "Get-Content","Set-Content","Add-Content","Get-Item","Set-Item",
            "Get-ChildItem","Remove-Item","Copy-Item","Move-Item","New-Item",
            "Get-Command","Invoke-Expression","Start-Process","Stop-Process",
            "ForEach-Object","Where-Object","Select-Object","Sort-Object",
            "Import-Module","Export-ModuleMember","New-Object","Add-Type");

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@"<#"),
            EndExpression   = new System.Text.RegularExpressions.Regex(@"#>"),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85)) },
        });

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
            Regex = new System.Text.RegularExpressions.Regex(@"\$[\w]+"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(156, 220, 254)) },
        });

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new System.Text.RegularExpressions.Regex(@"\b\d+\.?\d*\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(181, 206, 168)) },
        });

        return new SimpleHighlighting("PowerShell", ruleSet);
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