using System.IO;
using System.Text.Json;

namespace LootEditor.Services;

public sealed record WindowSettings(
    double Width,
    double Height,
    double Top,
    double Left);

public static class WindowSettingsManager
{
    private static readonly string SettingsPath = Path.Combine(FileSystemService.AppDataDirectory, "windowsettings.json");

    public static WindowSettings Load()
    {
        if (File.Exists(SettingsPath))
        {
            var json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<WindowSettings>(json);
        }

        return null;
    }

    public static void Save(WindowSettings windowSettings)
    {
        var directory = Path.GetDirectoryName(SettingsPath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(windowSettings);
        File.WriteAllText(SettingsPath, json);
    }
}
