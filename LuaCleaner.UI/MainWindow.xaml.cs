using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Highlighting;
using LuaCleaner.Core;
using LuaCleaner.IO;
using LuaCleaner.UI.Highlighting;
using CoreLanguage = LuaCleaner.Core.Language;
using Microsoft.Win32;

namespace LuaCleaner.UI;

public partial class MainWindow : Window
{
    private string? _currentFilePath;
    private string? _cleanedSource;
    private Language _selectedLanguage = CoreLanguage.Luau;
    private readonly RecentFilesService _recentFiles = new();

    public MainWindow()
    {
        InitializeComponent();
        PopulateLanguageSelector();
        ApplyHighlighting();
        EditorOriginal.TextChanged += OnOriginalTextChanged;
        EditorOriginal.AllowDrop = false;
        EditorClean.AllowDrop = false;
    }

    private void PopulateLanguageSelector()
    {
        var languages = new[]
        {
            (CoreLanguage.Luau,       "🌙 Lua / Luau"),
            (CoreLanguage.Python,     "🐍 Python"),
            (CoreLanguage.JavaScript, "📜 JavaScript"),
            (CoreLanguage.TypeScript, "📘 TypeScript"),
            (CoreLanguage.CSharp,     "🔷 C#"),
            (CoreLanguage.Sql,        "🗄 SQL"),
            (CoreLanguage.Ruby,       "💎 Ruby"),
            (CoreLanguage.Go,         "🐹 Go"),
            (CoreLanguage.Kotlin,     "🎯 Kotlin"),
            (CoreLanguage.Swift,      "🦅 Swift"),
            (CoreLanguage.Bash,       "🐚 Bash / Shell"),
            (CoreLanguage.Rust,       "🦀 Rust"),
            (CoreLanguage.Html,       "🌐 HTML"),
            (CoreLanguage.Css,        "🎨 CSS"),
            (CoreLanguage.Php,        "🐘 PHP"),
            (CoreLanguage.Java,       "☕ Java"),
            (CoreLanguage.C,          "🔵 C"),
            (CoreLanguage.Cpp,        "⚡ C++"),
            (CoreLanguage.Dart,       "🎯 Dart"),
            (CoreLanguage.PowerShell, "💙 PowerShell"),
            (CoreLanguage.Scala,      "🔴 Scala"),
            (CoreLanguage.R,          "📊 R"),
            (CoreLanguage.Perl,       "🐪 Perl"),
            (CoreLanguage.Haskell,    "λ Haskell"),
            (CoreLanguage.Elixir,     "💧 Elixir"),
        };

        foreach (var (lang, label) in languages)
            LanguageSelector.Items.Add(new LanguageItem(lang, label));

        LanguageSelector.SelectedIndex = 0;
        LanguageSelector.SelectionChanged += (_, _) =>
        {
            if (LanguageSelector.SelectedItem is LanguageItem item)
            {
                _selectedLanguage = item.Language;
                ApplyHighlighting();
                UpdateFileNameHints();
            }
        };
    }

    private void ApplyHighlighting()
    {
        var highlighting = HighlightingFactory.Build(_selectedLanguage);
        EditorOriginal.SyntaxHighlighting = highlighting;
        EditorClean.SyntaxHighlighting = highlighting;
    }

    private void UpdateFileNameHints()
    {
        if (_currentFilePath == null) return;
        TxtCleanFileName.Text =
            Path.GetFileNameWithoutExtension(_currentFilePath) +
            ".clean" +
            Path.GetExtension(_currentFilePath);
    }

    private void OnOriginalTextChanged(object? sender, EventArgs e)
    {
        var text = EditorOriginal.Text;
        if (string.IsNullOrEmpty(text))
        {
            ResetStats();
            return;
        }
        var bytes = Encoding.UTF8.GetByteCount(text);
        TxtOriginalSize.Text = FormatSize(bytes);
        TxtOriginalSizeStat.Text = FormatSize(bytes);
    }

    private void Window_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string first = files[0];
            bool supported = Directory.Exists(first) ||
                (File.Exists(first) && LanguageDetector.TryDetect(Path.GetExtension(first), out _));
            e.Effects = supported ? DragDropEffects.Copy : DragDropEffects.None;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
        e.Handled = true;
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
        string[] dropped = (string[])e.Data.GetData(DataFormats.FileDrop);
        if (dropped.Length == 0) return;

        string path = dropped[0];
        if (Directory.Exists(path))
            OpenBatchWindow(path);
        else
            LoadFile(path);
    }

    private void BtnBatch_Click(object sender, RoutedEventArgs e)
    {
        string? path = FolderPicker.Pick("Selecione a pasta a processar");
        if (path == null) return;
        OpenBatchWindow(path);
    }

    private void OpenBatchWindow(string folderPath)
    {
        var answer = MessageBox.Show(
            $"Processar subpastas tambem?\n\nPasta: {folderPath}",
            "Processar pasta",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        bool recursive = answer == MessageBoxResult.Yes;
        var window = new BatchWindow(folderPath, recursive);
        window.ShowDialog();
    }

    private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Abrir arquivo",
            Filter = "Todos suportados|*.lua;*.luau;*.py;*.pyw;*.js;*.mjs;*.ts;*.mts;*.cs;*.sql;*.rb;*.rake;*.gemspec;*.go;*.kt;*.kts;*.swift;*.sh;*.bash;*.zsh;*.fish;*.rs;*.html;*.htm;*.css;*.php;*.php8;*.java;*.c;*.h;*.cpp;*.cc;*.cxx;*.hpp;*.hxx;*.dart;*.ps1;*.psm1;*.psd1;*.scala;*.sbt;*.r;*.rmd;*.pl;*.pm;*.hs;*.lhs;*.ex;*.exs" +
                     "|Lua (*.lua;*.luau)|*.lua;*.luau" +
                     "|Python (*.py;*.pyw)|*.py;*.pyw" +
                     "|JavaScript (*.js;*.mjs)|*.js;*.mjs" +
                     "|TypeScript (*.ts;*.mts)|*.ts;*.mts" +
                     "|C# (*.cs)|*.cs" +
                     "|SQL (*.sql)|*.sql" +
                     "|Ruby (*.rb;*.rake;*.gemspec)|*.rb;*.rake;*.gemspec" +
                     "|Go (*.go)|*.go" +
                     "|Kotlin (*.kt;*.kts)|*.kt;*.kts" +
                     "|Swift (*.swift)|*.swift" +
                     "|Bash / Shell (*.sh;*.bash;*.zsh;*.fish)|*.sh;*.bash;*.zsh;*.fish" +
                     "|Rust (*.rs)|*.rs" +
                     "|HTML (*.html;*.htm)|*.html;*.htm" +
                     "|CSS (*.css)|*.css" +
                     "|PHP (*.php;*.php8)|*.php;*.php8" +
                     "|Java (*.java)|*.java" +
                     "|C (*.c;*.h)|*.c;*.h" +
                     "|C++ (*.cpp;*.cc;*.cxx;*.hpp;*.hxx)|*.cpp;*.cc;*.cxx;*.hpp;*.hxx" +
                     "|Dart (*.dart)|*.dart" +
                     "|PowerShell (*.ps1;*.psm1;*.psd1)|*.ps1;*.psm1;*.psd1" +
                     "|Scala (*.scala;*.sbt)|*.scala;*.sbt" +
                     "|R (*.r;*.rmd)|*.r;*.rmd" +
                     "|Perl (*.pl;*.pm)|*.pl;*.pm" +
                     "|Haskell (*.hs;*.lhs)|*.hs;*.lhs" +
                     "|Elixir (*.ex;*.exs)|*.ex;*.exs" +
                     "|Todos os arquivos (*.*)|*.*",
        };

        if (dialog.ShowDialog() != true) return;
        LoadFile(dialog.FileName);
    }

    private void BtnRecentDropdown_Click(object sender, RoutedEventArgs e)
    {
        BuildRecentMenu();
        RecentPopup.IsOpen = true;
    }

    private void BuildRecentMenu()
    {
        RecentList.Children.Clear();

        var entries = _recentFiles.Entries.Where(File.Exists).ToList();

        if (entries.Count == 0)
        {
            RecentList.Children.Add(new TextBlock
            {
                Text = "Nenhum arquivo recente",
                Foreground = new SolidColorBrush(Color.FromRgb(107, 107, 138)),
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 12,
                Padding = new Thickness(8, 6, 8, 6),
            });
            return;
        }

        foreach (string path in entries)
        {
            var row = new Grid { Margin = new Thickness(0, 1, 0, 1) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var btn = new Button
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Padding = new Thickness(8, 6, 4, 6),
                Tag = path,
            };

            var content = new StackPanel();
            content.Children.Add(new TextBlock
            {
                Text = Path.GetFileName(path),
                Foreground = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 13,
            });
            content.Children.Add(new TextBlock
            {
                Text = ShortenPath(path),
                Foreground = new SolidColorBrush(Color.FromRgb(107, 107, 138)),
                FontFamily = new FontFamily("Consolas"),
                FontSize = 10,
            });

            btn.Content = content;
            btn.Click += (_, _) =>
            {
                RecentPopup.IsOpen = false;
                LoadFile(path);
            };

            btn.MouseEnter += (_, _) =>
                btn.Background = new SolidColorBrush(Color.FromRgb(60, 63, 90));
            btn.MouseLeave += (_, _) =>
                btn.Background = Brushes.Transparent;

            var removeBtn = new Button
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Foreground = new SolidColorBrush(Color.FromRgb(107, 107, 138)),
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 14,
                Content = "×",
                Padding = new Thickness(6, 4, 6, 4),
                Cursor = Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Center,
                Tag = path,
                ToolTip = "Remover da lista",
            };
            removeBtn.Click += (_, _) =>
            {
                _recentFiles.Remove(path);
                BuildRecentMenu();
            };
            removeBtn.MouseEnter += (_, _) =>
                removeBtn.Foreground = new SolidColorBrush(Color.FromRgb(224, 224, 224));
            removeBtn.MouseLeave += (_, _) =>
                removeBtn.Foreground = new SolidColorBrush(Color.FromRgb(107, 107, 138));

            Grid.SetColumn(btn, 0);
            Grid.SetColumn(removeBtn, 1);
            row.Children.Add(btn);
            row.Children.Add(removeBtn);
            RecentList.Children.Add(row);
        }
    }

    private static string ShortenPath(string path)
    {
        const int max = 55;
        if (path.Length <= max) return path;
        string dir = Path.GetDirectoryName(path) ?? "";
        string file = Path.GetFileName(path);
        int budget = max - file.Length - 4;
        return budget > 0
            ? dir[..Math.Min(budget, dir.Length)] + "..." + Path.DirectorySeparatorChar + file
            : "..." + Path.DirectorySeparatorChar + file;
    }

    private void LoadFile(string path)
    {
        if (!File.Exists(path))
        {
            SetStatus("Arquivo nao encontrado", path, "#F44336");
            _recentFiles.Remove(path);
            return;
        }

        try
        {
            string ext = Path.GetExtension(path);
            if (LanguageDetector.TryDetect(ext, out CoreLanguage detected))
            {
                _selectedLanguage = detected;
                SyncLanguageSelector(detected);
            }

            string source = File.ReadAllText(path, Encoding.UTF8);
            _currentFilePath = path;
            EditorOriginal.Text = source;
            TxtOriginalFileName.Text = Path.GetFileName(path);
            TxtCleanFileName.Text = Path.GetFileNameWithoutExtension(path) + ".clean" + ext;
            LanguageWarning.Visibility = Visibility.Collapsed;
            SetStatus("Arquivo carregado", Path.GetFileName(path), "#4CAF50");
            TxtHint.Text = "Clique em 'Remover Comentarios' para processar o arquivo.";
            _recentFiles.Add(path);
        }
        catch (Exception ex)
        {
            SetStatus("Erro ao abrir arquivo", $"{Path.GetFileName(path)}: {ex.Message}", "#F44336");
        }
    }

    private void SyncLanguageSelector(CoreLanguage language)
    {
        for (int i = 0; i < LanguageSelector.Items.Count; i++)
        {
            if (LanguageSelector.Items[i] is LanguageItem item && item.Language == language)
            {
                LanguageSelector.SelectedIndex = i;
                return;
            }
        }
    }

    private void BtnPasteCode_Click(object sender, RoutedEventArgs e)
    {
        string text = Clipboard.GetText();
        if (string.IsNullOrEmpty(text))
        {
            SetStatus("Area de transferencia vazia", "Nada para colar.", "#F44336");
            return;
        }
        EditorOriginal.Text = text;
        _currentFilePath = null;
        TxtOriginalFileName.Text = "clipboard";
        TxtCleanFileName.Text = "clipboard.clean";
        SetStatus("Codigo colado", "Pronto para processar.", "#4CAF50");
        TxtHint.Text = "Clique em 'Remover Comentarios' para processar o codigo.";

        if (LanguageSelector.SelectedItem is LanguageItem item)
        {
            TxtLanguageWarning.Text = $"Linguagem manual: {item}";
            LanguageWarning.Visibility = Visibility.Visible;
        }
    }

    private void BtnProcess_Click(object sender, RoutedEventArgs e)
    {
        string source = EditorOriginal.Text;
        if (string.IsNullOrWhiteSpace(source))
        {
            SetStatus("Sem conteudo", "Abra um arquivo ou cole codigo primeiro.", "#F44336");
            return;
        }

        try
        {
            SetStatus("Processando...", "Removendo comentarios...", "#4A90D9");
            var stripper = CommentStripperFactory.Get(_selectedLanguage);
            StripResult result = stripper.Strip(source);
            _cleanedSource = result.CleanedSource;
            EditorClean.Text = _cleanedSource;

            long originalBytes = Encoding.UTF8.GetByteCount(source);
            long cleanBytes = Encoding.UTF8.GetByteCount(_cleanedSource);
            double reduction = originalBytes > 0
                ? (1.0 - (double)cleanBytes / originalBytes) * 100.0
                : 0;

            TxtCommentsRemoved.Text = result.CommentsRemoved.ToString();
            TxtOriginalSizeStat.Text = FormatSize(originalBytes);
            TxtCleanSizeStat.Text = FormatSize(cleanBytes);
            TxtReduction.Text = $"{reduction:F2}%";
            TxtCleanSize.Text = FormatSize(cleanBytes);

            SetStatus("Concluido",
                $"{result.CommentsRemoved} comentario(s) removido(s) — {LanguageDetector.DisplayName(_selectedLanguage)}",
                "#4CAF50");
            TxtHint.Text = "Processamento concluido. Use 'Salvar codigo' ou 'Copiar codigo'.";
        }
        catch (Exception ex)
        {
            SetStatus("Erro no processamento", ex.Message, "#F44336");
        }
    }

    private void BtnCopyCode_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_cleanedSource))
        {
            SetStatus("Sem codigo limpo", "Processe o codigo primeiro.", "#F44336");
            return;
        }
        Clipboard.SetText(_cleanedSource);
        SetStatus("Copiado", "Codigo limpo copiado para a area de transferencia.", "#4CAF50");
    }

    private void BtnSaveCode_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_cleanedSource))
        {
            SetStatus("Sem codigo limpo", "Processe o codigo primeiro.", "#F44336");
            return;
        }

        string defaultName = "clean.lua";
        if (_currentFilePath != null)
        {
            string dir = Path.GetDirectoryName(_currentFilePath) ?? ".";
            string nameNoExt = Path.GetFileNameWithoutExtension(_currentFilePath);
            string ext = Path.GetExtension(_currentFilePath);
            defaultName = Path.Combine(dir, $"{nameNoExt}.clean{ext}");
        }

        var dialog = new SaveFileDialog
        {
            Title = "Salvar codigo limpo",
            Filter = "Todos os arquivos (*.*)|*.*",
            FileName = Path.GetFileName(defaultName),
            InitialDirectory = _currentFilePath != null
                ? Path.GetDirectoryName(_currentFilePath)
                : null,
        };

        if (dialog.ShowDialog() != true) return;

        try
        {
            File.WriteAllText(dialog.FileName, _cleanedSource, Encoding.UTF8);
            SetStatus("Salvo", Path.GetFileName(dialog.FileName), "#4CAF50");
        }
        catch (Exception ex)
        {
            SetStatus("Erro ao salvar", ex.Message, "#F44336");
        }
    }

    private void BtnClearAll_Click(object sender, RoutedEventArgs e)
    {
        EditorOriginal.Text = string.Empty;
        EditorClean.Text = string.Empty;
        _cleanedSource = null;
        _currentFilePath = null;
        TxtOriginalFileName.Text = string.Empty;
        TxtCleanFileName.Text = string.Empty;
        TxtOriginalSize.Text = string.Empty;
        TxtCleanSize.Text = string.Empty;
        ResetStats();
        LanguageWarning.Visibility = Visibility.Collapsed;
        SetStatus("Pronto", "Aguardando codigo...", "#4CAF50");
        TxtHint.Text = "Abra um arquivo ou cole codigo para comecar.";
    }

    private void BtnSettings_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Configuracoes disponiveis em versoes futuras.", "Configuracoes",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void BtnHelp_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "CodeCleaner v1.0.0\n\n" +
            "Remove todos os comentarios de codigo com seguranca.\n\n" +
            "Linguagens suportadas:\n" +
            "  • Lua / Luau\n  • Python\n  • JavaScript\n  • TypeScript\n  • C#\n" +
            "  • SQL\n  • Ruby\n  • Go\n  • Kotlin\n  • Swift\n  • Bash / Shell\n" +
            "  • Rust\n  • HTML\n  • CSS\n  • PHP\n  • Java\n  • C\n  • C++\n" +
            "  • Dart\n  • PowerShell\n  • Scala\n  • R\n  • Perl\n  • Haskell\n  • Elixir\n\n" +
            "• Abra um arquivo, arraste para a janela ou cole codigo\n" +
            "• Selecione a linguagem\n" +
            "• Clique em 'Remover Comentarios'\n" +
            "• Copie ou salve o resultado\n\n" +
            "O arquivo original nunca e modificado.",
            "Ajuda — CodeCleaner",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void SetStatus(string title, string detail, string colorHex)
    {
        TxtStatusTitle.Text = title;
        TxtStatusDetail.Text = detail;
        var color = (Color)ColorConverter.ConvertFromString(colorHex);
        TxtStatusTitle.Foreground = new SolidColorBrush(color);
        StatusDot.Fill = new SolidColorBrush(color);
    }

    private void ResetStats()
    {
        TxtCommentsRemoved.Text = "0";
        TxtOriginalSizeStat.Text = "—";
        TxtCleanSizeStat.Text = "—";
        TxtReduction.Text = "—";
    }

    private static string FormatSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F2} KB";
        return $"{bytes / (1024.0 * 1024.0):F2} MB";
    }
}

internal sealed class LanguageItem
{
    public CoreLanguage Language { get; }
    private readonly string _label;
    public LanguageItem(CoreLanguage language, string label) { Language = language; _label = label; }
    public override string ToString() => _label;
}