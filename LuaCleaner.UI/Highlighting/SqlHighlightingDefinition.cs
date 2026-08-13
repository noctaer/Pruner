using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace LuaCleaner.UI.Highlighting;

internal static class SqlHighlightingDefinition
{
    public static IHighlightingDefinition Build()
    {
        var ruleSet = new HighlightingRuleSet();

        AddKeywords(ruleSet, Color.FromRgb(86, 156, 214),
            "SELECT","FROM","WHERE","INSERT","INTO","VALUES","UPDATE","SET",
            "DELETE","CREATE","TABLE","ALTER","DROP","INDEX","VIEW","PROCEDURE",
            "FUNCTION","TRIGGER","DATABASE","SCHEMA","JOIN","LEFT","RIGHT","INNER",
            "OUTER","FULL","CROSS","ON","AS","AND","OR","NOT","IN","BETWEEN","LIKE",
            "IS","NULL","EXISTS","UNION","ALL","DISTINCT","ORDER","BY","GROUP","HAVING",
            "LIMIT","OFFSET","TOP","WITH","CASE","WHEN","THEN","ELSE","END","BEGIN",
            "COMMIT","ROLLBACK","TRANSACTION","PRIMARY","KEY","FOREIGN","REFERENCES",
            "UNIQUE","DEFAULT","CHECK","CONSTRAINT","IF","WHILE","RETURN",
            "select","from","where","insert","into","values","update","set",
            "delete","create","table","alter","drop","index","view","procedure",
            "function","trigger","database","schema","join","left","right","inner",
            "outer","full","cross","on","as","and","or","not","in","between","like",
            "is","null","exists","union","all","distinct","order","by","group","having",
            "limit","offset","top","with","case","when","then","else","end","begin",
            "commit","rollback","transaction","primary","key","foreign","references",
            "unique","default","check","constraint","if","while","return");

        AddKeywords(ruleSet, Color.FromRgb(220, 220, 170),
            "INT","VARCHAR","NVARCHAR","TEXT","BIGINT","SMALLINT","TINYINT",
            "DECIMAL","NUMERIC","FLOAT","REAL","BIT","DATETIME","DATE","TIME",
            "TIMESTAMP","BOOLEAN","CHAR","NCHAR","BINARY","VARBINARY","BLOB",
            "COUNT","SUM","AVG","MIN","MAX","COALESCE","ISNULL","NULLIF",
            "CAST","CONVERT","GETDATE","NOW","DATEADD","DATEDIFF");

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@"--"),
            EndExpression = new System.Text.RegularExpressions.Regex(@"$"),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85)) },
        });

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@"/\*"),
            EndExpression = new System.Text.RegularExpressions.Regex(@"\*/"),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(106, 153, 85)) },
        });

        ruleSet.Spans.Add(new HighlightingSpan
        {
            StartExpression = new System.Text.RegularExpressions.Regex(@"'"),
            EndExpression = new System.Text.RegularExpressions.Regex(@"'"),
            SpanColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(206, 145, 120)) },
        });

        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = new System.Text.RegularExpressions.Regex(@"\b\d+\.?\d*\b"),
            Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(181, 206, 168)) },
        });

        return new SimpleHighlighting("SQL", ruleSet);
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