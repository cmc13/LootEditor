using LootEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace LootEditor.Services
{
    public class TemplateService
    {
        private static readonly string TEMPLATES_FOLDER = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loot Editor", "Templates");
        private readonly FileSystemWatcher fsw = new FileSystemWatcher(TEMPLATES_FOLDER, "*.ruleTemplate");

        public event EventHandler TemplatesChanged;

        public TemplateService()
        {
            //fsw.Changed += (s, e) => TemplatesChanged?.Invoke(this, EventArgs.Empty);
            fsw.Created += (s, e) => TemplatesChanged?.Invoke(this, EventArgs.Empty);
            fsw.Deleted += (s, e) => TemplatesChanged?.Invoke(this, EventArgs.Empty);
            fsw.Renamed += (s, e) => TemplatesChanged?.Invoke(this, EventArgs.Empty);
            fsw.EnableRaisingEvents = true;
        }

        public IEnumerable<RuleTemplate> GetTemplates()
        {
            if (Directory.Exists(TEMPLATES_FOLDER))
            {
                foreach (var file in Directory.EnumerateFiles(TEMPLATES_FOLDER, "*.ruleTemplate"))
                    yield return new RuleTemplate(file);
            }
        }
    }
}
