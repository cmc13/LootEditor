using LootEditor.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LootEditor.Dialogs
{
    public class ImportRulesViewModel : ObservableRecipient
    {
        private readonly LootFile fileToImport;

        public ImportRulesViewModel(LootFile fileToImport)
        {
            this.fileToImport = fileToImport;
        }

        public IEnumerable<LootRule> ItemsToImport => fileToImport.Rules;

        public ObservableCollection<LootRule> CheckedRules { get; } = new();
    }
}
