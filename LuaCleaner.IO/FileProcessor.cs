using System.Text;
using LuaCleaner.Core;

namespace LuaCleaner.IO;

public static class FileProcessor
{
    public static ProcessResult Process(string inputPath, Language? languageOverride = null, bool dryRun = false)
    {
        if (!File.Exists(inputPath))
            return ProcessResult.Failure($"File not found: {inputPath}");

        string extension = Path.GetExtension(inputPath);

        Language language;
        if (languageOverride.HasValue)
        {
            language = languageOverride.Value;
        }
        else if (!LanguageDetector.TryDetect(extension, out language))
        {
            string supported = string.Join(", ", LanguageDetector.SupportedExtensions);
            return ProcessResult.Failure($"Unsupported extension '{extension}'. Supported: {supported}");
        }

        string source;
        try
        {
            source = File.ReadAllText(inputPath, Encoding.UTF8);
        }
        catch (UnauthorizedAccessException)
        {
            return ProcessResult.Failure($"Permission denied reading: {inputPath}");
        }
        catch (IOException ex)
        {
            return ProcessResult.Failure($"I/O error reading file: {ex.Message}");
        }

        var stripper = CommentStripperFactory.Get(language);
        StripResult result = stripper.Strip(source);

        long originalSize = new FileInfo(inputPath).Length;

        if (dryRun)
        {
            long cleanedSize = Encoding.UTF8.GetByteCount(result.CleanedSource);
            string projectedOutput = ResolveOutputPath(inputPath);
            return ProcessResult.Success(inputPath, projectedOutput, result.CommentsRemoved, originalSize, cleanedSize, language, dryRun: true);
        }

        string outputPath = ResolveOutputPath(inputPath);

        try
        {
            File.WriteAllText(outputPath, result.CleanedSource, Encoding.UTF8);
        }
        catch (UnauthorizedAccessException)
        {
            return ProcessResult.Failure($"Permission denied writing: {outputPath}");
        }
        catch (IOException ex)
        {
            return ProcessResult.Failure($"I/O error writing file: {ex.Message}");
        }

        long writtenSize = new FileInfo(outputPath).Length;

        return ProcessResult.Success(inputPath, outputPath, result.CommentsRemoved, originalSize, writtenSize, language, dryRun: false);
    }

    public static IReadOnlyList<string> ResolveInputPaths(IEnumerable<string> rawArgs, bool recursive)
    {
        var paths = new List<string>();

        foreach (string arg in rawArgs)
        {
            string fullArg = Path.GetFullPath(arg);

            if (Directory.Exists(fullArg))
            {
                var option = recursive
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly;

                foreach (string ext in LanguageDetector.SupportedExtensions)
                {
                    paths.AddRange(Directory.GetFiles(fullArg, $"*{ext}", option));
                }
                continue;
            }

            string? dir = Path.GetDirectoryName(fullArg);
            string pattern = Path.GetFileName(fullArg);

            if (dir != null && pattern.Contains('*') || pattern.Contains('?'))
            {
                var option = recursive
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly;

                string searchDir = string.IsNullOrEmpty(dir) ? "." : dir;
                paths.AddRange(Directory.GetFiles(searchDir, pattern, option));
                continue;
            }

            paths.Add(fullArg);
        }

        return paths
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(p => !Path.GetFileName(p).Contains(".clean.") &&
                        !Path.GetFileNameWithoutExtension(p).EndsWith(".clean"))
            .ToList();
    }

    private static string ResolveOutputPath(string inputPath)
    {
        string dir = Path.GetDirectoryName(inputPath) ?? ".";
        string nameWithoutExt = Path.GetFileNameWithoutExtension(inputPath);
        string ext = Path.GetExtension(inputPath);
        string candidate = Path.Combine(dir, $"{nameWithoutExt}.clean{ext}");

        if (!File.Exists(candidate))
            return candidate;

        int counter = 1;
        while (true)
        {
            candidate = Path.Combine(dir, $"{nameWithoutExt}.clean.{counter}{ext}");
            if (!File.Exists(candidate))
                return candidate;
            counter++;
        }
    }
}

public sealed class ProcessResult
{
    public bool IsSuccess { get; private init; }
    public bool IsDryRun { get; private init; }
    public string? ErrorMessage { get; private init; }
    public string? InputPath { get; private init; }
    public string? OutputPath { get; private init; }
    public int CommentsRemoved { get; private init; }
    public long OriginalSize { get; private init; }
    public long CleanedSize { get; private init; }
    public Language? Language { get; private init; }

    public static ProcessResult Success(
        string inputPath, string outputPath, int commentsRemoved,
        long originalSize, long cleanedSize, Language language, bool dryRun = false) =>
        new()
        {
            IsSuccess = true,
            IsDryRun = dryRun,
            InputPath = inputPath,
            OutputPath = outputPath,
            CommentsRemoved = commentsRemoved,
            OriginalSize = originalSize,
            CleanedSize = cleanedSize,
            Language = language,
        };

    public static ProcessResult Failure(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };
}