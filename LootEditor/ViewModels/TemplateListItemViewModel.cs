using LootEditor.Models;
using LootEditor.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.IO;

namespace LootEditor.ViewModels
{
    public class TemplateListItemViewModel : ObservableRecipient
    {
        private bool isEditingTemplateName;
        private readonly TemplateService templateService;

        public RelayCommand DeleteTemplateCommand { get; }
        public RelayCommand EditTemplateNameCommand { get; }
        public RelayCommand DoneEditingTemplateCommand { get; }
        public RuleTemplate Template { get; }
        public bool IsEditingTemplateName
        {
            get => isEditingTemplateName;
            set
            {
                if (isEditingTemplateName != value)
                {
                    isEditingTemplateName = value;
                    OnPropertyChanged(nameof(IsEditingTemplateName));
                    EditTemplateNameCommand?.NotifyCanExecuteChanged();
                    DoneEditingTemplateCommand?.NotifyCanExecuteChanged();
                }
            }
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
            DeleteTemplateCommand = new RelayCommand(() => File.Delete(template.FileName));
            EditTemplateNameCommand = new RelayCommand(() => IsEditingTemplateName = true, () => !IsEditingTemplateName);
            DoneEditingTemplateCommand = new RelayCommand(() => IsEditingTemplateName = false, () => IsEditingTemplateName);
            Template = template;
            this.templateService = templateService;
        }
    }
}
