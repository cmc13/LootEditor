using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LootEditor.Services;
using System.Collections.Generic;
using System.Linq;

namespace LootEditor.ViewModels;

public partial class TemplateEditorViewModel : ObservableRecipient
{
    private readonly TemplateService templateService = new();

    public TemplateEditorViewModel()
    {
        templateService.TemplatesChanged += (s, e) => OnPropertyChanged(nameof(Templates));
    }

    public IEnumerable<TemplateListItemViewModel> Templates => templateService.GetTemplates().Select(t => new TemplateListItemViewModel(t, templateService));

    [RelayCommand(CanExecute = nameof(DeleteTemplate_CanExecute))]
    private void DeleteTemplate() => templateService.DeleteTemplate(SelectedTemplate.Template);

    private bool DeleteTemplate_CanExecute() => SelectedTemplate != null;

    [ObservableProperty]
    private TemplateListItemViewModel selectedTemplate;

    partial void OnSelectedTemplateChanged(TemplateListItemViewModel value)
    {
        if (SelectedTemplate != null)
        {
            templateService.GetRuleFromTemplate(SelectedTemplate.Template)
                .ContinueWith(t =>
                {
                    SelectedRule = new LootRuleViewModel(t.Result);
                    OnPropertyChanged(nameof(SelectedRule));
                }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }

    [ObservableProperty]
    private LootRuleViewModel selectedRule;

    partial void OnSelectedRuleChanging(LootRuleViewModel oldValue, LootRuleViewModel newValue)
    {
        if (oldValue != null)
            oldValue.PropertyChanged -= SelectedRule_PropertyChanged;
    }

    partial void OnSelectedRuleChanged(LootRuleViewModel oldValue, LootRuleViewModel newValue)
    {
        if (newValue != null)
            newValue.PropertyChanged += SelectedRule_PropertyChanged;
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
