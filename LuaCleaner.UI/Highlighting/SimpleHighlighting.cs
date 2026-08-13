using ICSharpCode.AvalonEdit.Highlighting;

namespace LuaCleaner.UI.Highlighting;

internal sealed class SimpleHighlighting : IHighlightingDefinition
{
    private readonly HighlightingRuleSet _ruleSet;

    public SimpleHighlighting(string name, HighlightingRuleSet ruleSet)
    {
        Name = name;
        _ruleSet = ruleSet;
    }

    public string Name { get; }
    public HighlightingRuleSet MainRuleSet => _ruleSet;
    public IEnumerable<HighlightingColor> NamedHighlightingColors => Enumerable.Empty<HighlightingColor>();
    public IDictionary<string, string> Properties => new Dictionary<string, string>();
    public HighlightingRuleSet? GetNamedRuleSet(string name) => null;
    public HighlightingColor? GetNamedColor(string name) => null;
}