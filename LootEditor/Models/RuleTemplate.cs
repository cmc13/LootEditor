using System.IO;

namespace LootEditor.Models;

public class RuleTemplate
{
    public RuleTemplate(string fileName)
    {
        FileName = fileName;
    }

    public string FileName { get; }

    public string Name => Path.GetFileNameWithoutExtension(FileName);
}
