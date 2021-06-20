using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace LootEditor.Dialogs
{
    public class SaveRuleTemplateViewModel : ObservableRecipient
    {
        private string templateName;

        public string TemplateName
        {
            get => templateName;
            set
            {
                if (templateName != value)
                {
                    templateName = value;
                    OnPropertyChanged(nameof(TemplateName));
                }
            }
        }
    }
}
