using LootEditor.Dialogs;
using LootEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace LootEditor.Services
{
    public class TemplateService
    {
        private static readonly string TEMPLATES_FOLDER = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loot Editor", "Templates");
        private readonly FileSystemWatcher fsw = new(TEMPLATES_FOLDER, "*.ruleTemplate");
        private readonly DialogService dialogService = new();

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

        public async Task<LootRule> GetRuleFromTemplate(RuleTemplate template)
        {
            if (File.Exists(template.FileName))
            {
                using var fs = File.OpenRead(template.FileName);
                using var reader = new StreamReader(fs);
                var rule = await LootRule.ReadRuleAsync(1, reader);
                return rule;
            }

            return null;
        }

        public async Task SaveRuleAsTemplate(LootRule rule)
        {
            var vm = new SaveRuleTemplateViewModel();
            while (true)
            {
                var result = dialogService.ShowDialog("Name Template", vm);
                if (result == null || result.Value == false)
                    return;

                if (!string.IsNullOrWhiteSpace(vm.TemplateName))
                {
                    if (File.Exists(Path.Combine(TEMPLATES_FOLDER, vm.TemplateName + ".ruleTemplate")))
                    {
                        var mbResult = MessageBox.Show(
                            $"A template named {vm.TemplateName} already exists. Would you like to overwrite it?",
                            "Overwrite Existing Template",
                            MessageBoxButton.YesNoCancel,
                            MessageBoxImage.Question);
                        if (mbResult == MessageBoxResult.No)
                            continue;
                        else if (mbResult == MessageBoxResult.Cancel)
                            return;
                    }
                    break;
                }
            }

            if (!Directory.Exists(TEMPLATES_FOLDER))
                Directory.CreateDirectory(TEMPLATES_FOLDER);
            using var fs = File.OpenWrite(Path.Combine(TEMPLATES_FOLDER, vm.TemplateName + ".ruleTemplate"));
            using var writer = new StreamWriter(fs);
            await rule.WriteAsync(writer).ConfigureAwait(false);
            await fs.FlushAsync().ConfigureAwait(false);
        }
    }
}
