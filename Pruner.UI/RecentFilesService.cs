using System.IO;
using System.Text.Json;

namespace Pruner.UI;

internal sealed class RecentFilesService
{
    private const int MaxEntries = 10;
    private static readonly string StoragePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Pruner", "recent.json");

    private readonly List<string> _entries;

    public IReadOnlyList<string> Entries => _entries;

    public RecentFilesService()
    {
        _entries = Load();
    }

    public void Add(string filePath)
    {
        _entries.Remove(filePath);
        _entries.Insert(0, filePath);
        if (_entries.Count > MaxEntries)
            _entries.RemoveRange(MaxEntries, _entries.Count - MaxEntries);
        Save();
    }

    public void Remove(string filePath)
    {
        if (_entries.Remove(filePath))
            Save();
    }

    private static List<string> Load()
    {
        try
        {
            if (!File.Exists(StoragePath)) return new List<string>();
            string json = File.ReadAllText(StoragePath);
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(StoragePath)!);
            File.WriteAllText(StoragePath, JsonSerializer.Serialize(_entries));
        }
        catch { }
    }
}