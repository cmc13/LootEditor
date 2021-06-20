using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootEditor.ViewModels
{
    public class SettingsViewModel : ObservableRecipient
    {
        private readonly TemplateEditorViewModel templateEditorViewModel = new();

        public TemplateEditorViewModel TemplateEditorViewModel => templateEditorViewModel;
    }
}
