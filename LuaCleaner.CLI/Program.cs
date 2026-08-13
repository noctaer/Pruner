using LuaCleaner.Core;
using LuaCleaner.IO;

const string Version = "1.0.0";

if (args.Length == 0 || args[0] is "--help" or "-h")
{
    PrintHelp();
    return 0;
}

if (args[0] is "--version" or "-v")
{
    Console.WriteLine($"CodeCleaner {Version}");
    return 0;
}

bool dryRun = false;
bool recursive = false;
var inputs = new List<string>();

foreach (string arg in args)
{
    switch (arg)
    {
        case "--dry-run":
        case "-n":
            dryRun = true;
            break;
        case "--recursive":
        case "-r":
            recursive = true;
            break;
        default:
            inputs.Add(arg);
            break;
    }
}

if (inputs.Count == 0)
{
    Console.Error.WriteLine("No input files or directories specified.");
    Console.Error.WriteLine("Run 'CodeCleaner --help' for usage.");
    return 1;
}

IReadOnlyList<string> paths = FileProcessor.ResolveInputPaths(inputs, recursive);

if (paths.Count == 0)
{
    Console.Error.WriteLine("No supported files found.");
    return 1;
}

Console.WriteLine("CodeCleaner");
Console.WriteLine("-----------");
if (dryRun)
    Console.WriteLine("DRY RUN — no files will be written");
Console.WriteLine();

int totalFiles = 0;
int totalErrors = 0;
int totalComments = 0;
long totalOriginal = 0;
long totalCleaned = 0;

foreach (string path in paths)
{
    ProcessResult result = FileProcessor.Process(path, dryRun: dryRun);

    if (!result.IsSuccess)
    {
        Console.Error.WriteLine($"  ERROR  {path}");
        Console.Error.WriteLine($"         {result.ErrorMessage}");
        totalErrors++;
        continue;
    }

    totalFiles++;
    totalComments += result.CommentsRemoved;
    totalOriginal += result.OriginalSize;
    totalCleaned += result.CleanedSize;

    string status = dryRun ? "  DRY  " : "  OK   ";
    string lang = LanguageDetector.DisplayName(result.Language!.Value).PadRight(14);
    string comments = result.CommentsRemoved.ToString().PadLeft(4);
    Console.WriteLine($"{status} [{lang}] {comments} comments  {Path.GetFileName(path)}");

    if (!dryRun)
        Console.WriteLine($"         → {result.OutputPath}");
}

Console.WriteLine();
Console.WriteLine($"Files processed : {totalFiles}");
if (totalErrors > 0)
    Console.WriteLine($"Errors          : {totalErrors}");
Console.WriteLine($"Comments removed: {totalComments}");
Console.WriteLine($"Original size   : {FormatSize(totalOriginal)}");
Console.WriteLine($"Cleaned size    : {FormatSize(totalCleaned)}");

if (totalOriginal > 0)
{
    double reduction = (1.0 - (double)totalCleaned / totalOriginal) * 100.0;
    Console.WriteLine($"Reduction       : {reduction:F1}%");
}

Console.WriteLine();
if (dryRun)
    Console.WriteLine("Dry run complete. No files written.");
else
    Console.WriteLine("Done.");

return totalErrors > 0 ? 1 : 0;

static string FormatSize(long bytes)
{
    if (bytes < 1024) return $"{bytes} B";
    if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
    return $"{bytes / (1024.0 * 1024.0):F1} MB";
}

static void PrintHelp()
{
    Console.WriteLine("CodeCleaner — removes all comments from source code files");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  CodeCleaner <file> [file2 ...] [options]");
    Console.WriteLine("  CodeCleaner <directory> [options]");
    Console.WriteLine("  CodeCleaner *.cs [options]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --dry-run   -n   Preview changes without writing files");
    Console.WriteLine("  --recursive -r   Process subdirectories recursively");
    Console.WriteLine("  --help      -h   Show this help");
    Console.WriteLine("  --version   -v   Show version");
    Console.WriteLine();
    Console.WriteLine("Supported languages:");
    Console.WriteLine("  .lua .luau              Lua / Luau");
    Console.WriteLine("  .py .pyw                Python");
    Console.WriteLine("  .js .mjs .cjs           JavaScript");
    Console.WriteLine("  .ts .mts                TypeScript");
    Console.WriteLine("  .cs                     C#");
    Console.WriteLine("  .sql                    SQL");
    Console.WriteLine("  .rb .rake .gemspec      Ruby");
    Console.WriteLine("  .go                     Go");
    Console.WriteLine("  .kt .kts                Kotlin");
    Console.WriteLine("  .swift                  Swift");
    Console.WriteLine("  .sh .bash .zsh .fish    Bash / Shell");
    Console.WriteLine("  .rs                     Rust");
    Console.WriteLine("  .html .htm              HTML");
    Console.WriteLine("  .css                    CSS");
    Console.WriteLine("  .php .php8              PHP");
    Console.WriteLine();
    Console.WriteLine("Output:");
    Console.WriteLine("  Saved as <name>.clean.<ext> in the same directory.");
    Console.WriteLine("  The original file is never modified.");
}