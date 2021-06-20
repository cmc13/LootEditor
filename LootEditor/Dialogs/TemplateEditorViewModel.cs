using LootEditor.Models;
using LootEditor.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace LootEditor.Dialogs
{
    public class TemplateEditorViewModel : ObservableRecipient
    {
        private readonly TemplateService templateService;

        public TemplateEditorViewModel(TemplateService templateService)
        {
            this.templateService = templateService;
            templateService.TemplatesChanged += (s, e) => OnPropertyChanged(nameof(Templates));
        }

        public IEnumerable<RuleTemplate> Templates => templateService.GetTemplates();


    }
}
