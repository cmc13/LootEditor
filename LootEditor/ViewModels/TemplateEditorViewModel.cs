using LootEditor.Models;
using LootEditor.Services;
using LootEditor.ViewModels;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace LootEditor.ViewModels
{
    public class TemplateEditorViewModel : ObservableRecipient
    {
        private readonly TemplateService templateService = new();
        private RuleTemplate selectedTemplate;

        public TemplateEditorViewModel()
        {
            templateService.TemplatesChanged += (s, e) => OnPropertyChanged(nameof(Templates));
        }

        public IEnumerable<RuleTemplate> Templates => templateService.GetTemplates();

        public RuleTemplate SelectedTemplate
        {
            get => selectedTemplate;
            set
            {
                if (selectedTemplate != value)
                {
                    selectedTemplate = value;
                    OnPropertyChanged(nameof(SelectedTemplate));

                    templateService.GetRuleFromTemplate(selectedTemplate)
                        .ContinueWith(t =>
                        {
                            SelectedRule = new LootRuleViewModel(t.Result);
                            OnPropertyChanged(nameof(SelectedRule));
                        });
                }
            }
        }

        public LootRuleViewModel SelectedRule { get; private set; }
    }
}
