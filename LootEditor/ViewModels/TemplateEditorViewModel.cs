using LootEditor.Models;
using LootEditor.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LootEditor.ViewModels
{
    public class TemplateEditorViewModel : ObservableRecipient
    {
        private readonly TemplateService templateService = new();
        private TemplateListItemViewModel selectedTemplate;
        private LootRuleViewModel selectedRule;

        public RelayCommand DeleteTemplateCommand { get; }

        public TemplateEditorViewModel()
        {
            templateService.TemplatesChanged += (s, e) => OnPropertyChanged(nameof(Templates));

            DeleteTemplateCommand = new RelayCommand(() => templateService.DeleteTemplate(SelectedTemplate.Template), () => SelectedTemplate != null);
        }

        public IEnumerable<TemplateListItemViewModel> Templates => templateService.GetTemplates().Select(t => new TemplateListItemViewModel(t, templateService));

        public TemplateListItemViewModel SelectedTemplate
        {
            get => selectedTemplate;
            set
            {
                if (selectedTemplate != value)
                {
                    selectedTemplate = value;
                    OnPropertyChanged(nameof(SelectedTemplate));

                    if (selectedTemplate != null)
                    {
                        templateService.GetRuleFromTemplate(selectedTemplate.Template)
                            .ContinueWith(t =>
                            {
                                SelectedRule = new LootRuleViewModel(t.Result);
                                OnPropertyChanged(nameof(SelectedRule));
                            }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnRanToCompletion);
                    }
                }
            }
        }

        public LootRuleViewModel SelectedRule
        {
            get => selectedRule;
            private set
            {
                if (selectedRule != value)
                {
                    if (selectedRule != null)
                        selectedRule.PropertyChanged -= SelectedRule_PropertyChanged;
                    selectedRule = value;
                    OnPropertyChanged(nameof(SelectedRule));
                    if (selectedRule != null)
                        selectedRule.PropertyChanged += SelectedRule_PropertyChanged;
                }
            }
        }

        private async void SelectedRule_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LootRuleViewModel.IsDirty))
            {
                if (SelectedRule.IsDirty)
                {
                    await templateService.SaveRuleAsTemplate(SelectedRule.Rule, SelectedTemplate.Name).ConfigureAwait(false);
                    SelectedRule.IsDirty = false;
                }
            }
        }
    }
}
