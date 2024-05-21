using LootEditor.Dialogs;
using LootEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace LootEditor.Services;

public class TemplateService
{
    private static readonly string TEMPLATES_FOLDER = Path.Combine(FileSystemService.AppDataDirectory, "Templates");
    private readonly FileSystemWatcher fsw;
    private readonly DialogService dialogService = new();
    private readonly FileSystemService fileSystemService = new();

    public event EventHandler TemplatesChanged;

    public TemplateService()
    {
        fileSystemService.TryCreateDirectory(TEMPLATES_FOLDER);
        fsw = new(TEMPLATES_FOLDER, "*.ruleTemplate");
        //fsw.Changed += (s, e) => TemplatesChanged?.Invoke(this, EventArgs.Empty);
        fsw.Created += (s, e) => TemplatesChanged?.Invoke(this, EventArgs.Empty);
        fsw.Deleted += (s, e) => TemplatesChanged?.Invoke(this, EventArgs.Empty);
        fsw.Renamed += (s, e) => TemplatesChanged?.Invoke(this, EventArgs.Empty);
        fsw.EnableRaisingEvents = true;
    }

    public void DeleteTemplate(RuleTemplate template)
    {
        fileSystemService.DeleteFile(template.FileName);
    }

    public IEnumerable<RuleTemplate> GetTemplates()
    {
        if (fileSystemService.DirectoryExists(TEMPLATES_FOLDER))
        {
            foreach (var file in fileSystemService.GetFilesInDirectory(TEMPLATES_FOLDER, "*.ruleTemplate"))
                yield return new RuleTemplate(file);
        }
    }

    public async Task<LootRule> GetRuleFromTemplate(RuleTemplate template)
    {
        if (fileSystemService.FileExists(template.FileName))
        {
            using var fs = fileSystemService.OpenFileForReadAccess(template.FileName);
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
                if (fileSystemService.FileExists(Path.Combine(TEMPLATES_FOLDER, vm.TemplateName + ".ruleTemplate")))
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

        await SaveRuleAsTemplate(rule, vm.TemplateName).ConfigureAwait(false);
    }

    public async Task SaveRuleAsTemplate(LootRule rule, string templateName)
    {
        fileSystemService.TryCreateDirectory(TEMPLATES_FOLDER);
        using var fs = fileSystemService.OpenFileForWriteAccess(Path.Combine(TEMPLATES_FOLDER, templateName + ".ruleTemplate"));
        using var writer = new StreamWriter(fs);
        await rule.WriteAsync(writer).ConfigureAwait(false);
        await fs.FlushAsync().ConfigureAwait(false);
    }

    public void RenameTemplate(RuleTemplate template, string newName)
    {
        fileSystemService.MoveFile(template.FileName, Path.Combine(TEMPLATES_FOLDER, newName + ".ruleTemplate"));
    }
}
