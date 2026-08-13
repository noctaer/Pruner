using System.Text.RegularExpressions;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Pruner.UI.Highlighting;

internal static class JavaHighlightingDefinition
{
    public static IHighlightingDefinition Build()
    {
        var ruleSet = new HighlightingRuleSet();

        AddKeywords(ruleSet);
        AddComments(ruleSet);
        AddStrings(ruleSet);
        AddNumbers(ruleSet);

        return new SimpleHighlighting("Java", ruleSet);
    }

    private static void AddKeywords(HighlightingRuleSet ruleSet)
    {
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new Regex(
                @"\b(abstract|assert|boolean|break|byte|case|catch|char|class|const|continue|" +
                @"default|do|double|else|enum|extends|final|finally|float|for|goto|if|implements|" +
                @"import|instanceof|int|interface|long|native|new|null|package|private|protected|" +
                @"public|record|return|sealed|short|static|strictfp|super|switch|synchronized|" +
                @"this|throw|throws|transient|try|var|void|volatile|while|true|false)\b"),
            Color = new HighlightingColor
            {
                Foreground = new SimpleHighlightingBrush(Color.FromRgb(86, 156, 214))
            }
        });
    }

    private static void AddComments(HighlightingRuleSet ruleSet)
    {
        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new Regex(@"//"),
            EndExpression = new Regex(@"$"),
            SpanColor = new HighlightingColor
            {
                Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85))
            }
        });

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new Regex(@"/\*"),
            EndExpression = new Regex(@"\*/"),
            SpanColor = new HighlightingColor
            {
                Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85))
            }
        });
    }

    private static void AddStrings(HighlightingRuleSet ruleSet)
    {
        foreach (var delimiter in new[] { "\"", "'" })
        {
            ruleSet.Spans.Add(new HighlightingSpan
            {
                StartExpression = new Regex(Regex.Escape(delimiter)),
                EndExpression = new Regex(Regex.Escape(delimiter)),
                SpanColor = new HighlightingColor
                {
                    Foreground = new SimpleHighlightingBrush(Color.FromRgb(206, 145, 120))
                }
            });
        }
    }

    private static void AddNumbers(HighlightingRuleSet ruleSet)
    {
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new Regex(@"\b\d+\.?\d*\b"),
            Color = new HighlightingColor
            {
                Foreground = new SimpleHighlightingBrush(Color.FromRgb(181, 206, 168))
            }
        });
    }
}