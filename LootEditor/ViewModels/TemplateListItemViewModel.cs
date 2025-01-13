using LootEditor.Models;
using LootEditor.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;

namespace LootEditor.ViewModels;

public partial class TemplateListItemViewModel : ObservableRecipient
{
    private readonly TemplateService templateService;

    public RuleTemplate Template { get; }

    [ObservableProperty]
    private bool isEditingTemplateName;

    partial void OnIsEditingTemplateNameChanged(bool value)
    {
        EditTemplateNameCommand?.NotifyCanExecuteChanged();
        DoneEditingTemplateCommand?.NotifyCanExecuteChanged();
    }

    public string Name
    {
        get => Template.Name;
        set
        {
            if (Template.Name != value)
            {
                // rename template
                templateService.RenameTemplate(Template, value);
                OnPropertyChanged(nameof(Name));
                IsEditingTemplateName = false;
            }
        }
    }

    public void CancelTemplateNameEdit()
    {
        Name = Template.Name;
        IsEditingTemplateName = false;
    }

    public TemplateListItemViewModel(RuleTemplate template, TemplateService templateService)
    {
        Template = template;
        this.templateService = templateService;
    }

    [RelayCommand]
    public void DeleteTemplate() => File.Delete(Template.FileName);

    [RelayCommand(CanExecute = nameof(EditTemplateName_CanExecute))]
    public void EditTemplateName() => IsEditingTemplateName = true;

    private bool EditTemplateName_CanExecute() => !IsEditingTemplateName;

    [RelayCommand(CanExecute = nameof(DoneEditingTemplate_CanExecute))]
    public void DoneEditingTemplate() => IsEditingTemplateName = false;

    private bool DoneEditingTemplate_CanExecute() => IsEditingTemplateName;
}
