using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Pruner.Core;
using Pruner.IO;

namespace Pruner.UI;

public partial class BatchWindow : Window
{
    private readonly string _folderPath;
    private readonly bool _recursive;
    private IReadOnlyList<string>? _resolvedFiles;
    private bool _processing;
    private bool _overwrite;

    public BatchWindow(string folderPath, bool recursive)
    {
        InitializeComponent();
        _folderPath = folderPath;
        _recursive = recursive;
        TxtFolderPath.Text = folderPath;
        Owner = Application.Current.MainWindow;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _resolvedFiles = FileProcessor.ResolveInputPaths(new[] { _folderPath }, _recursive);
        int count = _resolvedFiles.Count;

        if (count == 0)
        {
            TxtLogHeader.Text = "Nenhum arquivo suportado encontrado.";
            BtnProcess.IsEnabled = false;
            return;
        }

        TxtLogHeader.Text = $"{count} arquivo(s) encontrado(s). Clique em Processar para continuar.";
        TxtStatFiles.Text = count.ToString();

        foreach (string path in _resolvedFiles)
            AppendLog(Path.GetFileName(path), "#6B6B8A", Path.GetDirectoryName(path) ?? "");
    }

    private void ChkOverwrite_Changed(object sender, RoutedEventArgs e)
    {
        _overwrite = ChkOverwrite.IsChecked == true;

        if (_overwrite)
        {
            BtnProcess.Content = "Processar e Sobrescrever";
            BtnProcess.Background = new SolidColorBrush(Color.FromRgb(180, 60, 60));
        }
        else
        {
            BtnProcess.Content = "Processar";
            BtnProcess.Background = new SolidColorBrush(Color.FromRgb(74, 144, 217));
        }
    }

    private async void BtnProcess_Click(object sender, RoutedEventArgs e)
    {
        if (_resolvedFiles == null || _resolvedFiles.Count == 0 || _processing) return;

        if (_overwrite)
        {
            var confirm = MessageBox.Show(
                $"Os arquivos originais serao substituidos permanentemente.\n\n" +
                $"{_resolvedFiles.Count} arquivo(s) em:\n{_folderPath}\n\n" +
                $"Esta operacao nao pode ser desfeita. Continuar?",
                "Confirmar sobrescrita",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;
        }

        _processing = true;
        BtnProcess.IsEnabled = false;
        BtnClose.IsEnabled = false;
        ChkOverwrite.IsEnabled = false;
        ProgressBar.Visibility = Visibility.Visible;
        LogPanel.Children.Clear();

        int total = _resolvedFiles.Count;
        int processed = 0;
        int totalComments = 0;
        long totalOriginal = 0;
        long totalClean = 0;
        var failures = new List<(string file, string reason)>();

        await Task.Run(() =>
        {
            foreach (string path in _resolvedFiles)
            {
                try
                {
                    string ext = Path.GetExtension(path);
                    if (!LanguageDetector.TryDetect(ext, out Language language))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            failures.Add((path, "extensao nao suportada"));
                            AppendLog(Path.GetFileName(path), "#F44336", "extensao nao suportada");
                            processed++;
                            UpdateProgress(processed, total);
                            UpdateStats(processed, totalComments, totalOriginal, totalClean, failures.Count);
                        });
                        continue;
                    }

                    string source = File.ReadAllText(path, Encoding.UTF8);
                    long originalSize = Encoding.UTF8.GetByteCount(source);

                    var stripper = CommentStripperFactory.Get(language);
                    StripResult result = stripper.Strip(source);

                    long cleanSize = Encoding.UTF8.GetByteCount(result.CleanedSource);
                    string outputPath = _overwrite ? path : ResolveOutputPath(path);

                    File.WriteAllText(outputPath, result.CleanedSource, Encoding.UTF8);

                    Dispatcher.Invoke(() =>
                    {
                        processed++;
                        totalComments += result.CommentsRemoved;
                        totalOriginal += originalSize;
                        totalClean += cleanSize;

                        UpdateProgress(processed, total);

                        string saved = FormatSize(originalSize - cleanSize);
                        string label = result.CommentsRemoved > 0
                            ? $"{Path.GetFileName(path)}  —  {result.CommentsRemoved} comentario(s), -{saved}"
                            : $"{Path.GetFileName(path)}  —  sem comentarios";
                        string color = result.CommentsRemoved > 0 ? "#E0E0E0" : "#6B6B8A";
                        string secondary = _overwrite
                            ? "sobrescrito"
                            : Path.GetFileName(outputPath);

                        AppendLog(label, color, secondary);
                        UpdateStats(processed, totalComments, totalOriginal, totalClean, failures.Count);
                    });
                }
                catch (Exception ex)
                {
                    string captured = ex.Message;
                    Dispatcher.Invoke(() =>
                    {
                        processed++;
                        failures.Add((path, captured));
                        AppendLog(Path.GetFileName(path), "#F44336", captured);
                        UpdateProgress(processed, total);
                        UpdateStats(processed, totalComments, totalOriginal, totalClean, failures.Count);
                    });
                }
            }
        });

        string mode = _overwrite ? "arquivos sobrescritos" : "arquivos .clean gerados";
        TxtLogHeader.Text = failures.Count == 0
            ? $"Concluido — {processed} {mode}."
            : $"Concluido com {failures.Count} falha(s) — {processed - failures.Count} {mode}.";

        ProgressBar.Visibility = Visibility.Collapsed;
        TxtProgress.Text = "";
        BtnClose.IsEnabled = true;
        _processing = false;
    }

    private void UpdateProgress(int processed, int total)
    {
        ProgressBar.Value = (double)processed / total * 100;
        TxtProgress.Text = $"{processed} / {total}";
    }

    private void UpdateStats(int files, int comments, long original, long clean, int errors)
    {
        TxtStatFiles.Text = files.ToString();
        TxtStatComments.Text = comments.ToString();
        TxtStatOriginal.Text = FormatSize(original);
        TxtStatSaved.Text = original > clean ? FormatSize(original - clean) : "0 B";
        TxtStatErrors.Text = errors.ToString();
        TxtStatErrors.Foreground = errors > 0
            ? new SolidColorBrush(Color.FromRgb(244, 67, 54))
            : new SolidColorBrush(Color.FromRgb(224, 224, 224));
    }

    private void AppendLog(string primary, string colorHex, string secondary)
    {
        var color = (Color)ColorConverter.ConvertFromString(colorHex);
        var row = new StackPanel { Margin = new Thickness(0, 2, 0, 2) };

        row.Children.Add(new TextBlock
        {
            Text = primary,
            Foreground = new SolidColorBrush(color),
            FontFamily = new FontFamily("Segoe UI"),
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
        });

        if (!string.IsNullOrEmpty(secondary))
        {
            row.Children.Add(new TextBlock
            {
                Text = secondary,
                Foreground = new SolidColorBrush(Color.FromRgb(107, 107, 138)),
                FontFamily = new FontFamily("Consolas"),
                FontSize = 10,
                TextWrapping = TextWrapping.Wrap,
            });
        }

        LogPanel.Children.Add(row);
        LogScroll.ScrollToBottom();
    }

    private static string ResolveOutputPath(string inputPath)
    {
        string dir = Path.GetDirectoryName(inputPath) ?? ".";
        string nameWithoutExt = Path.GetFileNameWithoutExtension(inputPath);
        string ext = Path.GetExtension(inputPath);
        string candidate = Path.Combine(dir, $"{nameWithoutExt}.clean{ext}");

        if (!File.Exists(candidate)) return candidate;

        int counter = 1;
        while (true)
        {
            candidate = Path.Combine(dir, $"{nameWithoutExt}.clean.{counter}{ext}");
            if (!File.Exists(candidate)) return candidate;
            counter++;
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

    private static string FormatSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        return $"{bytes / (1024.0 * 1024.0):F1} MB";
    }
}